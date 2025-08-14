---
trigger: always_on
description: 
globs: 
---

# AI Architecture Guide: Vertical Slice Clean Architecture

## Overview
This codebase implements **Clean Architecture** with **Vertical Slice** organization using .NET 6, focusing on maintainability, testability, and feature-based development.

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
**Purpose:** REST API endpoints and presentation
**Organization:** Feature-based controllers
**Rules:**
- Minimal controllers (delegate to MediatR)
- Use API versioning
- Swagger documentation
- Modular startup configuration

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

4. **WebApi:**
   - Create feature controller
   - Use [ApiVersion] attributes
   - Delegate to MediatR handlers

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

**Controller Pattern:**
```csharp
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class FeatureController : ControllerBase
{
    private readonly IMediator _mediator;
    
    [HttpGet("{id}")]
    public async Task<ActionResult<ResponseDto>> Get(Guid id)
    {
        var query = new GetQuery { Id = id };
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { Error = result.Error });
    }
}
```

## Key Technologies & Patterns

**Core Stack:**
- .NET 6 with nullable reference types
- ASP.NET Core Web API
- Entity Framework Core 6.0.3
- PostgreSQL (Npgsql provider)

**Architecture Patterns:**
- MediatR 10.0.1 for CQRS
- AutoMapper 11.0.1 for mapping
- FluentValidation 10.4.0 for validation

**Infrastructure:**
- Serilog for structured logging
- Swagger/OpenAPI documentation
- Health checks and monitoring
- API versioning support

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

**Controller Responses:**
```csharp
var result = await _mediator.Send(query);
return result.IsSuccess ? Ok(result.Value) : BadRequest(new { Error = result.Error });
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

This architecture promotes maintainable, testable, and scalable applications through clear separation of concerns and established patterns.
