---
trigger: always_on
description: 
globs: 
---

# AI Architecture Guide: Vertical Slice Clean Architecture

## Overview
This codebase implements **Clean Architecture** with **Vertical Slice** organization using .NET 9, focusing on maintainability, testability, and feature-based development with **Minimal APIs**.

## Core Architecture Principles

### Layer Structure
```
Domain → Application → Infrastructure
   ↓         ↓            ↓
   └─── WebApi (Presentation) ←─┘
```

**Dependency Rules:**
- Domain has NO dependencies
- Application depends only on Domain
- Infrastructure depends on Domain + Application
- WebApi depends on Application + Infrastructure
- SharedKernel is referenced by all layers

## Project Organization

### Domain Layer (`Domain/`)
**Purpose:** Core business logic and entities
**Rules:**
- Rich domain models with private setters
- Use factory methods for entity creation
- No external dependencies
- Aggregate roots manage consistency boundaries

**Example Structure:**
```csharp
public class Student
{
    public Guid Id { get; private set; }
    public double Grade { get; private set; }
    public User User { get; private set; }
    
    private Student(User user) { /* constructor logic */ }
    public static Result<Student> Create(User user) => /* validation + creation */;
}
```

### Application Layer (`Application/`)
**Purpose:** Use cases and business workflows
**Patterns:** CQRS with MediatR
**Rules:**
- Feature-based organization (Home/Query/, Home/Command/)
- Each feature has Request/Response DTOs
- Handlers implement IRequestHandler<TRequest, TResponse>
- Use AutoMapper for object mapping
- FluentValidation for input validation

**Handler Template:**
```csharp
public sealed record GetUserInfoQuery : IRequest<Result<GetUserInfoResponse>>
{
    public Guid UserId { get; init; }
}

public class GetUserInfoQueryHandler : IRequestHandler<GetUserInfoQuery, Result<GetUserInfoResponse>>
{
    // Implementation with proper error handling using Result pattern
}
```

### Infrastructure Layer (`Infrastructure/`)
**Purpose:** Data persistence and external services
**Technologies:** Entity Framework Core + PostgreSQL
**Rules:**
- DbContext in Infrastructure/Data/
- Repository pattern through EF Core
- Configuration classes for entities
- No business logic

### WebApi Layer (`WebApi/`)
**Purpose:** REST API endpoints and presentation using Minimal APIs
**Organization:** Feature-based endpoint groups
**Rules:**
- Use Minimal APIs with MapGroup for organization
- Delegate to MediatR handlers
- Use TypedResults for better testability and OpenAPI metadata
- API versioning with endpoint groups
- Swagger documentation

### SharedKernel (`SharedKernel/`)
**Purpose:** Common domain primitives
**Contains:**
- Base domain classes
- Value objects
- Helper utilities
- Extension methods

## Development Guidelines

### Adding New Features

1. **Domain First:**
   - Create aggregate root in Domain/{FeatureName}/
   - Define entity with private setters
   - Add factory methods returning Results

2. **Application Layer:**
   - Create feature folder: Application/{FeatureName}/
   - Add Query/ and Command/ subfolders
   - Implement MediatR handlers returning Results
   - Create DTOs and AutoMapper profiles

3. **Infrastructure:**
   - Add EF Core entity configuration
   - Update ApplicationContext if needed

4. **WebApi (Minimal APIs):**
   - Create endpoint group using MapGroup
   - Define endpoint handlers that delegate to MediatR
   - Use TypedResults for responses
   - Apply versioning and authentication as needed

### Code Standards

**Entity Design:**
```csharp
public class EntityName
{
    public Guid Id { get; private set; }
    public string Property { get; private set; }
    
    private EntityName() { } // EF Core constructor
    private EntityName(params) { /* logic */ }
    
    public static Result<EntityName> Create(params) => /* validation + creation */;
    public Result UpdateProperty(value) => /* validation + update */;
}
```

**Minimal API Endpoint Pattern:**
```csharp
// Program.cs or separate endpoint configuration
var app = builder.Build();

// Group related endpoints
var userEndpoints = app.MapGroup("/api/v1/users")
    .WithTags("Users")
    .WithOpenApi();

// Define endpoints with handler methods
userEndpoints.MapGet("/{id}", GetUser);
userEndpoints.MapPost("/", CreateUser);
userEndpoints.MapPut("/{id}", UpdateUser);
userEndpoints.MapDelete("/{id}", DeleteUser);

// Endpoint handler methods
static async Task<IResult> GetUser(Guid id, IMediator mediator)
{
    var query = new GetUserQuery { Id = id };
    var result = await mediator.Send(query);
    
    return result.IsSuccess 
        ? TypedResults.Ok(result.Value) 
        : TypedResults.BadRequest(new { Error = result.Error });
}

static async Task<IResult> CreateUser(CreateUserRequest request, IMediator mediator)
{
    var command = new CreateUserCommand { /* map from request */ };
    var result = await mediator.Send(command);
    
    return result.IsSuccess
        ? TypedResults.Created($"/api/v1/users/{result.Value.Id}", result.Value)
        : TypedResults.BadRequest(new { Error = result.Error });
}
```

**Advanced Endpoint Organization:**
```csharp
// Feature-based endpoint extensions
public static class UserEndpoints
{
    public static RouteGroupBuilder MapUserEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/{id}", GetUser)
            .WithName("GetUser")
            .WithSummary("Get user by ID")
            .Produces<GetUserResponse>()
            .ProducesProblem(404);
            
        group.MapPost("/", CreateUser)
            .WithName("CreateUser")
            .WithSummary("Create a new user")
            .Accepts<CreateUserRequest>("application/json")
            .Produces<CreateUserResponse>(201)
            .ProducesValidationProblem();
            
        return group;
    }
    
    private static async Task<IResult> GetUser(Guid id, IMediator mediator)
    {
        // Implementation
    }
    
    private static async Task<IResult> CreateUser(CreateUserRequest request, IMediator mediator)
    {
        // Implementation
    }
}

// Usage in Program.cs
app.MapGroup("/api/v1/users")
   .WithTags("Users")
   .MapUserEndpoints();
```

## Key Technologies & Patterns

**Core Stack:**
- .NET 9 with nullable reference types
- ASP.NET Core Minimal APIs
- Entity Framework Core 9.0
- PostgreSQL (Npgsql provider)

**Architecture Patterns:**
- MediatR 12.0+ for CQRS
- AutoMapper 12.0+ for mapping

**Infrastructure:**
- Serilog for structured logging
- Swagger/OpenAPI documentation with TypedResults
- Health checks and monitoring
- API versioning with endpoint groups

**Minimal API Features (.NET 9):**
- MapGroup for endpoint organization
- TypedResults for better testability and OpenAPI metadata
- Built-in parameter binding and validation
- Endpoint filters for cross-cutting concerns
- Rate limiting and authentication integration

## Result Pattern Implementation

### Core Result Classes
Located in `SharedKernel/Domain/Result.cs`:
```csharp
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string Error { get; }
    
    public static Result Success() => new Result(true, null);
    public static Result Failure(string error) => new Result(false, error);
    public static Result<T> Success<T>(T value) => new Result<T>(value, true, null);
    public static Result<T> Failure<T>(string error) => new Result<T>(default!, false, error);
}

public class Result<T> : Result
{
    public T Value { get; }
    public static implicit operator Result<T>(T value) => Success(value);
}
```

### Usage Patterns

**Domain Operations:**
```csharp
public static Result<Student> Create(User user)
{
    if (user == null)
        return Result.Failure<Student>("User cannot be null");
    return Result.Success(new Student(user));
}
```

**Application Handlers:**
```csharp
public async Task<Result<GetUserInfoResponse>> Handle(GetUserInfoQuery request, CancellationToken token)
{
    if (request.UserId.IsDefault())
        return Result.Failure<GetUserInfoResponse>("UserId cannot be empty");
    
    var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.UserId, token);
    if (user is null)
        return Result.Failure<GetUserInfoResponse>("User not found");
    
    return Result.Success(_mapper.Map<GetUserInfoResponse>(user));
}
```

**Minimal API Responses:**
```csharp
static async Task<IResult> GetUser(Guid id, IMediator mediator)
{
    var query = new GetUserQuery { Id = id };
    var result = await mediator.Send(query);
    
    return result.IsSuccess 
        ? TypedResults.Ok(result.Value) 
        : TypedResults.BadRequest(new { Error = result.Error });
}
```

### Extension Methods
Available in `SharedKernel/Extensions/ResultExtensions.cs`:
- `Map<T, K>()` - Transform successful results
- `Bind<T, K>()` - Chain Result-returning operations
- `Ensure<T>()` - Add validation conditions
- `OnSuccess<T>()` / `OnFailure<T>()` - Side effects

### Benefits

1. **Explicit Error Handling** - Forces developers to handle both success and failure cases
2. **No Hidden Exceptions** - All possible failures are visible in method signatures
3. **Composable Operations** - Chain operations with extension methods
4. **Better Testing** - Easy to test both success and failure scenarios
5. **Performance** - Avoids exception throwing overhead

### Best Practices

1. **Use Result for Expected Failures** - Business rule violations, validation errors
2. **Keep Exceptions for Unexpected Failures** - System errors, infrastructure issues
3. **Make Error Messages User-Friendly** - Clear, actionable error descriptions
4. **Chain Operations** - Use Map and Bind for fluent operation chaining
5. **Consistent Return Types** - Always return Result from domain operations
6. **Use TypedResults in Minimal APIs** - Better testability and OpenAPI documentation

## Minimal API Best Practices (.NET 9)

### Endpoint Organization
- Use `MapGroup` to organize related endpoints
- Apply common configuration (authentication, versioning, tags) at the group level
- Separate endpoint definitions into extension methods for better organization

### Response Types
- Prefer `TypedResults` over `Results` for better compile-time safety
- Use appropriate HTTP status codes with TypedResults methods
- Include OpenAPI metadata with `WithSummary`, `WithDescription`, `Produces`, etc.

### Parameter Binding
- Leverage automatic parameter binding for simple types
- Use `[FromBody]`, `[FromRoute]`, `[FromQuery]` attributes when needed
- Validate input using FluentValidation or built-in validation attributes

### Error Handling
- Use Result pattern for business logic errors
- Return appropriate HTTP status codes based on error types
- Provide consistent error response format across all endpoints

### Performance
- Use `IAsyncEnumerable<T>` for streaming responses when appropriate
- Implement proper cancellation token support
- Consider endpoint filters for cross-cutting concerns instead of middleware when possible

This architecture promotes maintainable, testable, and scalable applications through clear separation of concerns and established patterns, now leveraging the power and simplicity of .NET 9 Minimal APIs.
