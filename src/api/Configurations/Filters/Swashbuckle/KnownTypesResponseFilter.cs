using System.Collections.Generic;
using API.Domains.Models.Faults;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Configurations.Filters.Swashbuckle
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


