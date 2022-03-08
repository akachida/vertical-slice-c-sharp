using Infrastructure.Data.PostgreSql;

namespace WebApi.Startups.Infrastructure.Persistence;

internal static class PostgreSqlStartup
{
    public static void AddMyPostgreSql(
        this IServiceCollection service,
        IHostEnvironment environment)
    {
        service.AddScoped<PostgreSqlContextFactory>();
        service.AddScoped(provider =>
        {
            var factory = provider.GetRequiredService<PostgreSqlContextFactory>();

            return factory.Create(builder =>
            {
                if (!environment.IsProduction())
                    builder.EnableSensitiveDataLogging();
            });
        });
    }
}
