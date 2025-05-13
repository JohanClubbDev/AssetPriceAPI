using AssetPriceAPI.Data;
using AssetPriceAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace AssetPriceAPI.Repositories;

public class PriceRepository : IPriceRepository
    {
        private readonly AppDbContext _context;

        public PriceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Price?> GetByAssetSourceDateAsync(Guid assetId, Guid sourceId, DateOnly date)
        {
            return await _context.Prices
                .FirstOrDefaultAsync(p => p.AssetId == assetId && p.SourceId == sourceId && p.PriceDate == date);
        }

        public async Task<List<Price>> GetByAssetIdsAndDateAsync(List<Guid> assetIds, DateOnly date, Guid? sourceId = null)
        {
            var query = _context.Prices
                .Where(p => assetIds.Contains(p.AssetId) && p.PriceDate == date);

            if (sourceId.HasValue)
            {
                query = query.Where(p => p.SourceId == sourceId.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<PriceHistory?> GetCurrentHistoryEntryAsync(Guid assetId, Guid sourceId, DateOnly date)
        {
            return await _context.PriceHistories
                .Where(ph => ph.AssetId == assetId && ph.SourceId == sourceId && ph.PriceDate == date && ph.ValidTo == null)
                .FirstOrDefaultAsync();
        }

        public async Task AddPriceAsync(Price price)
        {
            await _context.Prices.AddAsync(price);
        }

        public Task UpdatePriceAsync(Price price)
        {
            _context.Prices.Update(price);
            return Task.CompletedTask;
        }

        public async Task AddPriceHistoryAsync(PriceHistory priceHistory)
        {
            await _context.PriceHistories.AddAsync(priceHistory);
        }

        public Task UpdatePriceHistoryAsync(PriceHistory priceHistory)
        {
            _context.PriceHistories.Update(priceHistory);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }