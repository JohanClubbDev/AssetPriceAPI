using AssetPriceAPI.Entities;

namespace AssetPriceAPI.Services;

public interface IAssetService
{
    Task<IEnumerable<Asset>> GetAllAssetsAsync();
    Task<Asset?> GetAssetByIdAsync(Guid id);
    Task<Asset?> GetAssetBySymbolAsync(string symbol);
    Task<Asset?> GetAssetByIsinAsync(string isin);
    Task<Asset> CreateAssetAsync(Asset asset);
    Task<Asset?> UpdateAssetAsync(Asset asset);
}