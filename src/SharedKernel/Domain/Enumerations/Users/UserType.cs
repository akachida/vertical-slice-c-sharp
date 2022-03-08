using SharedKernel.ValueObjects.Enumerations;

namespace SharedKernel.Domain.Enumerations.Users;

public class UserType : Enumeration<UserType>
{
    public static UserType Professor = new (1, "professor");
    public static UserType Student = new (2, "student");

    public UserType(int id, string name) : base(id, name)
    {
    }
}
