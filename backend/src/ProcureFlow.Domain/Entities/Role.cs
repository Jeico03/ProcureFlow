
namespace ProcureFlow.Domain.Entities;

public class Role
{
    public int Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public DateTime CreatedAt { get; private set; }

    private Role()
    {
    }

    public Role(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name is required.", nameof(name));

        if (name.Length > 50)
            throw new ArgumentException("Role name cannot exceed 50 characters.", nameof(name));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Role description is required.", nameof(description));

        if (description.Length > 250)
            throw new ArgumentException("Role description cannot exceed 250 characters.", nameof(description));

        Name = name.Trim();
        Description = description.Trim();
        CreatedAt = DateTime.UtcNow;
    }
}
