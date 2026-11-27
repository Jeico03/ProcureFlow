
namespace ProcureFlow.Domain.Entities;

public class Notification
{
    public int Id { get; private set; }

    public int UserId { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public string Message { get; private set; } = string.Empty;

    public string? RelatedEntityType { get; private set; }

    public int? RelatedEntityId { get; private set; }

    public bool IsRead { get; private set; }

    public DateTime? ReadAt { get; private set; }

    public bool IsDeleted { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? DeletedAt { get; private set; }

    public User User { get; private set; } = null!;

    private Notification()
    {
    }

    public Notification(
        int userId,
        string title,
        string message,
        string? relatedEntityType = null,
        int? relatedEntityId = null)
    {
        ValidateForeignKey(userId, nameof(userId));
        ValidateRequiredValue(title, nameof(title), 150);
        ValidateRequiredValue(message, nameof(message), 1000);
        ValidateRelatedEntity(relatedEntityType, relatedEntityId);

        UserId = userId;
        Title = title.Trim();
        Message = message.Trim();
        RelatedEntityType = NormalizeOptionalValue(relatedEntityType);
        RelatedEntityId = relatedEntityId;

        IsRead = false;
        IsDeleted = false;
        CreatedAt = DateTime.UtcNow;
    }

    public void MarkAsRead()
    {
        if (IsDeleted)
            throw new InvalidOperationException(
                "Deleted notifications cannot be marked as read.");

        if (IsRead)
            return;

        IsRead = true;
        ReadAt = DateTime.UtcNow;
    }

    public void MarkAsUnread()
    {
        if (IsDeleted)
            throw new InvalidOperationException(
                "Deleted notifications cannot be marked as unread.");

        if (!IsRead)
            return;

        IsRead = false;
        ReadAt = null;
    }

    public void Delete()
    {
        if (IsDeleted)
            throw new InvalidOperationException(
                "Notification is already deleted.");

        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
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

    private static void ValidateRelatedEntity(
        string? entityType,
        int? entityId)
    {
        if (string.IsNullOrWhiteSpace(entityType) && entityId is null)
            return;

        if (string.IsNullOrWhiteSpace(entityType) || entityId is null)
            throw new ArgumentException(
                "Related entity type and identifier must be provided together.");

        if (entityId <= 0)
            throw new ArgumentException(
                "Related entity identifier must be greater than zero.",
                nameof(entityId));

        if (entityType.Trim().Length > 100)
            throw new ArgumentException(
                "Related entity type cannot exceed 100 characters.",
                nameof(entityType));
    }

    private static string? NormalizeOptionalValue(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
