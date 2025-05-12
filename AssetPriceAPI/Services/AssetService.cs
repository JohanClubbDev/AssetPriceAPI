using AssetPriceAPI.Entities;
using AssetPriceAPI.Repositories;

namespace AssetPriceAPI.Services;

public class AssetService : IAssetService
{
    private readonly IAssetRepository _assetRepository;

    public AssetService(IAssetRepository assetRepository)
    {
        _assetRepository = assetRepository;
    }

    public async Task<IEnumerable<Asset>> GetAllAssetsAsync()
    {
        return await _assetRepository.GetAllAsync();
    }

    public async Task<Asset?> GetAssetByIdAsync(Guid id)
    {
        return await _assetRepository.GetByIdAsync(id);
    }

    public async Task<Asset?> GetAssetBySymbolAsync(string symbol)
    {
        return await _assetRepository.GetBySymbolAsync(symbol);
    }

    public async Task<Asset?> GetAssetByIsinAsync(string isin)
    {
        return await _assetRepository.GetByIsinAsync(isin);
    }

    public async Task<Asset> CreateAssetAsync(Asset asset)
    {
        await _assetRepository.AddAsync(asset);
        await _assetRepository.SaveChangesAsync();
        return asset;
    }

    public async Task<Asset?> UpdateAssetAsync(Asset asset)
    {
        var existingAsset = await _assetRepository.GetByIdAsync(asset.Id);
        if (existingAsset == null)
            return null;

        existingAsset.Name = asset.Name;
        existingAsset.Symbol = asset.Symbol;
        existingAsset.Isin = asset.Isin;

        _assetRepository.Update(existingAsset);
        await _assetRepository.SaveChangesAsync();

        return existingAsset;
    }
}