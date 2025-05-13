namespace AssetPriceAPI.Models;

public record AssetCreateDto(
    string Name,
    string Symbol,
    string Isin
);