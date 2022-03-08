using System.Net.Mail;

namespace SharedKernel.ValueObjects;

public record Email
{
    public string Value { get; init; }

    public Email(string value)
    {
        if (string.IsNullOrEmpty(value))
            throw new ArgumentNullException(nameof(value));

        var trimmedEmail = value.Trim();
        if (trimmedEmail.Length > 50)
            throw new Exception("Email should not have more than 50 characters");

        if (!IsValidEmail(trimmedEmail))
            throw new Exception("Email is not in a valida format");

        Value = trimmedEmail;
    }

    private static bool IsValidEmail(string value)
    {
        var trimmedEmail = value.Trim();

        if (trimmedEmail.EndsWith("."))
            return false;

        try
        {
            var addr = new MailAddress(trimmedEmail);
            return addr.Address == trimmedEmail;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
