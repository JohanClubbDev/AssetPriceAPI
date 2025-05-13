namespace AssetPriceAPI.Models;

public record PriceCreateDto(
    Guid AssetId,
    Guid SourceId,
    DateOnly PriceDate,
    decimal PriceValue
);
public record PriceReadDto(
    Guid Id,
    Guid AssetId,
    string AssetName,
    Guid SourceId,
    string SourceName,
    DateOnly PriceDate,
    decimal PriceValue,
    DateTime LastUpdated
);