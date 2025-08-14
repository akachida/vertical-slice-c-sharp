---
trigger: manual
description: 
globs: 
---

# AI Instructions for Building Integration Tests - .NET Application Layer

## CORE PRINCIPLES & ARCHITECTURE

### Testing Framework Stack
- **Primary Framework**: xUnit (.NET 9.0+)
- **Assertion Library**: FluentAssertions for expressive, readable assertions
- **Test Data Generation**: Bogus library for realistic fake data
- **Mocking Framework**: Moq for external dependency mocking
- **Database Testing**: Entity Framework Core with SQLite In-Memory provider
- **Integration Testing**: Microsoft.AspNetCore.Mvc.Testing for web API testing

### Project Structure Requirements
```
tests/Application.IntegrationTests/
├── {DomainArea}/                    # Organize by domain boundaries
│   ├── Commands/                    # CQRS Command handler tests
│   ├── Queries/                     # CQRS Query handler tests
│   └── Notifications/               # Event/Notification handler tests
├── Setups/
│   ├── IntegrationTestBase.cs       # Shared test infrastructure
│   ├── IntegrationTestCollection.cs # xUnit collection definition
│   └── Fixtures/                    # Test data builders
│       └── {Entity}Fixture.cs       # Domain entity builders
├── Application.IntegrationTests.csproj
└── Usings.cs                        # Global using statements
```

## MANDATORY MOCKING RULES

### MOCK EVERYTHING OUTSIDE APPLICATION/DOMAIN LAYERS
**Critical Rule**: Any dependency that exists outside the Application and Domain layers MUST be mocked:

- **Infrastructure Layer**: Database repositories, external APIs, file systems, email services
- **Persistence Layer**: Actual database connections, data access implementations
- **External Services**: HTTP clients, third-party APIs, message queues, cloud services
- **System Dependencies**: DateTime.Now, file I/O, network calls, environment variables
- **Cross-Cutting Concerns**: Logging, caching, authentication providers, authorization services

### REAL IMPLEMENTATIONS ALLOWED
Only use real implementations for:
- **Application Layer**: Command/Query handlers, application services, validators
- **Domain Layer**: Domain entities, value objects, domain services, business rules
- **In-Memory Database**: EF Core with SQLite in-memory for data persistence testing

## TEST CLASS STRUCTURE TEMPLATE

```csharp
using Application.IntegrationTests.Setups;
using Application.IntegrationTests.Setups.Fixtures.{Entity};
using Application.{Feature}.Commands; // or Queries/Notifications
using Bogus;
using FluentAssertions;
using MediatR;
using Moq;

namespace Application.IntegrationTests.{DomainArea}.{TestType};

[Collection("IntegrationTestCollection")]
public sealed class {FeatureName}Tests(IntegrationTestBase testBase)
{
    private static readonly Faker Faker = new();

    [Fact]
    public async Task Handle_{action}_{expected_outcome}_{condition}()
    {
        // Arrange
        var entity = {Entity}Fixture.BuildRandom();
        var command = new {Command}(/* parameters */);
        var mockService = new Mock<I{ExternalService}>();

        await testBase.Context.{Entity}.AddAsync(entity);
        await testBase.Context.SaveChangesAsync();

        var handler = new {Command}Handler(testBase.Context, mockService.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        // Additional assertions...

        mockService.Verify(x => x.{Method}(It.IsAny<{Type}>()), Times.Once);
    }
}
```

## FIXTURE PATTERN IMPLEMENTATION

### Entity Fixture Requirements
```csharp
public static class {Entity}Fixture
{
    private static readonly Faker Faker = new();

    public static {Entity} BuildRandom({OptionalParameters})
        => BuildList(1, {parameters}).First();

    public static IEnumerable<{Entity}> BuildList(int amount = 10, {OptionalParameters})
    {
        var entities = new List<{Entity}>();

        for (var i = 0; i < amount; i++)
        {
            var entity = {Entity}.Create(
                Faker.{PropertyType}.{Method}(),
                // Use Bogus methods matching domain requirements
            );

            entities.Add(entity);
        }

        return entities;
    }
}
```

## INTEGRATION TEST BASE SETUP

### Required Infrastructure Configuration
```csharp
public sealed class IntegrationTestBase : IDisposable
{
    public readonly ApplicationContext Context;
    public readonly ServiceProvider ServiceProvider;

    public IntegrationTestBase()
    {
        Faker.DefaultStrictMode = true;

        var serviceCollection = new ServiceCollection();
        var sqliteConnection = new SqliteConnection("DataSource=:memory:");
        sqliteConnection.Open();

        // Configure in-memory database
        serviceCollection.AddDbContext<ApplicationContext>(options =>
            options.UseSqlite(sqliteConnection));

        // Register Application layer services
        serviceCollection.AddMediatR(/* Application assemblies */);
        serviceCollection.AddAutoMapper(/* Application assemblies */);
        serviceCollection.AddValidatorsFromAssemblies(/* Application assemblies */);

        // Mock all external dependencies here

        ServiceProvider = serviceCollection.BuildServiceProvider();
        Context = ServiceProvider.GetRequiredService<ApplicationContext>();
        Context.Database.EnsureCreated();
    }
}
```

## TESTING PATTERNS & STANDARDS

### AAA Pattern (Mandatory)
- **Arrange**: Setup test data using Fixtures, configure mocks, prepare dependencies
- **Act**: Execute the handler/service method being tested
- **Assert**: Verify results using FluentAssertions, verify mock interactions

### Naming Conventions (Strict)
- **Test Classes**: `{FeatureName}Tests.cs`
- **Test Methods**: `Handle_{action}_{expected_outcome}_{condition}()`
- **Variables**: Descriptive names, avoid abbreviations
- **Fixtures**: `{Entity}Fixture.BuildRandom()`, `{Entity}Fixture.BuildList()`

### Error Testing Requirements
Always test both success and failure scenarios:
```csharp
[Fact]
public async Task Handle_returns_{error_type}_when_{condition}()
{
    // Arrange - Setup failure condition

    // Act
    var result = await handler.Handle(command, CancellationToken.None);
    var error = result.UnwrapError();

    // Assert
    result.IsError.Should().BeTrue();
    error.Should().BeOfType<{ExpectedErrorType}>();
}
```

## CQRS TESTING SPECIFICATIONS

### Command Handler Tests
- Test successful command execution
- Verify domain entity state changes
- Verify database persistence
- Mock and verify external service calls
- Test validation failures
- Test authorization failures

### Query Handler Tests
- Test successful data retrieval
- Verify correct data mapping
- Test filtering and pagination
- Mock external data sources
- Test not found scenarios

### Notification Handler Tests
- Verify event processing logic
- Mock external notification services
- Test failure handling and retries

## MOCK VERIFICATION PATTERNS

### External Service Mocking
```csharp
var mockExternalService = new Mock<I{ExternalService}>();
mockExternalService
    .Setup(x => x.{Method}(It.IsAny<{Type}>()))
    .ReturnsAsync({ExpectedResult});

// Verify interactions
mockExternalService.Verify(
    x => x.{Method}(It.Is<{Type}>(param => param.{Property} == expectedValue)),
    Times.Once);
```

### Publisher/Mediator Mocking
```csharp
var mockPublisher = new Mock<IPublisher>();
var expectedNotification = new {Notification}({parameters});

mockPublisher.Verify(
    x => x.Publish(expectedNotification, It.IsAny<CancellationToken>()),
    Times.Once);
```

## PROJECT CONFIGURATION REQUIREMENTS

### .csproj Dependencies
```xml
<PackageReference Include="xunit" />
<PackageReference Include="xunit.runner.visualstudio" />
<PackageReference Include="FluentAssertions" />
<PackageReference Include="Bogus" />
<PackageReference Include="Moq" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" />
<PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" />
<PackageReference Include="coverlet.collector" />
```

## EXECUTION GUIDELINES

1. **Analyze Application Layer**: Identify all Commands, Queries, and Notifications
2. **Create Domain-Organized Structure**: Group tests by business domain areas
3. **Build Fixtures First**: Create test data builders for all domain entities
4. **Implement Test Base**: Setup shared infrastructure with in-memory database
5. **Mock External Dependencies**: Identify and mock all non-Application/Domain services
6. **Follow AAA Pattern**: Structure every test with clear Arrange-Act-Assert sections
7. **Verify Interactions**: Always verify mock calls for external dependencies
8. **Test Error Scenarios**: Include negative test cases for each handler
9. **Use Descriptive Names**: Follow strict naming conventions for clarity
10. **Maintain Test Isolation**: Ensure tests don't depend on each other's state

---

## IMPLEMENTATION CHECKLIST

### Before Starting
- [ ] Analyze existing Application layer structure
- [ ] Identify all CQRS handlers (Commands, Queries, Notifications)
- [ ] Map domain entities and their relationships
- [ ] Identify external dependencies to mock

### Project Setup
- [ ] Create test project with proper dependencies
- [ ] Setup IntegrationTestBase with in-memory database
- [ ] Configure xUnit collection for test isolation

### Test Development
- [ ] Create fixtures for all domain entities
- [ ] Implement test classes following naming conventions
- [ ] Write tests for success scenarios
- [ ] Write tests for error scenarios
- [ ] Mock all external dependencies
- [ ] Verify mock interactions

### Quality Assurance
- [ ] Ensure all tests follow AAA pattern
- [ ] Verify proper test isolation
- [ ] Check code coverage requirements
- [ ] Validate naming conventions compliance
- [ ] Review mock usage for architectural boundaries
