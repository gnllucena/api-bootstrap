using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Security;
using System.IdentityModel.Tokens.Jwt;
using API.Domains.Services;
using Microsoft.Extensions.DependencyInjection;

namespace API.Configurations.Middlewares
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IAuthenticatedService authenticatedService, ISqlService sqlService)
        {
            var token = authenticatedService.Token();

            var uri = context.Request.Path.ToUriComponent();

            var path = "/users";

            Validate(token, uri, path);

            var paths = uri.Replace(path, string.Empty).Split('/', StringSplitOptions.RemoveEmptyEntries);

            var parameter = string.Empty;

            if (paths.Length > 0) 
            {
                parameter = paths[0];
            }

            if (Int32.TryParse(parameter, out int value)) 
            {
                await Authorize(value, token, sqlService);
            }

            await _next(context);
        }

        public void Validate(JwtSecurityToken token, string uri, string path) 
        {
            if (token == null) 
            {
                throw new SecurityException("Invalid token: null");
            }

            if (token.Issuer != "API Bootstrap Identity Server") 
            {
                throw new SecurityException($"Invalid issuer: { token.Issuer }");
            }

            if (!uri.Contains(path))
            {
                throw new SecurityException($"Invalid uri: { uri }");
            }
        }

        public async Task Authorize(int id, JwtSecurityToken token, ISqlService sqlService) 
        {
            var sql = "SELECT CreatedBy FROM bootstrap.User WHERE Id = @Id";

            var createdBy = await sqlService.ObtainAsync(sql, new {
                Id = id,
                CreatedBy = token.Subject
            });

            if (!string.IsNullOrWhiteSpace(createdBy) && createdBy != token.Subject) 
            {
                throw new SecurityException("Resource is not from authenticated user");   
            }
        }
    }
}
