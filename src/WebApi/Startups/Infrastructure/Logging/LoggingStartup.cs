using Serilog;
using Serilog.Events;
using WebApi.Extensions;
using WebApi.Helpers;

namespace WebApi.Startups.Infrastructure.Logging;

internal static class LoggingStartup
{
    public static IHostBuilder AddMySerilogLogging(this IHostBuilder webBuilder)
    {
        return webBuilder.UseSerilog((context, config) =>
        {
            config
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("EnvironmentName", context.HostingEnvironment.EnvironmentName)
                .Enrich.WithMachineName()
                .WriteTo.Console();

            if (!context.HostingEnvironment.IsProduction())
            {
                config
                    .WriteTo.Debug();
            }
            else
            {
                config
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("CollaborationPortal", LogEventLevel.Information);
            }

            var logglySettings = context.Configuration.GetMyOptions<LogglySettings>();
            if (logglySettings.WriteToLoggly.GetValueOrDefault())
            {
                config.WriteTo.Loggly(customerToken: logglySettings.CustomerToken);
            }
        });
    }

    public static IApplicationBuilder UseMyRequestLogging(this IApplicationBuilder builder)
    {
        return builder
            .UseSerilogRequestLogging(
                options => options.GetLevel = LogHelper.ExcludeHealthChecks);
    }
}
