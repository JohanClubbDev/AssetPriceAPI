namespace AssetPriceAPI.Models;

public class AssetReadDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public string Isin { get; set; } = string.Empty;
}