using Common.Models.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;

namespace Common.Factories
{
    public interface IMessagingFactory
    {
        IModel Configure();
        void Disconnect();
    }

    public class MessagingFactory : IMessagingFactory
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly Messaging _messaging;
        private IModel _channel;
        private IConnection _connection;
        private readonly ILogger<MessagingFactory> _logger;

        public MessagingFactory(
            IOptions<Messaging> messaging,
            ILogger<MessagingFactory> logger)
        {
            _messaging = messaging.Value ?? throw new ArgumentNullException(nameof(messaging));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _connectionFactory = new ConnectionFactory()
            {
                HostName = _messaging.Host,
                Port = _messaging.Port,
                UserName = _messaging.User,
                Password = _messaging.Password,
                VirtualHost = _messaging.VirtualHost,
                DispatchConsumersAsync = true,
                AutomaticRecoveryEnabled = true,
                RequestedHeartbeat = new TimeSpan(10000)
            };
        }

        public IModel Configure()
        {
            if (_channel != null)
            {
                return _channel;
            }

            _logger.LogInformation("RABBITMQ | CREATING CONNECTION");
            _connection = _connectionFactory.CreateConnection();

            _logger.LogInformation("RABBITMQ | CREATING MODEL");
            _channel = _connection.CreateModel();

            CreateErrorStack();

            CreateConsumingStack();

            CreatePublishingStack();

            return _channel;
        }

        private void CreateErrorStack()
        {
            _logger.LogInformation($"RABBITMQ | CREATING ERROR EXCHANGE: {_messaging.Error.Exchange.Name}");

            _channel.ExchangeDeclare(_messaging.Error.Exchange.Name, ExchangeType(_messaging.Error.Exchange.Type), true);

            _logger.LogInformation($"RABBITMQ | CREATING ERROR QUEUE: {_messaging.Error.Queue}");

            _channel.QueueDeclare(_messaging.Error.Queue, true, false, false, null);

            _logger.LogInformation("RABBITMQ | BINDING ERROR EXCHANGE AND QUEUE");
            _channel.QueueBind(_messaging.Error.Queue, _messaging.Error.Exchange.Name, _messaging.Error.Routingkey);
        }

        private void CreateConsumingStack()
        {
            _logger.LogInformation($"RABBITMQ | CREATING DEADLETTER EXCHANGE: {_messaging.Consuming.Deadletter.Exchange.Name}");

            _channel.ExchangeDeclare(_messaging.Consuming.Deadletter.Exchange.Name, ExchangeType(_messaging.Consuming.Exchange.Type), true);

            _logger.LogInformation($"RABBITMQ | CREATING DEADLETTER QUEUE: {_messaging.Consuming.Deadletter.Queue}");

            _channel.QueueDeclare(_messaging.Consuming.Deadletter.Queue, true, false, false, new Dictionary<string, object>()
            {
                { "x-dead-letter-exchange", _messaging.Consuming.Exchange },
                { "x-dead-letter-routing-key", _messaging.Consuming.Bindingkey },
                { "x-message-ttl", _messaging.TTL }
            });

            _logger.LogInformation("RABBITMQ | BINDING DEADLETTER EXCHANGE AND QUEUE");
            _channel.QueueBind(_messaging.Consuming.Deadletter.Queue, _messaging.Consuming.Deadletter.Exchange.Name, _messaging.Consuming.Deadletter.Routingkey);

            _logger.LogInformation($"RABBITMQ | CREATING CONSUMING EXCHANGE: {_messaging.Consuming.Exchange.Name}");

            _channel.ExchangeDeclare(_messaging.Consuming.Exchange.Name, ExchangeType(_messaging.Consuming.Exchange.Type), true);

            _logger.LogInformation($"RABBITMQ | CREATING CONSUMING QUEUE: {_messaging.Consuming.Queue}");

            _channel.QueueDeclare(_messaging.Consuming.Queue, true, false, false, new Dictionary<string, object>()
            {
                { "x-dead-letter-exchange", _messaging.Consuming.Deadletter.Exchange.Name },
                { "x-dead-letter-routing-key", _messaging.Consuming.Deadletter.Routingkey }
            });

            _logger.LogInformation("RABBITMQ | BINDING CONSUMING EXCHANGE AND QUEUE");
            _channel.QueueBind(_messaging.Consuming.Queue, _messaging.Consuming.Exchange.Name, _messaging.Consuming.Bindingkey);
        }

        private void CreatePublishingStack()
        {
            if (
                !string.IsNullOrWhiteSpace(_messaging.Publishing.Queue) &&
                !string.IsNullOrWhiteSpace(_messaging.Publishing.Routingkey) &&
                !string.IsNullOrWhiteSpace(_messaging.Publishing.Exchange.Name) &&
                !string.IsNullOrWhiteSpace(_messaging.Publishing.Exchange.Type) &&
                !string.IsNullOrWhiteSpace(_messaging.Publishing.Deadletter.Queue) &&
                !string.IsNullOrWhiteSpace(_messaging.Publishing.Deadletter.Routingkey) &&
                !string.IsNullOrWhiteSpace(_messaging.Publishing.Deadletter.Exchange.Name) &&
                !string.IsNullOrWhiteSpace(_messaging.Publishing.Deadletter.Exchange.Type)
               )
            {
                _logger.LogInformation("RABBITMQ | CREATING POSTING EXCHANGE: {_messagingPublishingExchange}",
                    _messaging.Publishing.Exchange.Name);

                _channel.ExchangeDeclare(_messaging.Publishing.Exchange.Name, ExchangeType(_messaging.Publishing.Exchange.Type), true);

                _logger.LogInformation("RABBITMQ | CREATING POSTING QUEUE: {_messagingPublishingQueue}",
                    _messaging.Publishing.Queue);

                _channel.QueueDeclare(_messaging.Publishing.Queue, true, false, false, new Dictionary<string, object>()
                {
                    { "x-dead-letter-exchange", _messaging.Publishing.Deadletter.Exchange },
                    { "x-dead-letter-routing-key", _messaging.Publishing.Deadletter.Routingkey }
                });

                _logger.LogInformation("RABBITMQ | BINDING POSTING EXCHANGE AND QUEUE");
                _channel.QueueBind(_messaging.Publishing.Queue, _messaging.Publishing.Exchange.Name, _messaging.Publishing.Routingkey);

                _logger.LogInformation("RABBITMQ | CREATING POSTING DEADLETTER EXCHANGE: {_messagingPublishingDeadletterExchange}",
                    _messaging.Publishing.Deadletter.Exchange.Name);

                _channel.ExchangeDeclare(_messaging.Publishing.Deadletter.Exchange.Name, ExchangeType(_messaging.Publishing.Deadletter.Exchange.Type), true);

                _logger.LogInformation("RABBITMQ | CREATING POSTING DEADLETTER QUEUE: {_messagingPublishingDeadletterQueue}",
                    _messaging.Publishing.Deadletter.Queue);

                _channel.QueueDeclare(_messaging.Publishing.Deadletter.Queue, true, false, false, new Dictionary<string, object>()
                {
                    { "x-dead-letter-exchange", _messaging.Publishing.Exchange },
                    { "x-dead-letter-routing-key", _messaging.Publishing.Routingkey },
                    { "x-message-ttl", _messaging.TTL }
                });

                _logger.LogInformation("RABBITMQ | BINDING POSTING DEADLETTER EXCHANGE AND QUEUE");
                _channel.QueueBind(_messaging.Publishing.Deadletter.Queue, _messaging.Publishing.Deadletter.Exchange.Name, _messaging.Publishing.Deadletter.Routingkey);
            }
            else
            {
                _logger.LogInformation("RABBITMQ | PUBLISHING EXCHANGE AND QUEUE NOT CREATED");
            }
        }

        public void Disconnect()
        {
            if (_connection.IsOpen)
            {
                _connection.Close();
            }
        }

        private string ExchangeType(string exchangeType)
        {
            switch (exchangeType.ToLower())
            {
                case "direct":
                    return RabbitMQ.Client.ExchangeType.Direct;
                case "fanout":
                    return RabbitMQ.Client.ExchangeType.Fanout;
                case "headers":
                    return RabbitMQ.Client.ExchangeType.Headers;
                case "topic":
                    return RabbitMQ.Client.ExchangeType.Topic;
                default:
                    throw new NotImplementedException($"Exchange type {exchangeType} not implemented");
            }
        }
    }
}
