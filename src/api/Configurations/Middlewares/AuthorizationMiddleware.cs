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

        public async Task Invoke(HttpContext context, IAuthenticatedService authenticatedService)
        {
            var token = authenticatedService.Token();

            if (token == null) 
            {
                throw new SecurityException("Invalid token");
            }

            if (token.Issuer != "API Bootstrap Identity Server") 
            {
                throw new SecurityException($"Invalid issuer: { token.Issuer }");
            }

            // todo: database check
        }
    }
}
