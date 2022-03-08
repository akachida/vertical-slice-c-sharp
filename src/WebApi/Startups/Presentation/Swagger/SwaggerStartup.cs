using Microsoft.OpenApi.Models;
using WebApi.Extensions;

namespace WebApi.Startups.Presentation.Swagger;

internal static class SwaggerStartup
{
    public static void AddMySwagger(this IServiceCollection service, IConfiguration configuration)
    {
        var swaggerSettings = configuration.GetMyOptions<SwaggerSettings>();
        if (!swaggerSettings.UseSwagger) return;

        service.AddEndpointsApiExplorer();
        service.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo()
            {
                Title = swaggerSettings.ApiName, Version = "v1"
            });
        });
    }

    public static void UseMySwagger(this IApplicationBuilder app, IConfiguration configuration)
    {
        var swaggerSettings = configuration.GetMyOptions<SwaggerSettings>();
        if (!swaggerSettings.UseSwagger) return;

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Application API");
            options.DefaultModelsExpandDepth(-1);
        });
    }
}
