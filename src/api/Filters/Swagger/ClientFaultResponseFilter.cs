
using System.Collections.Generic;
using API.Domain.Models;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Filters.Swagger
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

            // operation.Responses.Add("401", new OpenApiResponse 
            // { 
            //     Description = "Your request was apparently valid, but it lack authentication. Please provide your authentication token on your next request",
            //     Content = content
            // });

            // operation.Responses.Add("403", new OpenApiResponse 
            // { 
            //     Description = "Your request was valid, but our server is refusing action. You might not have the necessary permissions for a resource, or may need an account of some sort",
            //     Content = content
            // });
        }
    }

    public class ClientFault 
    {
        public string message { get; set; }
        public IEnumerable<Fault> faults { get; set; }

        public class Fault 
        {
            public string code { get; set; }
            public string error { get; set; }
            public string property { get; set; }
            public string value { get; set; }
        }
    }
}


