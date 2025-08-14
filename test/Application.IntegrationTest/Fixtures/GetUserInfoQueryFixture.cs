using Application.Home.Query;
using Bogus;

namespace Application.IntegrationTest.Fixtures;

public class GetUserInfoQueryFixture
{
    private static readonly Faker Faker = new();

    private Guid _userId;

    // Private constructor sets realistic defaults
    private GetUserInfoQueryFixture()
    {
        _userId = Faker.Random.Guid();
    }

    // Static factory to get a builder instance
    public static GetUserInfoQueryFixture AGetUserInfoQuery() => new();

    // --- Object Mother Methods (for common scenarios) ---
    public static GetUserInfoQuery ADefaultRequest() => AGetUserInfoQuery().Build();

    public static GetUserInfoQuery ARequestWithEmptyUserId() => AGetUserInfoQuery()
        .WithUserId(Guid.Empty)
        .Build();

    public static GetUserInfoQuery ARequestWithSpecificUserId(Guid userId) => AGetUserInfoQuery()
        .WithUserId(userId)
        .Build();

    // --- Builder Methods (for customization) ---
    public GetUserInfoQueryFixture WithUserId(Guid userId)
    {
        _userId = userId;
        return this;
    }

    public GetUserInfoQuery Build()
    {
        return new GetUserInfoQuery
        {
            UserId = _userId
        };
    }
}
