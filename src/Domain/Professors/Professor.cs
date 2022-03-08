using Domain.Lectures;
using Domain.Students;
using Domain.Users;
using SharedKernel.Domain.Entities;

namespace Domain.Professors;

public class Professor : EntityBase
{
    public List<Student> Students { get; private set; }
    public List<Lecture> Lectures { get; private set; }
    public User User { get; private set; }

    private Professor()
    {
    }

    private Professor(User user)
    {
        User = user ?? throw new ArgumentNullException(nameof(user));
    }

    public Professor Create(User user)
    {
        return new Professor(user);
    }
}
