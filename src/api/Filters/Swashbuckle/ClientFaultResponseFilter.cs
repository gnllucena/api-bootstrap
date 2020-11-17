using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace API.Filters.Swashbuckle
{
    public class ClientFaultResponseFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var schema = new OpenApiSchema()
            {
                Reference = new OpenApiReference()
                {
                    Type = ReferenceType.Schema,
                    Id = "ClientFault"
                }
            };

            var content = new Dictionary<string, OpenApiMediaType>
            {
                {
                    "text/plain", new OpenApiMediaType()
                    {
                        Schema = schema
                    }
                },
                {
                    "application/json", new OpenApiMediaType()
                    {
                        Schema = schema
                    }
                },
                {
                    "text/json", new OpenApiMediaType()
                    {
                        Schema = schema
                    }
                },
            };

            operation.Responses.Add("400", new OpenApiResponse
            {
                Description = "Your request contains bad syntax or cannot be fulfiled",
                Content = content
            });
        }
    }
}
