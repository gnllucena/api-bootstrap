using Common.Domain.Models.Responses;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Filters.Swashbuckle
{
    public class KnownTypesResponseFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            context.SchemaGenerator.GenerateSchema(typeof(ServerFault), context.SchemaRepository);
            context.SchemaGenerator.GenerateSchema(typeof(ClientFault), context.SchemaRepository);
        }
    }
}
