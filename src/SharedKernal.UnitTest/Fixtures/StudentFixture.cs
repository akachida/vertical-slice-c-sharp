using Bogus;
using Domain.Students;
using Domain.Users;
using SharedKernal.UnitTest.Fixtures;

namespace SharedKernal.UnitTest.Fixtures;

public class StudentFixture
{
    private static readonly Faker Faker = new();

    private User _user;
    private double _grade;

    // Private constructor sets realistic defaults
    private StudentFixture()
    {
        _user = UserFixture.AStudentUser();
        _grade = Faker.Random.Double(0, 100);
    }

    // Static factory to get a builder instance
    public static StudentFixture AStudent() => new();

    // --- Object Mother Methods (for common scenarios) ---
    public static Student ADefaultStudent() => AStudent().Build();

    public static Student AStudentWithGrade(double grade) => AStudent()
        .WithGrade(grade)
        .Build();

    public static Student AStudentWithZeroGrade() => AStudent()
        .WithGrade(0)
        .Build();

    public static Student AStudentWithMaxGrade() => AStudent()
        .WithGrade(100)
        .Build();

    // --- Builder Methods (for customization) ---
    public StudentFixture WithUser(User user)
    {
        _user = user;
        return this;
    }

    public StudentFixture WithGrade(double grade)
    {
        _grade = grade;
        return this;
    }

    public Student Build()
    {
        var student = Student.Create(_user).Value;
        if (_grade > 0)
        {
            student.UpdateGrade(_grade);
        }
        return student;
    }
}
