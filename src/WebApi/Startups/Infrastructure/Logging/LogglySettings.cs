using System.ComponentModel.DataAnnotations;

namespace WebApi.Startups.Infrastructure.Logging;

internal sealed record LogglySettings
{
    [Required]
    public bool? WriteToLoggly { get; init; }
    public string CustomerToken { get; init; }
}
