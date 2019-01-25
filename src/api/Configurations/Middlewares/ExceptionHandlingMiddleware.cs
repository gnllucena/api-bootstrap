using System;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using API.Domains.Models.Faults;
using FluentValidation;
using Microsoft.AspNetCore.Http;
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

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var statusCode = default(HttpStatusCode);
            
            object message = null;

            switch (exception)
            {
                case SecurityException security:
                    statusCode = HttpStatusCode.Forbidden;
                    message = "You shall not pass!";
                    break;
                case ArgumentNullException argumentNull:
                    statusCode = HttpStatusCode.NotFound;
                    message = "These aren't the droids you're looking for...";
                    break;
                case ValidationException validation:
                    statusCode = HttpStatusCode.BadRequest;

                    var client = new ClientFault();
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
                    break;
                case ArgumentOutOfRangeException argumentOutOfRange:
                case TaskCanceledException taskCanceled:
                default:
                    statusCode = HttpStatusCode.InternalServerError;

                    var server = new ServerFault();
                    server.message = "Something is not right... Please, call our monkeys!";

                    message = server;
                    break;
            }

            var result = JsonConvert.SerializeObject(message);

            context.Response.ContentType = "application/json; charset=utf-8";
            context.Response.StatusCode = (int)statusCode;

            await context.Response.WriteAsync(result, Encoding.UTF8);
        }
    }
}
