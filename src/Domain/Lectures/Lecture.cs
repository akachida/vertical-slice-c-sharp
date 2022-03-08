using Domain.Professors;
using SharedKernel.Domain.Entities;

namespace Domain.Lectures;

public class Lecture : EntityBase
{
    public string Name { get; private set; }
    public List<Professor> Professors { get; set; }

    private Lecture()
    {
    }

    private Lecture(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public Lecture Create(string name)
    {
        return new Lecture(name);
    }
}
