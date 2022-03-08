using System.ComponentModel.DataAnnotations;

namespace WebApi.Startups.Infrastructure.Persistence.Settings;

internal sealed class DataSettings
{
    [Required]
    public bool? AutoMigrate { get; init; }

    [Required]
    public bool? AutoSeed { get; init; }
}
