using Common.Factories;
using Common.Models.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Services
{
    public interface IMessagingService
    {
        AsyncEventHandler<BasicDeliverEventArgs> Dequeue<T>(CancellationToken cancellationToken, Func<string, T, Task> callback);
        void Queue(string exchange, string routingKey, object message, Dictionary<string, object> headers = null);
    }

    public class MessagingService : IMessagingService
    {
        private readonly IMessagingFactory _messagingFactory;
        private readonly Messaging _messaging;
        private readonly ILogger<MessagingService> _logger;

        public MessagingService(
            IMessagingFactory messagingFactory,
            IOptions<Messaging> messaging,
            ILogger<MessagingService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _messagingFactory = messagingFactory ?? throw new ArgumentNullException(nameof(messagingFactory));
            _messaging = messaging.Value ?? throw new ArgumentNullException(nameof(messaging));
        }

        public AsyncEventHandler<BasicDeliverEventArgs> Dequeue<T>(CancellationToken cancellationToken, Func<string, T, Task> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            var channel = _messagingFactory.Configure();

            return async (model, ea) =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                _logger.LogDebug("MESSAGING | RECEIVING NEW MESSAGE");

                var raw = Encoding.UTF8.GetString(ea.Body.Span);
                var retries = 0;

                _logger.LogDebug("MESSAGING | RAW MESSAGE: { raw }");

                try
                {
                    retries = Retries(ea);

                    _logger.LogDebug("MESSAGING | THIS MESSAGE HAS BEEN PROCESSED { retries } TIMES");

                    var message = JsonConvert.DeserializeObject<T>(raw);

                    await callback.Invoke(raw, message).ConfigureAwait(false);

                    _logger.LogDebug("MESSAGING | MESSAGE WILL BE ACKED");

                    channel.BasicAck(ea.DeliveryTag, false);

                    _logger.LogDebug("MESSAGING | MESSAGE HAS BEEN ACKED");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"MESSAGING | SOMETHING HAPPENED WHEN PROCESSING THE MESSAGE: {ex}");

                    if (_messaging.Retries > retries)
                    {
                        channel.BasicNack(ea.DeliveryTag, false, false);

                        _logger.LogDebug("MESSAGING | MESSAGE HAS BEEN NACKED");
                    }
                    else
                    {
                        _logger.LogDebug("MESSAGING | MESSAGE WAS PROCESSED TOO MANY TIMES, PUSHING TO ERROR QUEUE");

                        var headers = new Dictionary<string, object>
                        {
                            { "queue", _messaging.Consuming.Queue },
                            { "exception", JsonConvert.SerializeObject(ex) }
                        };

                        Queue(_messaging.Error.Exchange.Name, _messaging.Error.Routingkey, raw, headers);

                        channel.BasicAck(ea.DeliveryTag, false);
                    }
                }
            };
        }

        public void Queue(string exchange, string routingKey, object value, Dictionary<string, object> headers = null)
        {
            var channel = _messagingFactory.Configure();

            var properties = channel.CreateBasicProperties();
            properties.Headers = headers;
            properties.Persistent = true;

            var message = JsonConvert.SerializeObject(value);

            _logger.LogDebug($"MESSAGING | PUSHING '{ message }' TO ROUTING KEY '{ routingKey }' ON '{ exchange }' EXCHANGE");

            channel.BasicPublish(exchange, routingKey, false, properties, Encoding.UTF8.GetBytes(message));

            _logger.LogDebug("MESSAGING | MESSAGE WAS SUCCESSFULLY PUSHED");
        }

        private int Retries(BasicDeliverEventArgs ea)
        {
            int count = 0;

            try
            {
                if (ea.BasicProperties.Headers is Dictionary<string, object> dic && dic.ContainsKey("x-death"))
                {
                    if (ea.BasicProperties.Headers["x-death"] is List<object> xdeath)
                    {
                        if (xdeath.FirstOrDefault() is Dictionary<string, object> headers)
                        {
                            count = Convert.ToInt32(headers["count"]);
                        }
                    }
                }
            }
            catch
            {
                count = 1;
            }

            return ++count;
        }
    }
}
