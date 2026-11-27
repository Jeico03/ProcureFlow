
namespace ProcureFlow.Domain.Entities;

public enum RequestType
{
    Modification = 1,
    Cancellation = 2
}

public enum RequestStatus
{
    Pending = 1,
    Approved = 2,
    Rejected = 3
}

public class Request
{
    public int Id { get; private set; }

    public int PurchaseOrderId { get; private set; }

    public int RequestedByUserId { get; private set; }

    public int? ReviewedByUserId { get; private set; }

    public RequestType Type { get; private set; }

    public RequestStatus Status { get; private set; }

    public string Reason { get; private set; } = string.Empty;

    public string? ReviewComments { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? ReviewedAt { get; private set; }

    public PurchaseOrder PurchaseOrder { get; private set; } = null!;

    public User RequestedByUser { get; private set; } = null!;

    public User? ReviewedByUser { get; private set; }

    private Request()
    {
    }

    public Request(
        int purchaseOrderId,
        int requestedByUserId,
        RequestType type,
        string reason)
    {
        ValidateForeignKey(purchaseOrderId, nameof(purchaseOrderId));
        ValidateForeignKey(requestedByUserId, nameof(requestedByUserId));
        ValidateType(type);
        ValidateReason(reason);

        PurchaseOrderId = purchaseOrderId;
        RequestedByUserId = requestedByUserId;
        Type = type;
        Reason = reason.Trim();

        Status = RequestStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public void Approve(int reviewedByUserId, string? reviewComments = null)
    {
        EnsurePending();
        ValidateForeignKey(reviewedByUserId, nameof(reviewedByUserId));
        ValidateReviewComments(reviewComments);

        ReviewedByUserId = reviewedByUserId;
        ReviewComments = NormalizeOptionalValue(reviewComments);
        ReviewedAt = DateTime.UtcNow;

        Status = RequestStatus.Approved;
    }

    public void Reject(int reviewedByUserId, string reviewComments)
    {
        EnsurePending();
        ValidateForeignKey(reviewedByUserId, nameof(reviewedByUserId));
        ValidateReviewComments(reviewComments);

        if (string.IsNullOrWhiteSpace(reviewComments))
            throw new ArgumentException(
                "Review comments are required when rejecting a request.",
                nameof(reviewComments));

        ReviewedByUserId = reviewedByUserId;
        ReviewComments = reviewComments.Trim();
        ReviewedAt = DateTime.UtcNow;

        Status = RequestStatus.Rejected;
    }

    private void EnsurePending()
    {
        if (Status != RequestStatus.Pending)
            throw new InvalidOperationException(
                "Only pending requests can be reviewed.");
    }

    private static void ValidateForeignKey(int id, string parameterName)
    {
        if (id <= 0)
            throw new ArgumentException(
                "The identifier must be greater than zero.",
                parameterName);
    }

    private static void ValidateType(RequestType type)
    {
        if (!Enum.IsDefined(type))
            throw new ArgumentOutOfRangeException(
                nameof(type),
                "Invalid request type.");
    }

    private static void ValidateReason(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException(
                "Request reason is required.",
                nameof(reason));

        if (reason.Length > 1000)
            throw new ArgumentException(
                "Request reason cannot exceed 1000 characters.",
                nameof(reason));
    }

    private static void ValidateReviewComments(string? comments)
    {
        if (comments?.Length > 1000)
            throw new ArgumentException(
                "Review comments cannot exceed 1000 characters.",
                nameof(comments));
    }

    private static string? NormalizeOptionalValue(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
