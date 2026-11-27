
namespace ProcureFlow.Domain.Entities;

public class Supplier
{
    public int Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string? TaxId { get; private set; }

    public string? ContactName { get; private set; }

    public string Email { get; private set; } = string.Empty;

    public string? Phone { get; private set; }

    public string? Address { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    private Supplier()
    {
    }

    public Supplier(
        string name,
        string email,
        string? taxId = null,
        string? contactName = null,
        string? phone = null,
        string? address = null)
    {
        ValidateName(name);
        ValidateEmail(email);
        ValidateOptionalLength(taxId, 50, nameof(taxId));
        ValidateOptionalLength(contactName, 150, nameof(contactName));
        ValidateOptionalLength(phone, 30, nameof(phone));
        ValidateOptionalLength(address, 300, nameof(address));

        Name = name.Trim();
        Email = email.Trim();
        TaxId = NormalizeOptionalValue(taxId);
        ContactName = NormalizeOptionalValue(contactName);
        Phone = NormalizeOptionalValue(phone);
        Address = NormalizeOptionalValue(address);

        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public void UpdateDetails(
        string name,
        string email,
        string? taxId,
        string? contactName,
        string? phone,
        string? address)
    {
        ValidateName(name);
        ValidateEmail(email);
        ValidateOptionalLength(taxId, 50, nameof(taxId));
        ValidateOptionalLength(contactName, 150, nameof(contactName));
        ValidateOptionalLength(phone, 30, nameof(phone));
        ValidateOptionalLength(address, 300, nameof(address));

        Name = name.Trim();
        Email = email.Trim();
        TaxId = NormalizeOptionalValue(taxId);
        ContactName = NormalizeOptionalValue(contactName);
        Phone = NormalizeOptionalValue(phone);
        Address = NormalizeOptionalValue(address);

        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw new InvalidOperationException("Supplier is already inactive.");

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        if (IsActive)
            throw new InvalidOperationException("Supplier is already active.");

        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Supplier name is required.", nameof(name));

        if (name.Length > 150)
            throw new ArgumentException("Supplier name cannot exceed 150 characters.", nameof(name));
    }

    private static void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Supplier email is required.", nameof(email));

        if (email.Length > 256)
            throw new ArgumentException("Supplier email cannot exceed 256 characters.", nameof(email));
    }

    private static void ValidateOptionalLength(
        string? value,
        int maxLength,
        string parameterName)
    {
        if (value?.Length > maxLength)
            throw new ArgumentException(
                $"Value cannot exceed {maxLength} characters.",
                parameterName);
    }

    private static string? NormalizeOptionalValue(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
