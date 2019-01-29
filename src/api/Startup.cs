using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using API.Domains.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using API.Domains.Models.Options;
using API.Configurations.Filters.Swashbuckle;
using API.Configurations.Factories;
using API.Configurations.Middlewares;
using API.Domains.Services;
using API.Domains.Validations;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using API.Configurations.Filters.Newtonsoft;

namespace API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Server
            services
                .AddMvcCore()
                .AddApiExplorer()
                .AddDataAnnotations()
                .AddJsonFormatters()
                .AddCors()
                .AddJsonOptions(options => 
                {
                    options.SerializerSettings.Converters.Add(new EnumConverterFilter());
                    options.SerializerSettings.Converters.Add(new BooleanConverterFilter());
                    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;

                    // options.SerializerSettings.Error = (sender, args) =>
                    // {
                    // };
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

#if (DEBUG)
            // Swagger
            services
                .AddSwaggerGen(options => 
                {
                    options.SwaggerDoc("v1", new OpenApiInfo
                    { 
                        Title = "API Bootstrap", 
                        Version = "v1",
                        Contact = new OpenApiContact
                        {
                            Name = "Gabriel Lucena",
                            Url = new Uri("https://www.github.com/gnllucena")
                        },
                        Description = @"API application with dynamic swagger documentation, endpoint for health checking, mysql container, dapper orm, fluentvalidator, jwt authentication, authorization, boolean and enum custom json converters."
                    });
                    
                    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme 
                    {
                        In = ParameterLocation.Header,
                        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey
                    });

                    options.DescribeAllEnumsAsStrings();

                    options.EnableAnnotations();
                    
                    options.DocumentFilter<KnownTypesResponseFilter>();
                    options.OperationFilter<ClientFaultResponseFilter>();
                    options.OperationFilter<ServerFaultResponseFilter>();
                    options.OperationFilter<HttpHeadersResponseFilter>();
                    // options.OperationFilter<HttpHeadersRequestFilter>();
                });
#endif
            // Healthcheck
            services.AddHealthChecks();

            // Appsettings configuration
            services.AddOptions();
            services.Configure<Database>(Configuration.GetSection("Database"));

            // Dependency injection
            // Factories
            services.AddScoped<IDatabaseFactory, DatabaseFactory>();

            // Services
            services.AddTransient<ISqlService, SqlService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IValidationService, ValidationService>();
            services.AddTransient<IAuthenticatedService, AuthenticatedService>();

            // Validators
            services.AddSingleton<IValidator<User>, UserValidator>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseHsts();

            app.UseHealthChecks("/healthcheck");
            
#if (DEBUG)            
            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Bootstrap");
            });
#endif

            app.UseHttpsRedirection();

            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseMiddleware<TransactionMiddleware>();
            app.UseMiddleware<AuthorizationMiddleware>();
            
            app.UseMvc();
        }
    }
}
