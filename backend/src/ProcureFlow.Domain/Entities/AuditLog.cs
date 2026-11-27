
namespace ProcureFlow.Domain.Entities;

public class AuditLog
{
    public int Id { get; private set; }

    public int UserId { get; private set; }

    public string EntityType { get; private set; } = string.Empty;

    public int EntityId { get; private set; }

    public string Action { get; private set; } = string.Empty;

    public string? Details { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public User User { get; private set; } = null!;

    private AuditLog()
    {
    }

    public AuditLog(
        int userId,
        string entityType,
        int entityId,
        string action,
        string? details = null)
    {
        ValidateForeignKey(userId, nameof(userId));
        ValidateForeignKey(entityId, nameof(entityId));
        ValidateRequiredValue(entityType, nameof(entityType), 100);
        ValidateRequiredValue(action, nameof(action), 100);
        ValidateOptionalValue(details, nameof(details), 1000);

        UserId = userId;
        EntityType = entityType.Trim();
        EntityId = entityId;
        Action = action.Trim();
        Details = NormalizeOptionalValue(details);

        CreatedAt = DateTime.UtcNow;
    }

    private static void ValidateForeignKey(int id, string parameterName)
    {
        if (id <= 0)
            throw new ArgumentException(
                "The identifier must be greater than zero.",
                parameterName);
    }

    private static void ValidateRequiredValue(
        string value,
        string parameterName,
        int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(
                "Value is required.",
                parameterName);

        if (value.Trim().Length > maxLength)
            throw new ArgumentException(
                $"Value cannot exceed {maxLength} characters.",
                parameterName);
    }

    private static void ValidateOptionalValue(
        string? value,
        string parameterName,
        int maxLength)
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
