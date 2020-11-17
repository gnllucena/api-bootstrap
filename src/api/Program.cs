using Amazon.CloudWatchLogs;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Json;
using Serilog.Sinks.AwsCloudWatch;
using System.IO;

namespace API
{
    public class Program
    {

        public static void Main(string[] args)
        {
            Log.Logger = CreateLogger();

            try
            {
                CreateWebHostBuilder(args).Build().Run();
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static Logger CreateLogger()
        {
            var group = "context";
            var application = "API.Context";

#if (!DEBUG)
            var client = Configuration.GetAWSOptions().CreateServiceClient<IAmazonCloudWatchLogs>();

            var options = new CloudWatchSinkOptions
            {
                LogGroupName = $"applications/{group}/{application}",
                LogStreamNameProvider = new DefaultLogStreamProvider(),
                BatchSizeLimit = 100,
                QueueSizeLimit = 10000,
                RetryAttempts = 3,
                LogGroupRetentionPolicy = LogGroupRetentionPolicy.FiveDays,
                TextFormatter = new JsonFormatter(),
                MinimumLogEventLevel = LogEventLevel.Information
            };   
#endif
            return new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", application)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Debug()
                .WriteTo.Console(
                    outputTemplate: "{NewLine}[{Timestamp:HH:mm:ss.fff} {Level:u3} {Application}] {Scope} {Message}{NewLine}{Exception}"
                )
#if (!DEBUG)
                .WriteTo.AmazonCloudWatch(options, client)
#endif
                .CreateLogger();
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseSerilog()
                .UseStartup<Startup>();

        private static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
    }
}
