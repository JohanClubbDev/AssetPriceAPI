namespace AssetPriceAPI.Models;

public record AssetReadDto(
    Guid Id,
    string Name,
    string Symbol,
    string Isin
);