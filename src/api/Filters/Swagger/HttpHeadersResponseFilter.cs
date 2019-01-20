
using System.Collections.Generic;
using API.Domain.Models;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Filters.Swagger
{
    public class HttpHeadersResponseFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var header = new Dictionary<string, OpenApiHeader> 
            {
                {
                    "X-Request-ID", new OpenApiHeader() 
                    {
                        Description = "Correlates HTTP requests between client and server",
                        Schema = new OpenApiSchema() 
                        {
                            Type = "GUID" 
                        }
                    }
                }
            };

            foreach (var response in operation.Responses) 
            {
                response.Value.Headers = header;
            }
        }
    }
}
