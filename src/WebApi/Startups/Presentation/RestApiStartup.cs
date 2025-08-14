using System.Reflection;
using System.Text.Json;
using FluentValidation;
using FluentValidation.AspNetCore;
using HealthChecks.UI.Client;
using MediatR;
using MicroElements.Swashbuckle.FluentValidation;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using SharedKernel.Helpers;

namespace WebApi.Startups.Presentation;

internal static class RestApiStartup
{
    public static void AddMyRestApi(this IServiceCollection service)
    {
        service.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
            Assembly.Load("Application"), 
            Assembly.Load("WebApi")));

        service.AddValidatorsFromAssemblies(new []
        {
            Assembly.Load("Application"),
            Assembly.Load("WebApi")
        });

        service.AddAutoMapper(Assembly.Load("Application"), Assembly.Load("WebApi"));

        service.AddHealthChecks();

        service.AddControllers()
            .AddControllersAsServices()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
            });

        service.AddFluentValidationAutoValidation()
               .AddFluentValidationClientsideAdapters();
    }

    public static void UseMyRestApi(this IApplicationBuilder app, IHostEnvironment environment)
    {
        app.UseEndpoints(endpoints =>
        {
            if (!environment.IsEnvironment("Debug"))
            {
                endpoints.MapControllers()
                    .RequireAuthorization();
            }
            else
            {
                endpoints.MapControllers();
            }

            endpoints.MapHealthChecks("/health", new HealthCheckOptions()
            {
                Predicate = _ => true
            });
            endpoints.MapHealthChecks("/healthz", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            endpoints.MapHealthChecksUI();
        });
    }
}
