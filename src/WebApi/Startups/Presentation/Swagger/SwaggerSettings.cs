using System.ComponentModel.DataAnnotations;

namespace WebApi.Startups.Presentation.Swagger;

public sealed record SwaggerSettings
{
    [Required]
    public string ApiName { get; init; }
    [Required]
    public bool UseSwagger { get; init; }
}
