using System.ComponentModel.DataAnnotations;

namespace WebApi.Extensions;

internal static class ConfigurationExtension
{
    /// <summary>
    /// Binds a section of the configuration as a strongly typed configuration instance.
    /// </summary>
    /// <typeparam name="T">The options type to bind. The name of this type is used as the section name as well.</typeparam>
    public static T GetMyOptions<T> (this IConfiguration configuration, bool required = false) where T : class
    {
        var bound = configuration.GetSection(typeof(T).Name).Get<T>();

        if (bound != null)
            Validator.ValidateObject(bound, new ValidationContext(bound), validateAllProperties: true);
        else if (required)
            throw new InvalidOperationException($"Settings type of '{nameof(T)}' was requested as required, but was not found in configuration.");

        return bound;
    }

    public static void RegisterMyOptions<T>(this IServiceCollection services) where T : class
    {
        // TODO: Note that validation is late when resolved through resolver delegate. :/ Maybe ask for a configuration too, and bind it eagerly.
        services.AddSingleton<T>(resolver =>
            resolver.GetRequiredService<IConfiguration>().GetMyOptions<T>());
    }
}
