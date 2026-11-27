
namespace ProcureFlow.Domain.Entities;

public class Category
{
    public int Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    private Category()
    {
    }

    public Category(string name, string description)
    {
        ValidateName(name);
        ValidateDescription(description);

        Name = name.Trim();
        Description = description.Trim();

        IsActive = true;

        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public void UpdateDetails(string name, string description)
    {
        ValidateName(name);
        ValidateDescription(description);

        Name = name.Trim();
        Description = description.Trim();

        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw new InvalidOperationException("Category is already inactive.");

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        if (IsActive)
            throw new InvalidOperationException("Category is already active.");

        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name is required.", nameof(name));

        if (name.Length > 100)
            throw new ArgumentException("Category name cannot exceed 100 characters.", nameof(name));
    }

    private static void ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Category description is required.", nameof(description));

        if (description.Length > 250)
            throw new ArgumentException("Category description cannot exceed 250 characters.", nameof(description));
    }
}
