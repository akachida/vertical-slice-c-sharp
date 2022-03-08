namespace WebApi.Startups.Infrastructure.Persistence.Settings;

internal sealed record ConnectionStrings
{
    public string PostgreSql { get; init; }

    // Can be separated to Commands and Queries configurations instead of DB type
    // public CommandsSettings CommandsSettings { get; init; }
    // public QuerySettings QueriesSettings { get; init; }
}
