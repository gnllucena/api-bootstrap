
using System.Collections.Generic;
using API.Domains.Models;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Configurations.Filters.Swashbuckle
{
    public class HttpHeadersRequestFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "X-JWT-Assertion",
                In = ParameterLocation.Header,
                Description = "JWT Token to validate authorization on resources",
                Required = true,
                Schema = new OpenApiSchema() 
                {
                    Type = "string" 
                }
            });
        }
    }
}
