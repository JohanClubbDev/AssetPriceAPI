namespace AssetPriceAPI.Entities;

public class Source
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<Price> Prices { get; set; } = new List<Price>();
}