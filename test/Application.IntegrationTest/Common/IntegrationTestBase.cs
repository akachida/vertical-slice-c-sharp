using Domain.Users;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Domain.Enumerations.Users;

namespace Application.IntegrationTest.Common;

public class IntegrationTestBase : IDisposable
{
    private readonly string _connectionString =
        "User ID=postgres;Password=root;Host=localhost;Port=5432;Database=test;Pooling=true;";

    public readonly ApplicationContext? Context;
    public readonly ServiceProvider ServiceProvider;

    public IntegrationTestBase()
    {
        Console.WriteLine("IntegrationTestBase initialized");

        var serviceCollection = new ServiceCollection();
        serviceCollection
            .AddEntityFrameworkNpgsql()
            .AddDbContext<ApplicationContext>(builder =>
            {
                builder.UseNpgsql(
                    _connectionString,
                    optionsBuilder =>
                    {
                        optionsBuilder.EnableRetryOnFailure(10, TimeSpan.FromSeconds(30), null);
                        optionsBuilder.MigrationsAssembly(typeof(ApplicationContext).Assembly.FullName);
                    })
                    .EnableSensitiveDataLogging();
            });

        ServiceProvider = serviceCollection.BuildServiceProvider();

        Context = ServiceProvider.GetService<ApplicationContext>();

        try
        {
            Context?.Database.EnsureCreated();
            Seeds();
        }
        catch (Exception)
        {
            Console.WriteLine("An error occurred while migrating or seeding the database");
            throw;
        }
    }

    private void Seeds()
    {
        Context?.Users.Add(User.Create("admin@test.com", "123", "System", "Admin",
            UserType.Professor, UserLevel.Professor));

        Context?.SaveChanges();
    }

    public void Dispose()
    {
        Context?.Database.EnsureDeleted();
    }
}
