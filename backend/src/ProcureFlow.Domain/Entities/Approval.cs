
namespace ProcureFlow.Domain.Entities;

public enum ApprovalDecision
{
    Approved = 1,
    Rejected = 2
}

public class Approval
{
    public int Id { get; private set; }

    public int PurchaseOrderId { get; private set; }

    public int ApproverUserId { get; private set; }

    public ApprovalDecision Decision { get; private set; }

    public string? Comments { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public PurchaseOrder PurchaseOrder { get; private set; } = null!;

    public User ApproverUser { get; private set; } = null!;

    private Approval()
    {
    }

    public Approval(
        int purchaseOrderId,
        int approverUserId,
        ApprovalDecision decision,
        string? comments = null)
    {
        ValidateForeignKey(purchaseOrderId, nameof(purchaseOrderId));
        ValidateForeignKey(approverUserId, nameof(approverUserId));
        ValidateDecision(decision);
        ValidateComments(comments);

        PurchaseOrderId = purchaseOrderId;
        ApproverUserId = approverUserId;
        Decision = decision;
        Comments = NormalizeOptionalValue(comments);

        CreatedAt = DateTime.UtcNow;
    }

    private static void ValidateForeignKey(int id, string parameterName)
    {
        if (id <= 0)
            throw new ArgumentException(
                "The identifier must be greater than zero.",
                parameterName);
    }

    private static void ValidateDecision(ApprovalDecision decision)
    {
        if (!Enum.IsDefined(decision))
            throw new ArgumentOutOfRangeException(
                nameof(decision),
                "Invalid approval decision.");
    }

    private static void ValidateComments(string? comments)
    {
        if (comments?.Length > 1000)
            throw new ArgumentException(
                "Comments cannot exceed 1000 characters.",
                nameof(comments));
    }

    private static string? NormalizeOptionalValue(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
