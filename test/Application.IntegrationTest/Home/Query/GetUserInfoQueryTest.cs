using Application.Home.Query;
using Application.IntegrationTest.Common;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SharedKernel.ValueObjects;
using Xunit;
using Xunit.Sdk;

namespace Application.IntegrationTest.Home.Query;

[Collection("IntegrationTestCollection")]
public sealed class GetUserInfoQueryTest
{
    private readonly IntegrationTestBase _testBase;

    public GetUserInfoQueryTest(IntegrationTestBase testBase)
    {
        _testBase = testBase;
    }

    [Fact]
    public async Task Handler_should_return_user_info_response()
    {
        // arrange
        var mapper = _testBase.ServiceProvider.GetService<IMapper>();

        if (_testBase.Context is null)
            throw new NullException(_testBase);

        if (mapper is null)
            throw new NullException(mapper);

        var handler = new GetUserInfoQueryHandler(_testBase.Context, mapper);
        var username = new Email("admin@test.com");
        var firstUser = await _testBase.Context.Users
            .FirstOrDefaultAsync(x => x.Username == username)
            .ConfigureAwait(false);

        if (firstUser is null)
            throw new NullException(firstUser);

        // act
        var query = new GetUserInfoQuery
        {
            UserId = firstUser.Id
        };
        var response = await handler.Handle(query, It.IsAny<CancellationToken>());

        // assert
        response.Should().NotBeNull()
            .And.BeAssignableTo<GetUserInfoResponse>();

        response.FirstName.Should()
            .NotBeNullOrEmpty()
            .And.Be(firstUser.FirstName);

        response.LastName.Should()
            .NotBeNullOrEmpty()
            .And.Be(firstUser.LastName);
    }

    [Fact]
    public async Task Throw_exception_for_query_with_empty_user_id()
    {
        // arrange
        var mapper = _testBase.ServiceProvider.GetService<IMapper>();

        if (_testBase.Context is null)
            throw new NullException(_testBase);

        if (mapper is null)
            throw new NullException(mapper);

        var handler = new GetUserInfoQueryHandler(_testBase.Context, mapper);

        // act & assert
        var query = new GetUserInfoQuery();

        await FluentActions.Invoking(async () => await handler.Handle(query, It.IsAny<CancellationToken>()))
            .Should()
            .ThrowAsync<Exception>()
            .WithMessage("UserId should not be null or empty");
    }

    [Fact]
    public async Task Throw_exception_for_user_not_found()
    {
        // arrange
        var mapper = _testBase.ServiceProvider.GetService<IMapper>();

        if (_testBase.Context is null)
            throw new NullException(_testBase);

        if (mapper is null)
            throw new NullException(mapper);

        var handler = new GetUserInfoQueryHandler(_testBase.Context, mapper);

        // act & assert
        var query = new GetUserInfoQuery{ UserId = Guid.NewGuid() };

        await FluentActions.Invoking(async () => await handler.Handle(query, It.IsAny<CancellationToken>()))
            .Should()
            .ThrowAsync<Exception>()
            .WithMessage("UserId not found on the system");
    }
}
