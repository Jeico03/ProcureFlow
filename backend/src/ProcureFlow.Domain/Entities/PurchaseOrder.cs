
namespace ProcureFlow.Domain.Entities;

public enum PurchaseOrderStatus
{
    Draft = 1,
    PendingApproval = 2,
    Approved = 3,
    Rejected = 4,
    ReceivingIssue = 5,
    Received = 6,
    Cancelled = 7
}

public class PurchaseOrder
{
    public int Id { get; private set; }

    public string OrderNumber { get; private set; } = string.Empty;

    public int SupplierId { get; private set; }

    public int DepartmentId { get; private set; }

    public int CreatedByUserId { get; private set; }

    public int AssignedToUserId { get; private set; }

    public PurchaseOrderStatus Status { get; private set; }

    public string? Justification { get; private set; }

    public DateTime? ExpectedDeliveryDate { get; private set; }

    public decimal Subtotal { get; private set; }

    public decimal TaxAmount { get; private set; }

    public decimal TotalAmount { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public Supplier Supplier { get; private set; } = null!;

    public Department Department { get; private set; } = null!;

    public User CreatedByUser { get; private set; } = null!;

    public User AssignedToUser { get; private set; } = null!;

    public ICollection<PurchaseOrderItem> Items { get; private set; }
        = new List<PurchaseOrderItem>();

    private PurchaseOrder()
    {
    }

    public PurchaseOrder(
        string orderNumber,
        int supplierId,
        int departmentId,
        int createdByUserId,
        string? justification = null,
        DateTime? expectedDeliveryDate = null)
    {
        ValidateOrderNumber(orderNumber);
        ValidateForeignKey(supplierId, nameof(supplierId));
        ValidateForeignKey(departmentId, nameof(departmentId));
        ValidateForeignKey(createdByUserId, nameof(createdByUserId));
        ValidateOptionalDetails(justification, expectedDeliveryDate);

        OrderNumber = orderNumber.Trim();
        SupplierId = supplierId;
        DepartmentId = departmentId;

        CreatedByUserId = createdByUserId;
        AssignedToUserId = createdByUserId;

        Justification = NormalizeOptionalValue(justification);
        ExpectedDeliveryDate = expectedDeliveryDate;

        Status = PurchaseOrderStatus.Draft;

        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public void UpdateDraftDetails(
        int supplierId,
        int departmentId,
        string? justification,
        DateTime? expectedDeliveryDate)
    {
        EnsureEditable();

        ValidateForeignKey(supplierId, nameof(supplierId));
        ValidateForeignKey(departmentId, nameof(departmentId));
        ValidateOptionalDetails(justification, expectedDeliveryDate);

        SupplierId = supplierId;
        DepartmentId = departmentId;
        Justification = NormalizeOptionalValue(justification);
        ExpectedDeliveryDate = expectedDeliveryDate;

        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignResponsibility(int userId)
    {
        ValidateForeignKey(userId, nameof(userId));

        if (Status == PurchaseOrderStatus.Received ||
            Status == PurchaseOrderStatus.Cancelled)
        {
            throw new InvalidOperationException(
                "Cannot reassign a completed or cancelled purchase order.");
        }

        AssignedToUserId = userId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateTotals(
        decimal subtotal,
        decimal taxAmount,
        decimal totalAmount)
    {
        EnsureEditable();

        if (subtotal < 0 || taxAmount < 0 || totalAmount < 0)
            throw new ArgumentOutOfRangeException(
                nameof(totalAmount),
                "Purchase order amounts cannot be negative.");

        if (totalAmount != subtotal + taxAmount)
            throw new ArgumentException(
                "Total amount must equal subtotal plus tax amount.");

        Subtotal = subtotal;
        TaxAmount = taxAmount;
        TotalAmount = totalAmount;

        UpdatedAt = DateTime.UtcNow;
    }

    public void SubmitForApproval(int itemCount)
    {
        if (Status != PurchaseOrderStatus.Draft &&
            Status != PurchaseOrderStatus.Rejected)
        {
            throw new InvalidOperationException(
                "Only draft or rejected orders can be submitted.");
        }

        if (itemCount < 1)
            throw new InvalidOperationException(
                "A purchase order must contain at least one item.");

        if (string.IsNullOrWhiteSpace(Justification))
            throw new InvalidOperationException(
                "Justification is required before submission.");

        if (!ExpectedDeliveryDate.HasValue)
            throw new InvalidOperationException(
                "Expected delivery date is required before submission.");

        Status = PurchaseOrderStatus.PendingApproval;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Approve()
    {
        if (Status != PurchaseOrderStatus.PendingApproval)
            throw new InvalidOperationException(
                "Only pending orders can be approved.");

        Status = PurchaseOrderStatus.Approved;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reject()
    {
        if (Status != PurchaseOrderStatus.PendingApproval)
            throw new InvalidOperationException(
                "Only pending orders can be rejected.");

        Status = PurchaseOrderStatus.Rejected;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ReturnToDraftAfterModificationApproval()
    {
        if (Status != PurchaseOrderStatus.Approved)
            throw new InvalidOperationException(
                "Only approved orders can return to draft through a modification request.");

        Status = PurchaseOrderStatus.Draft;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkReceivingIssue()
    {
        if (Status != PurchaseOrderStatus.Approved &&
            Status != PurchaseOrderStatus.ReceivingIssue)
        {
            throw new InvalidOperationException(
                "Only approved orders can have receiving issues.");
        }

        Status = PurchaseOrderStatus.ReceivingIssue;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkReceived()
    {
        if (Status != PurchaseOrderStatus.Approved &&
            Status != PurchaseOrderStatus.ReceivingIssue)
        {
            throw new InvalidOperationException(
                "Only approved orders or orders with receiving issues can be received.");
        }

        Status = PurchaseOrderStatus.Received;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == PurchaseOrderStatus.Received ||
            Status == PurchaseOrderStatus.Cancelled)
        {
            throw new InvalidOperationException(
                "Received or already cancelled orders cannot be cancelled.");
        }

        Status = PurchaseOrderStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    private void EnsureEditable()
    {
        if (Status != PurchaseOrderStatus.Draft &&
            Status != PurchaseOrderStatus.Rejected)
        {
            throw new InvalidOperationException(
                "Only draft or rejected orders can be modified directly.");
        }
    }

    private static void ValidateOrderNumber(string orderNumber)
    {
        if (string.IsNullOrWhiteSpace(orderNumber))
            throw new ArgumentException(
                "Order number is required.", nameof(orderNumber));

        if (orderNumber.Length > 30)
            throw new ArgumentException(
                "Order number cannot exceed 30 characters.", nameof(orderNumber));
    }

    private static void ValidateForeignKey(int id, string parameterName)
    {
        if (id <= 0)
            throw new ArgumentException(
                "The identifier must be greater than zero.", parameterName);
    }

    private static void ValidateOptionalDetails(
        string? justification,
        DateTime? expectedDeliveryDate)
    {
        if (justification?.Length > 1000)
            throw new ArgumentException(
                "Justification cannot exceed 1000 characters.",
                nameof(justification));
    }

    private static string? NormalizeOptionalValue(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
