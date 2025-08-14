---
trigger: manual
description: 
globs: 
---

# .NET 8 Testing Guidelines

This document outlines the rules and patterns for generating unit and integration tests for a .NET 8 application.

**Required NuGet Packages:**

* `xunit`
* `FluentAssertions`
* `Moq`
* `Bogus`

### Overall Instructions

1. Using `Thread.Sleep()` or `Task.Delay()` in tests can make them flaky and slow. In .NET 8+, use the `TimeProvider` abstraction to control time. You can use a `FakeTimeProvider` in tests to deterministically advance time instead of relying on actual delays.
2. Don't use reflection to access private members. Tests should only validate the public API of a class.
3. Always compare `DateTime` or `DateTimeOffset` fields like `CreatedAt` or `UpdatedAt` using methods like `BeOnOrAfter()` and `BeCloseTo()` from FluentAssertions.

---

### **Part 1: Unit Testing Instructions (Domain Layer)**

**Objective**: To test the internal business logic of domain objects in complete isolation from external concerns like databases or network calls. The focus is on validating behavior using concrete object instances.

**Core Rules**:

1. **No Test Host & No Mocking**:
   * **DO NOT** use `WebApplicationFactory` or any other test host that boots up the application's infrastructure.
   * **DO NOT** use mocking libraries like Moq. All collaborator objects required by the domain entity under test must be real, concrete instances, created manually or via a test data fixture.
2. **Instantiation**:
   * The class under test must be instantiated directly using its constructor (`new()`) or a test data fixture.
   * **ALWAYS** use the **Test Data Fixture** pattern (see Part 3) to create and configure instances for testing.
3. **Assertions**:
   * **ALWAYS** use the **FluentAssertions** library for assertions (`using FluentAssertions;`). Its fluent API is mandatory for readability.
   * Write specific and meaningful assertions. Instead of just `user.Should().NotBeNull()`, assert the actual state: `user.Status.Should().Be(UserStatus.Active)`.
4. **Structure (Arrange-Act-Assert)**:
   * Organize every test method using the `Arrange-Act-Assert` structure, clearly separated by comments or whitespace.
   * Use the `[Fact]` attribute from xUnit for parameter-less tests.
5. **Naming Rules**:
   * **Variables**: Use descriptive names (e.g., `command`, `handler`, `result`, `response`).
   * **Avoid abbreviations**, use full words.
   * **Follow established patterns** in the existing codebase.

   **5.1 Application Layer**
   * **Methods**: `Handle_{action_description_in_snake_case}`
   * Focus on testing command/query handlers.
   * Use descriptive action names that explain business intent.

   **5.2 Domain Layer**
   * **Happy Path**: `{DomainMethodName}` (exact method name from domain, converted to PascalCase).
   * **Exception Tests**: `{MethodName}_ShouldThrow_When_{specific_condition_in_PascalCase}`.
   * Be specific about the failure condition.

**Example Domain Unit Test Structure**:

```csharp
// No WebApplicationFactory. This is a plain xUnit test.
public class UserTests
{
    [Fact]
    public void Activate() // Happy Path: Matches the domain method name
    {
        // Arrange
        var inactiveUser = UserFixture.AnInactiveUser();

        // Act
        inactiveUser.Activate();

        // Assert
        inactiveUser.Status.Should().Be(UserStatus.Active);
        inactiveUser.ActivationDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Activate_ShouldThrow_WhenUserIsAlreadyActive() // Exception Path
    {
        // Arrange
        var activeUser = UserFixture.ADefaultActiveUser();

        // Act
        Action act = () => activeUser.Activate();

        // Assert
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("User is already active.");
    }
}
```

---

### **Part 2: Integration Testing Instructions (Application/Service Layer)**

**Objective**: To test an application service and its interaction with its direct dependencies (abstractions/interfaces). The focus is on verifying orchestration logic. External systems (database, external APIs) **MUST** be mocked.

**Core Rules**:

1. **Use `WebApplicationFactory`**:
   * Use `WebApplicationFactory<TEntryPoint>` to create an in-memory test server with a configured dependency injection (DI) container. `TEntryPoint` is typically your `Program.cs` or `Startup.cs` class.
   * Your test class should implement `IClassFixture<WebApplicationFactory<TEntryPoint>>`.
2. **Mock All External Dependencies**:
   * **ALWAYS** use a mocking library like **Moq** to replace any service that communicates with an external system (e.g., `IRepository`, `IApiClient`, `IMessageBroker`).
   * Replace the real services with your mocks in the DI container by configuring the `WebApplicationFactory`'s services.
3. **Structure and Naming**:
   * Follow the `Arrange-Act-Assert` structure.
   * Follow the `{MethodName}_Should_{ExpectedBehavior}_When_{Context}` naming convention.
4. **Stubbing and Verification**:
   * Use Moq's `Setup(...).ReturnsAsync(...)` to stub the behavior of mocked dependencies in the **Arrange** block.
   * Use Moq's `Verify(...)` to check for correct interactions with mocks in the **Assert** block.

**Example Application Service Integration Test Structure**:

```csharp
// The TEntryPoint is typically your Program class in modern .NET
public class UserServiceTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<INotificationClient> _notificationClientMock;
    private readonly UserService _userService;

    public UserServiceTests(WebApplicationFactory<Program> factory)
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _notificationClientMock = new Mock<INotificationClient>();

        // Create a client with services overridden for mocking
        var client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton(_userRepositoryMock.Object);
                services.AddSingleton(_notificationClientMock.Object);
            });
        });

        // Resolve the service under test from the test server's DI container
        _userService = client.Services.GetRequiredService<UserService>();
    }

    [Fact]
    public async Task CreateUser_Should_ReturnUserDto_WhenRequestIsValid()
    {
        // Arrange
        var request = CreateUserRequestFixture.ADefaultRequest();

        _userRepositoryMock
            .Setup(r => r.SaveAsync(It.IsAny<User>()))
            .ReturnsAsync((User userToSave) =>
            {
                userToSave.Id = 1; // Simulate the DB setting an ID on save
                return userToSave;
            });

        // Act
        var result = await _userService.CreateUserAsync(request);

        // Assert
        result.Email.Should().Be(request.Email);
        result.Id.Should().NotBe(0);

        // Verify interactions with mocks
        _userRepositoryMock.Verify(r => r.SaveAsync(It.IsAny<User>()), Times.Once);
        _notificationClientMock.Verify(n => n.SendWelcomeEmailAsync(request.Email), Times.Once);
    }
}
```

---

### **Part 3: Rules for Test Data Fixtures**

**Objective**: To create test data in a readable, maintainable, and flexible way, following a hybrid Builder/Object Mother pattern.

**Core Rules**:

1. **Use the Fixture Suffix**:
   * Classes designed to create test data for an object (e.g., `User`) must be named with a `Fixture` suffix (e.g., `UserFixture`).
2. **Implement a Fluent Builder with Static Factories**:
   * The fixture class itself acts as the builder. Its constructor should be private.
   * Provide a static factory method (e.g., `AUser()`, `ARequest()`) as the entry point to get a new instance of the fixture (the builder).
   * The fixture must have fluent `With...()` methods for every property to allow overrides.
   * The fixture must have a final `Build()` method that returns the configured test object.
3. **Provide Static Object Mother Methods**:
   * For common, reusable scenarios, provide static, argument-less methods that return a fully built object (e.g., `AnInactiveUser()`, `ADefaultRequest()`). These methods should use the fixture's own builder internally.
4. **Use Bogus for Defaults**:
   * Use the **Bogus** library (`using Bogus;`) to generate sensible, realistic default values inside the fixture's private constructor.

**Example Test Data Fixture**:

```csharp
// In a /Tests/Fixtures directory
using Bogus;

public class UserFixture
{
    private static readonly Faker Faker = new();

    private long _id;
    private string _name;
    private string _email;
    private UserStatus _status;

    // Private constructor sets realistic defaults
    private UserFixture()
    {
        _id = Faker.Random.Long(1);
        _name = Faker.Name.FullName();
        _email = Faker.Internet.Email();
        _status = UserStatus.Active;
    }

    // Static factory to get a builder instance
    public static UserFixture AUser() => new();

    // --- Object Mother Methods (for common scenarios) ---
    public static User AnInactiveUser() => AUser().WithStatus(UserStatus.Inactive).Build();
    public static User ADefaultActiveUser() => AUser().WithStatus(UserStatus.Active).Build();

    // --- Builder Methods (for customization) ---
    public UserFixture WithStatus(UserStatus status)
    {
        _status = status;
        return this;
    }

    public UserFixture WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public User Build()
    {
        return new User
        {
            Id = _id,
            Name = _name,
            Email = _email,
            Status = _status
        };
    }
}
```