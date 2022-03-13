using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data.PostgreSql;

public sealed class PostgreSqlContextFactory
{
    private readonly IConfiguration _configuration;

    public PostgreSqlContextFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public ApplicationContext Create(
        Action<DbContextOptionsBuilder<ApplicationContext>>? actionBuilder = default)
    {
        var builder = new DbContextOptionsBuilder<ApplicationContext>(
            new DbContextOptions<ApplicationContext>());

        builder.UseNpgsql(
            _configuration.GetConnectionString("PostgreSql"),
            optionsBuilder =>
            {
                optionsBuilder.EnableRetryOnFailure(10, TimeSpan.FromSeconds(30), null);
                optionsBuilder.MigrationsAssembly(typeof(ApplicationContext).Assembly.FullName);
            });

        actionBuilder?.Invoke(builder);

        return new ApplicationContext(builder.Options);
    }
}
