using Domain.Lectures;
using Domain.Professors;
using Domain.Users;
using SharedKernel.Domain.Entities;

namespace Domain.Students;

public class Student : EntityBase
{
    public double Grade { get; private set; }

    public List<Lecture> Lectures { get; private set; }
    public List<Professor> Professors { get; private set; }
    public User User { get; private set; }

    private Student()
    {
    }

    private Student(User user)
    {
        Grade = 0;
        User = user;
    }

    public Student Create(User user)
    {
        return new Student(user);
    }
}
