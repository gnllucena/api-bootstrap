using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace API.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ILogger<LoggingMiddleware> logger)
        {
            var id = Guid.NewGuid().ToString();

            context.Response.Headers.Add("X-Request-ID", new[] { id });

            using (logger.BeginScope(id))
            {
                await _next(context);
            }
        }
    }
}
