
namespace ProcureFlow.Domain.Entities;

public class Attachment
{
    public int Id { get; private set; }

    public int? PurchaseOrderId { get; private set; }

    public int? RequestId { get; private set; }

    public int? ReceivingIssueId { get; private set; }

    public int UploadedByUserId { get; private set; }

    public string FileName { get; private set; } = string.Empty;

    public string StorageKey { get; private set; } = string.Empty;

    public string ContentType { get; private set; } = string.Empty;

    public long FileSize { get; private set; }

    public bool IsDeleted { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? DeletedAt { get; private set; }

    public PurchaseOrder? PurchaseOrder { get; private set; }

    public Request? Request { get; private set; }

    public ReceivingIssue? ReceivingIssue { get; private set; }

    public User UploadedByUser { get; private set; } = null!;

    private Attachment()
    {
    }

    public Attachment(
        int uploadedByUserId,
        string fileName,
        string storageKey,
        string contentType,
        long fileSize,
        int? purchaseOrderId = null,
        int? requestId = null,
        int? receivingIssueId = null)
    {
        ValidateForeignKey(uploadedByUserId, nameof(uploadedByUserId));
        ValidateFileName(fileName);
        ValidateStorageKey(storageKey);
        ValidateContentType(contentType);
        ValidateFileSize(fileSize);
        ValidateAssociation(purchaseOrderId, requestId, receivingIssueId);

        UploadedByUserId = uploadedByUserId;

        FileName = fileName.Trim();
        StorageKey = storageKey.Trim();
        ContentType = contentType.Trim();
        FileSize = fileSize;

        PurchaseOrderId = purchaseOrderId;
        RequestId = requestId;
        ReceivingIssueId = receivingIssueId;

        IsDeleted = false;
        CreatedAt = DateTime.UtcNow;
    }

    public void Delete()
    {
        if (IsDeleted)
            throw new InvalidOperationException(
                "Attachment is already deleted.");

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

    private static void ValidateFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException(
                "File name is required.",
                nameof(fileName));

        if (fileName.Length > 255)
            throw new ArgumentException(
                "File name cannot exceed 255 characters.",
                nameof(fileName));
    }

    private static void ValidateStorageKey(string storageKey)
    {
        if (string.IsNullOrWhiteSpace(storageKey))
            throw new ArgumentException(
                "Storage key is required.",
                nameof(storageKey));

        if (storageKey.Length > 500)
            throw new ArgumentException(
                "Storage key cannot exceed 500 characters.",
                nameof(storageKey));
    }

    private static void ValidateContentType(string contentType)
    {
        if (string.IsNullOrWhiteSpace(contentType))
            throw new ArgumentException(
                "Content type is required.",
                nameof(contentType));

        if (contentType.Length > 150)
            throw new ArgumentException(
                "Content type cannot exceed 150 characters.",
                nameof(contentType));
    }

    private static void ValidateFileSize(long fileSize)
    {
        if (fileSize <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(fileSize),
                "File size must be greater than zero.");
    }

    private static void ValidateAssociation(
        int? purchaseOrderId,
        int? requestId,
        int? receivingIssueId)
    {
        var associationCount = 0;

        if (purchaseOrderId.HasValue)
        {
            ValidateForeignKey(purchaseOrderId.Value, nameof(purchaseOrderId));
            associationCount++;
        }

        if (requestId.HasValue)
        {
            ValidateForeignKey(requestId.Value, nameof(requestId));
            associationCount++;
        }

        if (receivingIssueId.HasValue)
        {
            ValidateForeignKey(receivingIssueId.Value, nameof(receivingIssueId));
            associationCount++;
        }

        if (associationCount != 1)
            throw new ArgumentException(
                "An attachment must be associated with exactly one entity.");
    }
}
