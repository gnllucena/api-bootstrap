using System;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using API.Domains.Models.Faults;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static API.Domains.Models.Faults.ClientFault;

namespace API.Configurations.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ILogger<ExceptionHandlingMiddleware> logger)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex, logger);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception, ILogger<ExceptionHandlingMiddleware> logger)
        {
            var statusCode = default(HttpStatusCode);
            
            object message = null;

            switch (exception)
            {
                case SecurityException security:
                    statusCode = HttpStatusCode.Forbidden;

                    var securityex = new ServerFault();
                    securityex.message = "You shall not pass!";

                    message = securityex;

                    logger.LogWarning($"EXCEPTION HANDLING 403 | { security.Message }");
                    break;
                case ArgumentNullException argumentNull:
                    statusCode = HttpStatusCode.NotFound;

                    var argumentex = new ServerFault();
                    argumentex.message = "These aren't the droids you're looking for...";;

                    message = argumentex;

                    logger.LogInformation($"EXCEPTION HANDLING 404 | { argumentNull.Message }");
                    break;
                case ValidationException validation:
                    statusCode = HttpStatusCode.BadRequest;

                    var client = new ClientFault();
                    client.message = "Something is not right...";

                    foreach (var erro in validation.Errors)  
                    {
                        var fault = new Fault()
                        {
                            code = erro.ErrorCode,
                            error = erro.ErrorMessage,
                            property = erro.PropertyName,
                            value = erro.AttemptedValue == null ? "null" : erro.AttemptedValue.ToString()
                        };

                        client.faults.Add(fault);
                    }
                    
                    message = client;

                    logger.LogInformation($"EXCEPTION HANDLING 400 | { message }");
                    break;
                case ArgumentOutOfRangeException argumentOutOfRange:
                case TaskCanceledException taskCanceled:
                default:
                    statusCode = HttpStatusCode.InternalServerError;

                    var defaultex = new ServerFault();
                    defaultex.message = "Something is not right... Please, call one of our monkeys at help@bootstrap.com";

#if (DEBUG)
                    defaultex.message = $"Exception: {exception.Message} - Inner: {exception.InnerException?.Message} - Stacktrace: {exception.StackTrace}";
#endif

                    message = defaultex;

                    logger.LogError($"EXCEPTION HANDLING 500 | { exception }");
                    break;
            }

            var result = JsonConvert.SerializeObject(message);

            context.Response.ContentType = "application/json; charset=utf-8";
            context.Response.StatusCode = (int)statusCode;

            await context.Response.WriteAsync(result, Encoding.UTF8);
        }
    }
}
