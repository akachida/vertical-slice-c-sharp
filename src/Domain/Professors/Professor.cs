using Domain.Lectures;
using Domain.Students;
using Domain.Users;

namespace Domain.Professors;

public class Professor
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public List<Student>? Students { get; private set; }
    public List<Lecture>? Lectures { get; private set; }
    public User User { get; private set; } = null!;

    // Parameterless constructor for EF Core
    private Professor()
    {
    }

    private Professor(Guid userId)
    {
        Id = Guid.NewGuid();
        UserId = userId;
    }

    public static Professor Create(User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));
            
        return new Professor(user.Id);
    }
}
