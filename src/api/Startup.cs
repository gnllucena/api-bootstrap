using API.Filters.Swashbuckle;
using API.Middlewares;
using Common.Domain.Entities;
using Common.Factories;
using Common.Models.Options;
using Common.Repositories;
using Common.Services;
using Common.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

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
            services
                .AddMvcCore()
                .AddApiExplorer()
                .AddDataAnnotations()
                .AddCors()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Converters.Add(new StringEnumConverter { AllowIntegerValues = true });
                    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                    options.SerializerSettings.Error = (sender, args) =>
                    {
                        throw args?.ErrorContext?.Error;
                    };
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "API",
                    Version = "v1",
                    Description = @"API operations."
                });

                options.EnableAnnotations();

                options.DocumentFilter<KnownTypesResponseFilter>();
                options.OperationFilter<ClientFaultResponseFilter>();
                options.OperationFilter<ServerFaultResponseFilter>();
                options.OperationFilter<HttpHeadersResponseFilter>();
            });

            services.AddHealthChecks();

            services.AddOptions();

            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());

            services.Configure<Connection>(Configuration.GetSection("Database"));
            services.Configure<Cache>(Configuration.GetSection("Cache"));
            services.Configure<Messaging>(Configuration.GetSection("Messaging"));

            services.AddSingleton<IDatabaseFactory, DatabaseFactory>();
            services.AddSingleton<IMessagingFactory, MessagingFactory>();
            services.AddSingleton<ICacheFactory, CacheFactory>();

            services.AddTransient<ISqlService, SqlService>();
            services.AddTransient<IValidationService, ValidationService>();
            services.AddTransient<IAuthenticatedService, AuthenticatedService>();
            services.AddTransient<IUserService, UserService>();

            services.AddSingleton<IValidator<User>, UserValidator>();

            services.AddScoped<IUserRepository, UserRepository>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(configuration => configuration
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithExposedHeaders("Content-Disposition"));

            app.UseHealthChecks("/healthcheck");

            app.UseSwagger(options =>
            {
                options.SerializeAsV2 = true;
            });

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "API");
            });

            app.UseHttpsRedirection();

            app.UseMiddleware<LoggingMiddleware>();
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseMiddleware<DatabaseTransactionMiddleware>();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
