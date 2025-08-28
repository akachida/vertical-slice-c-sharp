using SharedKernel.Domain.Enumerations.Users;
using SharedKernel.ValueObjects;

namespace Domain.Users;

public class User
{
    public Guid Id { get; private set; }
    public Email Username { get; private set; }
    public string Password { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public UserType Type { get; private set; }
    public UserLevel Level { get; private set; }

    private User(Email username, string password, string firstName, string lastName, UserType type,
        UserLevel level)
    {
        Id = Guid.NewGuid();
        Username = username;
        Password = password;
        FirstName = firstName;
        LastName = lastName;
        Type = type;
        Level = level;
    }

    public static User Create(string username, string password, string firstName, string lastName,
        UserType type, UserLevel level)
    {
        if (string.IsNullOrEmpty(username))
            throw new Exception("Username should not be null");

        if (string.IsNullOrEmpty(password))
            throw new Exception("Password should not be null");

        if (string.IsNullOrEmpty(firstName))
            throw new Exception("First Name should not be null");

        if (string.IsNullOrEmpty(lastName))
            throw new Exception("Last Name should not be null");

        var usernameEmail = new Email(username);

        return new User(usernameEmail, password, firstName, lastName, type, level);
    }
}
