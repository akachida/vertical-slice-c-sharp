using Bogus;
using Domain.Users;
using SharedKernel.Domain.Enumerations.Users;

namespace SharedKernal.UnitTest.Fixtures;

public class UserFixture
{
    private static readonly Faker Faker = new();

    private string _email;
    private string _password;
    private string _firstName;
    private string _lastName;
    private UserType _userType;
    private UserLevel _userLevel;

    // Private constructor sets realistic defaults
    private UserFixture()
    {
        _email = Faker.Internet.Email();
        _password = Faker.Internet.Password(8);
        _firstName = Faker.Name.FirstName();
        _lastName = Faker.Name.LastName();
        _userType = UserType.Student;
        _userLevel = UserLevel.Student;
    }

    // Static factory to get a builder instance
    public static UserFixture AUser() => new();

    // --- Object Mother Methods (for common scenarios) ---
    public static User AStudentUser() => AUser()
        .WithUserType(UserType.Student)
        .WithUserLevel(UserLevel.Student)
        .Build();

    public static User AProfessorUser() => AUser()
        .WithUserType(UserType.Professor)
        .WithUserLevel(UserLevel.Professor)
        .Build();

    public static User ADefaultUser() => AUser().Build();

    // --- Builder Methods (for customization) ---
    public UserFixture WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public UserFixture WithPassword(string password)
    {
        _password = password;
        return this;
    }

    public UserFixture WithFirstName(string firstName)
    {
        _firstName = firstName;
        return this;
    }

    public UserFixture WithLastName(string lastName)
    {
        _lastName = lastName;
        return this;
    }

    public UserFixture WithUserType(UserType userType)
    {
        _userType = userType;
        return this;
    }

    public UserFixture WithUserLevel(UserLevel userLevel)
    {
        _userLevel = userLevel;
        return this;
    }

    public User Build()
    {
        return User.Create(
            _email,
            _password,
            _firstName,
            _lastName,
            _userType,
            _userLevel);
    }
}
