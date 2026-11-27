
namespace ProcureFlow.Domain.Entities;

public class TaxConfiguration
{
    public int Id { get; private set; }

    public decimal TaxRate { get; private set; }

    public bool IsActive { get; private set; }

    public string? Description { get; private set; }

    public int CreatedByUserId { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public User CreatedByUser { get; private set; } = null!;

    private TaxConfiguration()
    {
    }

    public TaxConfiguration(
        decimal taxRate,
        int createdByUserId,
        string? description = null)
    {
        ValidateTaxRate(taxRate);
        ValidateForeignKey(createdByUserId, nameof(createdByUserId));
        ValidateDescription(description);

        TaxRate = taxRate;
        CreatedByUserId = createdByUserId;
        Description = NormalizeOptionalValue(description);

        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw new InvalidOperationException(
                "Tax configuration is already inactive.");

        IsActive = false;
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

    private static void ValidateForeignKey(int id, string parameterName)
    {
        if (id <= 0)
            throw new ArgumentException(
                "The identifier must be greater than zero.",
                parameterName);
    }

    private static void ValidateDescription(string? description)
    {
        if (description?.Length > 500)
            throw new ArgumentException(
                "Description cannot exceed 500 characters.",
                nameof(description));
    }

    private static string? NormalizeOptionalValue(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
