namespace AssetPriceAPI.Models;

public record AddOrUpdatePriceRequest(
    Guid AssetId,
    Guid SourceId,
    DateOnly PriceDate,
    decimal PriceValue
);

public record PriceResponse(
    Guid AssetId,
    Guid SourceId,
    DateOnly PriceDate,
    decimal PriceValue,
    DateTime LastUpdated
);