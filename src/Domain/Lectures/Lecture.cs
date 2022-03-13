using Domain.Professors;

namespace Domain.Lectures;

public class Lecture
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public List<Professor>? Professors { get; set; }

    private Lecture(string name)
    {
        Id = Guid.NewGuid();
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public Lecture Create(string name)
    {
        return new Lecture(name);
    }
}
