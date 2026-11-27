
namespace ProcureFlow.Domain.Entities;

public class User
{
    public int Id { get; private set; }

    public string FirstName { get; private set; } = string.Empty;

    public string LastName { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public int RoleId { get; private set; }

    public int DepartmentId { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public Role Role { get; private set; } = null!;

    public Department Department { get; private set; } = null!;

    private User()
    {
    }

    public User(
        string firstName,
        string lastName,
        string email,
        int roleId,
        int departmentId)
    {
        ValidateName(firstName, nameof(firstName));
        ValidateName(lastName, nameof(lastName));
        ValidateEmail(email);
        ValidateForeignKey(roleId, nameof(roleId));
        ValidateForeignKey(departmentId, nameof(departmentId));

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = email.Trim();

        RoleId = roleId;
        DepartmentId = departmentId;

        IsActive = true;

        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public void UpdateProfile(
        string firstName,
        string lastName,
        string email)
    {
        ValidateName(firstName, nameof(firstName));
        ValidateName(lastName, nameof(lastName));
        ValidateEmail(email);

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = email.Trim();

        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeRole(int roleId)
    {
        ValidateForeignKey(roleId, nameof(roleId));

        RoleId = roleId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeDepartment(int departmentId)
    {
        ValidateForeignKey(departmentId, nameof(departmentId));

        DepartmentId = departmentId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw new InvalidOperationException("User is already inactive.");

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        if (IsActive)
            throw new InvalidOperationException("User is already active.");

        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidateName(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Name is required.", parameterName);

        if (value.Length > 100)
            throw new ArgumentException("Name cannot exceed 100 characters.", parameterName);
    }

    private static void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));

        if (email.Length > 256)
            throw new ArgumentException("Email cannot exceed 256 characters.", nameof(email));
    }

    private static void ValidateForeignKey(int id, string parameterName)
    {
        if (id <= 0)
            throw new ArgumentException("The identifier must be greater than zero.", parameterName);
    }
}
