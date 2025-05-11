namespace AssetPriceAPI.Entities;

public class Price
{
    public Guid Id { get; set; }

    public Guid AssetId { get; set; }
    public Asset Asset { get; set; } = null!;

    public Guid SourceId { get; set; }
    public Source Source { get; set; } = null!;

    public DateOnly PriceDate { get; set; }

    public decimal PriceValue { get; set; }

    public DateTime ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }

    public DateTime LastUpdated { get; set; }
}

