using AssetPriceAPI.Entities;

namespace AssetPriceAPI.Repositories;

public interface IAssetRepository
{
    Task<IEnumerable<Asset>> GetAllAsync();
    Task<Asset?> GetByIdAsync(Guid id);
    Task<Asset?> GetByIsinAsync(string isin);
    Task<Asset?> GetBySymbolAsync(string symbol);
    Task AddAsync(Asset asset);
    void Update(Asset asset);
    Task<bool> SaveChangesAsync();
}