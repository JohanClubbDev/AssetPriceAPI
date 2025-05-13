using AssetPriceAPI.Entities;
using AssetPriceAPI.Repositories;

namespace AssetPriceAPI.Services;

public class PriceService : IPriceService
{
    private readonly IPriceRepository _priceRepository;

    public PriceService(IPriceRepository priceRepository)
    {
        _priceRepository = priceRepository;
    }

    public async Task<Price?> GetPriceAsync(Guid assetId, DateOnly date, Guid? sourceId = null)
    {
        var result = await _priceRepository.GetByAssetIdsAndDateAsync(new List<Guid> { assetId }, date, sourceId);
        return result.FirstOrDefault();
    }

    public async Task<List<Price>> GetPricesAsync(List<Guid> assetIds, DateOnly date, Guid? sourceId = null)
    {
        return await _priceRepository.GetByAssetIdsAndDateAsync(assetIds, date, sourceId);
    }

    public async Task AddOrUpdatePriceAsync(Guid assetId, Guid sourceId, DateOnly date, decimal value)
    {
        var now = DateTime.UtcNow;

        var existingPrice = await _priceRepository.GetByAssetSourceDateAsync(assetId, sourceId, date);

        if (existingPrice != null)
        {
            if (existingPrice.PriceValue != value)
            {
                var existingHistory = await _priceRepository.GetCurrentHistoryEntryAsync(assetId, sourceId, date);
                if (existingHistory != null)
                {
                    existingHistory.ValidTo = now;
                    await _priceRepository.UpdatePriceHistoryAsync(existingHistory);
                }

                var newHistory = new PriceHistory
                {
                    Id = Guid.NewGuid(),
                    AssetId = assetId,
                    SourceId = sourceId,
                    PriceDate = date,
                    PriceValue = value,
                    ValidFrom = now,
                    ValidTo = null
                };
                await _priceRepository.AddPriceHistoryAsync(newHistory);

                existingPrice.PriceValue = value;
                existingPrice.LastUpdated = now;
                await _priceRepository.UpdatePriceAsync(existingPrice);
            }
        }
        else
        {
            var newPrice = new Price
            {
                Id = Guid.NewGuid(),
                AssetId = assetId,
                SourceId = sourceId,
                PriceDate = date,
                PriceValue = value,
                LastUpdated = now
            };
            await _priceRepository.AddPriceAsync(newPrice);

            var newHistory = new PriceHistory
            {
                Id = Guid.NewGuid(),
                AssetId = assetId,
                SourceId = sourceId,
                PriceDate = date,
                PriceValue = value,
                ValidFrom = now
            };
            await _priceRepository.AddPriceHistoryAsync(newHistory);
        }

        await _priceRepository.SaveChangesAsync();
    }
}