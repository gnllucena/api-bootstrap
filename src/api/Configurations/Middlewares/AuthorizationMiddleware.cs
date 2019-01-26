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

            if (token == null) 
            {
                throw new SecurityException("Invalid token: null");
            }

            if (token.Issuer != "API Bootstrap Identity Server") 
            {
                throw new SecurityException($"Invalid issuer: { token.Issuer }");
            }

            var uri = context.Request.Path.ToUriComponent();

            var path = "/users/";

            if (!uri.Contains(path))
            {
                throw new SecurityException("Invalid uri for authorization");
            }

            var parameter = uri.Replace(path, string.Empty).Split('/')[0];

            if (Int32.TryParse(parameter, out int value)) 
            {
                var sql = "SELECT CreatedBy FROM bootstrap.User WHERE Id = @Id";

                var createdBy = await sqlService.ObtainAsync(sql, new {
                    Id = value,
                    CreatedBy = token.Subject
                });

                if (!string.IsNullOrWhiteSpace(createdBy) && createdBy != token.Subject) 
                {
                    throw new SecurityException("Resource is not from authenticated user");   
                }
            }

            await _next(context);
        }
    }
}
