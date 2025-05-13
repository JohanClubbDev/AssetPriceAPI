using AssetPriceAPI.Entities;

namespace AssetPriceAPI.Services;

public interface IPriceService
{
    Task<Price?> GetPriceAsync(Guid assetId, DateOnly date, Guid? sourceId = null);
    Task<List<Price>> GetPricesAsync(List<Guid> assetIds, DateOnly date, Guid? sourceId = null);
    Task AddOrUpdatePriceAsync(Guid assetId, Guid sourceId, DateOnly date, decimal value);
}