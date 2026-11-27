
namespace ProcureFlow.Domain.Entities;

public class PurchaseOrderItem
{
    public int Id { get; private set; }

    public int PurchaseOrderId { get; private set; }

    public int ProductId { get; private set; }

    public string ProductName { get; private set; } = string.Empty;

    public string SKU { get; private set; } = string.Empty;

    public decimal Quantity { get; private set; }

    public decimal UnitPrice { get; private set; }

    public decimal TaxRate { get; private set; }

    public decimal Subtotal { get; private set; }

    public decimal TaxAmount { get; private set; }

    public decimal TotalAmount { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public PurchaseOrder PurchaseOrder { get; private set; } = null!;

    public Product Product { get; private set; } = null!;

    private PurchaseOrderItem()
    {
    }

    public PurchaseOrderItem(
        int purchaseOrderId,
        int productId,
        string productName,
        string sku,
        decimal quantity,
        decimal unitPrice,
        decimal taxRate)
    {
        ValidateForeignKey(purchaseOrderId, nameof(purchaseOrderId));
        ValidateForeignKey(productId, nameof(productId));
        ValidateProductName(productName);
        ValidateSKU(sku);
        ValidateQuantity(quantity);
        ValidateUnitPrice(unitPrice);
        ValidateTaxRate(taxRate);

        PurchaseOrderId = purchaseOrderId;
        ProductId = productId;

        ProductName = productName.Trim();
        SKU = sku.Trim();

        Quantity = quantity;
        UnitPrice = unitPrice;
        TaxRate = taxRate;

        CalculateAmounts();

        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateQuantity(decimal quantity)
    {
        ValidateQuantity(quantity);

        Quantity = quantity;
        CalculateAmounts();
    }

    public void UpdateUnitPrice(decimal unitPrice)
    {
        ValidateUnitPrice(unitPrice);

        UnitPrice = unitPrice;
        CalculateAmounts();
    }

    public void UpdateTaxRate(decimal taxRate)
    {
        ValidateTaxRate(taxRate);

        TaxRate = taxRate;
        CalculateAmounts();
    }

    private void CalculateAmounts()
    {
        Subtotal = decimal.Round(
            Quantity * UnitPrice,
            2,
            MidpointRounding.AwayFromZero);

        TaxAmount = decimal.Round(
            Subtotal * (TaxRate / 100m),
            2,
            MidpointRounding.AwayFromZero);

        TotalAmount = Subtotal + TaxAmount;
    }

    private static void ValidateForeignKey(int id, string parameterName)
    {
        if (id <= 0)
            throw new ArgumentException(
                "The identifier must be greater than zero.",
                parameterName);
    }

    private static void ValidateProductName(string productName)
    {
        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException(
                "Product name is required.",
                nameof(productName));

        if (productName.Length > 150)
            throw new ArgumentException(
                "Product name cannot exceed 150 characters.",
                nameof(productName));
    }

    private static void ValidateSKU(string sku)
    {
        if (string.IsNullOrWhiteSpace(sku))
            throw new ArgumentException(
                "Product SKU is required.",
                nameof(sku));

        if (sku.Length > 50)
            throw new ArgumentException(
                "Product SKU cannot exceed 50 characters.",
                nameof(sku));
    }

    private static void ValidateQuantity(decimal quantity)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                "Quantity must be greater than zero.");

        if (decimal.Round(quantity, 3) != quantity)
            throw new ArgumentException(
                "Quantity cannot have more than three decimal places.",
                nameof(quantity));
    }

    private static void ValidateUnitPrice(decimal unitPrice)
    {
        if (unitPrice < 0)
            throw new ArgumentOutOfRangeException(
                nameof(unitPrice),
                "Unit price cannot be negative.");

        if (decimal.Round(unitPrice, 2) != unitPrice)
            throw new ArgumentException(
                "Unit price cannot have more than two decimal places.",
                nameof(unitPrice));
    }

    private static void ValidateTaxRate(decimal taxRate)
    {
        if (taxRate < 0 || taxRate > 100)
            throw new ArgumentOutOfRangeException(
                nameof(taxRate),
                "Tax rate must be between 0 and 100.");

        if (decimal.Round(taxRate, 2) != taxRate)
            throw new ArgumentException(
                "Tax rate cannot have more than two decimal places.",
                nameof(taxRate));
    }
}
