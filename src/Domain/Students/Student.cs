using Domain.Lectures;
using Domain.Professors;
using Domain.Users;
using SharedKernel.Domain;

namespace Domain.Students;

public class Student
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public double Grade { get; private set; }

    public List<Lecture>? Lectures { get; private set; }
    public List<Professor>? Professors { get; private set; }
    public User User { get; private set; } = null!;

    // Parameterless constructor for EF Core
    private Student()
    {
    }

    private Student(User user)
    {
        Id = Guid.NewGuid();
        Grade = 0;
        UserId = user.Id;
        User = user;
    }

    public static Result<Student> Create(User user)
    {
        if (user == null)
            return Result.Failure<Student>("User cannot be null");

        return Result.Success(new Student(user));
    }

    public Result UpdateGrade(double newGrade)
    {
        if (newGrade < 0 || newGrade > 100)
            return Result.Failure("Grade must be between 0 and 100");

        Grade = newGrade;
        return Result.Success();
    }

    public Result AddLecture(Lecture lecture)
    {
        if (lecture == null)
            return Result.Failure("Lecture cannot be null");

        Lectures ??= new List<Lecture>();
        
        if (Lectures.Any(l => l.Id == lecture.Id))
            return Result.Failure("Student is already enrolled in this lecture");

        Lectures.Add(lecture);
        return Result.Success();
    }

    public Result RemoveLecture(Guid lectureId)
    {
        if (Lectures == null || !Lectures.Any())
            return Result.Failure("Student has no lectures to remove");

        var lecture = Lectures.FirstOrDefault(l => l.Id == lectureId);
        if (lecture == null)
            return Result.Failure("Student is not enrolled in this lecture");

        Lectures.Remove(lecture);
        return Result.Success();
    }
}
