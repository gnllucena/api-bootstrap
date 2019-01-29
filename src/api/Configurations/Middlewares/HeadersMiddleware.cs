using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace API.Configurations.Middlewares
{
    public class HeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public HeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Response.OnStarting(state => {
                context.Response.Headers.Add("X-Request-ID", new[] { context.TraceIdentifier });
                
                return Task.FromResult(0);
            }, context);
            
            await _next(context);
        }
    }
}
