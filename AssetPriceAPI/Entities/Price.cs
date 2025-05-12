namespace AssetPriceAPI.Entities;

public class Price
{
    public Guid Id { get; set; }

    public Guid AssetId { get; set; }
    public required Asset Asset { get; set; }

    public Guid SourceId { get; set; }
    public Source Source { get; set; } = null!;

    public DateOnly PriceDate { get; set; }

    public decimal PriceValue { get; set; }

    public DateTime LastUpdated { get; set; }
}

