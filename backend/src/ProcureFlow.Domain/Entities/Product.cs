
namespace ProcureFlow.Domain.Entities;

public enum ProductStatus
{
    InStock = 1,
    OutOfStock = 2,
    Discontinued = 3
}

public class Product
{
    public int Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string SKU { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public decimal UnitPrice { get; private set; }

    public ProductStatus Status { get; private set; }

    public int CategoryId { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public Category Category { get; private set; } = null!;

    private Product()
    {
    }

    public Product(
        string name,
        string sku,
        decimal unitPrice,
        int categoryId,
        string? description = null)
    {
        ValidateName(name);
        ValidateSKU(sku);
        ValidateUnitPrice(unitPrice);
        ValidateCategoryId(categoryId);
        ValidateDescription(description);

        Name = name.Trim();
        SKU = sku.Trim();
        UnitPrice = unitPrice;
        CategoryId = categoryId;
        Description = NormalizeOptionalValue(description);

        Status = ProductStatus.InStock;
        IsActive = true;

        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public void UpdateDetails(
        string name,
        string sku,
        decimal unitPrice,
        int categoryId,
        string? description)
    {
        ValidateName(name);
        ValidateSKU(sku);
        ValidateUnitPrice(unitPrice);
        ValidateCategoryId(categoryId);
        ValidateDescription(description);

        Name = name.Trim();
        SKU = sku.Trim();
        UnitPrice = unitPrice;
        CategoryId = categoryId;
        Description = NormalizeOptionalValue(description);

        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeStatus(ProductStatus status)
    {
        if (!Enum.IsDefined(status))
            throw new ArgumentOutOfRangeException(nameof(status), "Invalid product status.");

        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw new InvalidOperationException("Product is already inactive.");

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        if (IsActive)
            throw new InvalidOperationException("Product is already active.");

        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name is required.", nameof(name));

        if (name.Length > 150)
            throw new ArgumentException("Product name cannot exceed 150 characters.", nameof(name));
    }

    private static void ValidateSKU(string sku)
    {
        if (string.IsNullOrWhiteSpace(sku))
            throw new ArgumentException("Product SKU is required.", nameof(sku));

        if (sku.Length > 50)
            throw new ArgumentException("Product SKU cannot exceed 50 characters.", nameof(sku));
    }

    private static void ValidateUnitPrice(decimal unitPrice)
    {
        if (unitPrice < 0)
            throw new ArgumentOutOfRangeException(nameof(unitPrice), "Unit price cannot be negative.");

        if (decimal.Round(unitPrice, 2) != unitPrice)
            throw new ArgumentException("Unit price cannot have more than two decimal places.", nameof(unitPrice));
    }

    private static void ValidateCategoryId(int categoryId)
    {
        if (categoryId <= 0)
            throw new ArgumentException("Category ID must be greater than zero.", nameof(categoryId));
    }

    private static void ValidateDescription(string? description)
    {
        if (description?.Length > 500)
            throw new ArgumentException("Product description cannot exceed 500 characters.", nameof(description));
    }

    private static string? NormalizeOptionalValue(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
