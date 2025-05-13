using AssetPriceAPI.Entities;

namespace AssetPriceAPI.Repositories;

public interface IPriceRepository
{
    Task<Price?> GetByAssetSourceDateAsync(Guid assetId, Guid sourceId, DateOnly date);
    Task<List<Price>> GetByAssetIdsAndDateAsync(List<Guid> assetIds, DateOnly date, Guid? sourceId = null);
    
    Task<PriceHistory?> GetCurrentHistoryEntryAsync(Guid assetId, Guid sourceId, DateOnly date);
    Task AddPriceAsync(Price price);
    Task UpdatePriceAsync(Price price);

    Task AddPriceHistoryAsync(PriceHistory priceHistory);
    Task UpdatePriceHistoryAsync(PriceHistory priceHistory);

    Task SaveChangesAsync();
}