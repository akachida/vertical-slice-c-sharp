using Application.Home.Query;
using Application.IntegrationTest.Fixtures;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Application.IntegrationTest.Home.Query;

public class GetUserInfoQueryHandlerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly IMediator _mediator;

    public GetUserInfoQueryHandlerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _mediator = _factory.Services.GetRequiredService<IMediator>();
    }

    [Fact]
    public async Task Handle_Should_ReturnUserInfoResponse_When_UserExists()
    {
        // Arrange
        var query = GetUserInfoQueryFixture.ARequestWithSpecificUserId(Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6")); // Use a known test user ID

        // Act
        var result = await _mediator.Send(query);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.FirstName.Should().NotBeNullOrEmpty();
        result.Value.LastName.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UserIdIsEmpty()
    {
        // Arrange
        var query = GetUserInfoQueryFixture.ARequestWithEmptyUserId();

        // Act
        var result = await _mediator.Send(query);

        // Assert
        result.Should().NotBeNull();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("UserId cannot be empty");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UserNotFound()
    {
        // Arrange
        var query = GetUserInfoQueryFixture.ARequestWithSpecificUserId(Guid.NewGuid()); // Random GUID that won't exist

        // Act
        var result = await _mediator.Send(query);

        // Assert
        result.Should().NotBeNull();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("User not found");
    }
}
