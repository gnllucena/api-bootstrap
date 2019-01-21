using System;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace API.Configurations.Middlewares
{
    public class UnhandledErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public UnhandledErrorHandlingMiddleware(RequestDelegate next)
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
            var message = string.Empty;

            switch (exception)
            {
                case SecurityException security:
                    statusCode = HttpStatusCode.Forbidden;
                    message = "Você não pode acessar esse recurso";
                    break;
                case ArgumentNullException argumentNull:
                    statusCode = HttpStatusCode.NotFound;
                    message = argumentNull.ParamName;
                    break;
                case ArgumentOutOfRangeException argumentOutOfRange:
                    statusCode = HttpStatusCode.NotFound;
                    message = argumentOutOfRange.ParamName;
                    break;
                case ValidationException validation:
                    statusCode = HttpStatusCode.BadRequest;

                    message = $"{{ \"mensagem\": \"{ CleanData(validation.Message) }\", \"erros\": [ #erros-fluent-validation ] }}";

                    var erros = string.Empty;

                    foreach (var error in validation.Errors)
                    {
                        erros += $"{{ \"codigo\": \"{CleanData(error.ErrorCode)}\", \"erro\": \"{CleanData(error.ErrorMessage)}\", \"propriedade\": \"{CleanData(error.PropertyName)}\", \"valor\": \"{CleanData(error.AttemptedValue)}\" }},";
                    }

                    if (!string.IsNullOrWhiteSpace(erros))
                        erros = erros.Substring(0, erros.Length - 1);

                    message = message.Replace("#erros-fluent-validation", erros);
                    break;
                case TaskCanceledException taskCanceled:
                    statusCode = HttpStatusCode.BadGateway;
                    message = taskCanceled.Message;
                    break;
                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    message = exception.Message;
                    break;
            }

            var result = JsonConvert.SerializeObject(message);

            context.Response.ContentType = "application/json; charset=utf-8";
            context.Response.StatusCode = (int)statusCode;

            await context.Response.WriteAsync(result, Encoding.UTF8);
        }

        private string CleanData(object data)
        {
            if (data == null)
                return "null";

            return data.ToString().Replace("\n", string.Empty).Replace("\t", string.Empty).Replace("\r", string.Empty);
        }
    }
}
