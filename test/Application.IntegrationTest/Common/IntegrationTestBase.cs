using Domain.Users;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Domain.Enumerations.Users;

namespace Application.IntegrationTest.Common;

public class IntegrationTestBase : IDisposable
{
    public readonly ApplicationContext? Context;
    public readonly ServiceProvider ServiceProvider;

    public IntegrationTestBase()
    {
        Console.WriteLine("IntegrationTestBase initialized");

        var serviceCollection = new ServiceCollection();
        serviceCollection
            .AddDbContext<ApplicationContext>(builder =>
            {
                builder.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
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
