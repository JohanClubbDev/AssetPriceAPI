namespace AssetPriceAPI.Models;

public record SourceCreateDto(
    string Name
);

public record SourceReadDto(
    Guid Id,
    string Name
);