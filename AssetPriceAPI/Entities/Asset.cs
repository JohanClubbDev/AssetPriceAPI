namespace AssetPriceAPI.Entities;

public class Asset
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public string Isin { get; set; } = string.Empty;

    public ICollection<Price> Prices { get; set; } = new List<Price>();
}