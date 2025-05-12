namespace AssetPriceAPI.Entities;

public class PriceHistory
{
    public Guid Id { get; set; }

    public Guid AssetId { get; set; }
    public Guid SourceId { get; set; }

    public DateOnly PriceDate { get; set; }

    public decimal PriceValue { get; set; }

    public DateTime ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
}
