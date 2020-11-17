using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace API.Filters.Swashbuckle
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
                            Type = "string"
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
