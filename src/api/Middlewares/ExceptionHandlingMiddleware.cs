using Common.Domain.Models.Responses;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace API.Middlewares
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
                    logger.LogWarning($"EXCEPTION HANDLING 403 | { security.Message }");

                    statusCode = HttpStatusCode.Forbidden;

                    message = new ServerFault
                    {
                        Message = "You shall not pass!"
                    };
                    break;
                case ValidationException validation:
                    logger.LogDebug($"EXCEPTION HANDLING 400 | { message }");

                    statusCode = HttpStatusCode.BadRequest;
                    
                    message = new ClientFault
                    {
                        Message = "Your request contains bad syntax or cannot be fulfiled",
                        Faults = validation.Errors.Select(x => new Fault()
                        {
                            Error = x.ErrorMessage,
                            Property = x.PropertyName,
                            Value = x.AttemptedValue == null ? "null" : x.AttemptedValue.ToString()
                        }).ToList()
                    };
                    break;
                case Newtonsoft.Json.JsonReaderException jsonReader:
                    logger.LogDebug($"EXCEPTION HANDLING 400 | { message }");

                    statusCode = HttpStatusCode.BadRequest;

                    message = new ClientFault
                    {
                        Message = "Our server failed to fulfill an apparently valid request",
                        Faults = new List<Fault>()
                        {
                            new Fault()
                            {
                                Error = "Could not cast value on property",
                                Property = jsonReader.Path,
                                Value = jsonReader.Message
                            }
                        }
                    };
                    break;
                default:
                    logger.LogError($"EXCEPTION HANDLING 500 | { exception }");

                    statusCode = HttpStatusCode.InternalServerError;

                    message = new ServerFault
                    {
                        Message = "Something happend"
                    };
                    break;
            }

            var result = JsonSerializer.Serialize(message);

            context.Response.ContentType = "application/json; charset=utf-8";
            context.Response.StatusCode = (int)statusCode;

            await context.Response.WriteAsync(result, Encoding.UTF8);
        }
    }
}
