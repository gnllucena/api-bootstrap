using System.Collections.Generic;
using API.Domain.Models.Faults;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Configurations.Filters.Swagger
{
    public class KnownTypesResponseFilter : IDocumentFilter
    {        
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            context.SchemaRegistry.GetOrRegister(typeof(ServerFault));
            context.SchemaRegistry.GetOrRegister(typeof(ClientFault));
        }
    }
}


