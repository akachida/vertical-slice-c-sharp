using Domain.Lectures;
using Domain.Students;
using Domain.Users;

namespace Domain.Professors;

public class Professor
{
    public Guid Id { get; private set; }
    public List<Student>? Students { get; private set; }
    public List<Lecture>? Lectures { get; private set; }
    public User User { get; private set; }

    private Professor(User user)
    {
        Id = Guid.NewGuid();
        User = user ?? throw new ArgumentNullException(nameof(user));
    }

    public Professor Create(User user)
    {
        return new Professor(user);
    }
}
