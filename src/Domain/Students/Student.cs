using Domain.Lectures;
using Domain.Professors;
using Domain.Users;

namespace Domain.Students;

public class Student
{
    public Guid Id { get; private set; }
    public double Grade { get; private set; }

    public List<Lecture>? Lectures { get; private set; }
    public List<Professor>? Professors { get; private set; }
    public User User { get; private set; }

    private Student(User user)
    {
        Id = Guid.NewGuid();
        Grade = 0;
        User = user;
    }

    public Student Create(User user)
    {
        return new Student(user);
    }
}
