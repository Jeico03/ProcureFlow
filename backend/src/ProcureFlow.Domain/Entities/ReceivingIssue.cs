namespace ProcureFlow.Domain.Entities;

public enum ReceivingIssueType
{
    Missing = 1,
    Damaged = 2
}

public enum ReceivingIssueStatus
{
    Open = 1,
    Resolved = 2,
    Cancelled = 3
}

public class ReceivingIssue
{
    public int Id { get; private set; }

    public int PurchaseOrderId { get; private set; }

    public int PurchaseOrderItemId { get; private set; }

    public ReceivingIssueType Type { get; private set; }

    public ReceivingIssueStatus Status { get; private set; }

    public decimal Quantity { get; private set; }

    public string Description { get; private set; } = string.Empty;

    public int ReportedByUserId { get; private set; }

    public int? ResolvedByUserId { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? ResolvedAt { get; private set; }

    public PurchaseOrder PurchaseOrder { get; private set; } = null!;

    public PurchaseOrderItem PurchaseOrderItem { get; private set; } = null!;

    public User ReportedByUser { get; private set; } = null!;

    public User? ResolvedByUser { get; private set; }

    private ReceivingIssue()
    {
    }

    public ReceivingIssue(
        int purchaseOrderId,
        int purchaseOrderItemId,
        ReceivingIssueType type,
        decimal quantity,
        string description,
        int reportedByUserId)
    {
        ValidateForeignKey(purchaseOrderId, nameof(purchaseOrderId));
        ValidateForeignKey(purchaseOrderItemId, nameof(purchaseOrderItemId));
        ValidateForeignKey(reportedByUserId, nameof(reportedByUserId));
        ValidateType(type);
        ValidateQuantity(quantity);
        ValidateDescription(description);

        PurchaseOrderId = purchaseOrderId;
        PurchaseOrderItemId = purchaseOrderItemId;
        Type = type;
        Quantity = quantity;
        Description = description.Trim();
        ReportedByUserId = reportedByUserId;

        Status = ReceivingIssueStatus.Open;
        CreatedAt = DateTime.UtcNow;
    }

    public void Resolve(int resolvedByUserId)
    {
        if (Status != ReceivingIssueStatus.Open)
            throw new InvalidOperationException(
                "Only open receiving issues can be resolved.");

        ValidateForeignKey(resolvedByUserId, nameof(resolvedByUserId));

        ResolvedByUserId = resolvedByUserId;
        ResolvedAt = DateTime.UtcNow;
        Status = ReceivingIssueStatus.Resolved;
    }

    public void Cancel()
    {
        if (Status != ReceivingIssueStatus.Open)
            throw new InvalidOperationException(
                "Only open receiving issues can be cancelled.");

        Status = ReceivingIssueStatus.Cancelled;
    }

    private static void ValidateForeignKey(int id, string parameterName)
    {
        if (id <= 0)
            throw new ArgumentException(
                "The identifier must be greater than zero.",
                parameterName);
    }

    private static void ValidateType(ReceivingIssueType type)
    {
        if (!Enum.IsDefined(type))
            throw new ArgumentOutOfRangeException(
                nameof(type),
                "Invalid receiving issue type.");
    }

    private static void ValidateQuantity(decimal quantity)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                "Issue quantity must be greater than zero.");

        if (decimal.Round(quantity, 3) != quantity)
            throw new ArgumentException(
                "Issue quantity cannot have more than three decimal places.",
                nameof(quantity));
    }

    private static void ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException(
                "Issue description is required.",
                nameof(description));

        if (description.Length > 1000)
            throw new ArgumentException(
                "Issue description cannot exceed 1000 characters.",
                nameof(description));
    }
}
