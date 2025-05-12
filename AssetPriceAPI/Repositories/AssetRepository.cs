using AssetPriceAPI.Data;
using AssetPriceAPI.Entities;
using AssetPriceAPI.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AssetPriceApi.Repositories
{
    public class AssetRepository : IAssetRepository
    {
        private readonly AppDbContext _context;

        public AssetRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Asset>> GetAllAsync()
        {
            return await _context.Assets
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Asset?> GetByIdAsync(Guid id)
        {
            return await _context.Assets
                .Include(a => a.Prices)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Asset?> GetBySymbolAsync(string symbol)
        {
            return await _context.Assets
                .Include(a => a.Prices)
                .FirstOrDefaultAsync(a => a.Symbol == symbol);
        }
        
        public async Task<Asset?> GetByIsinAsync(string isin)
        {
            return await _context.Assets
                .Include(a => a.Prices)
                .FirstOrDefaultAsync(a => a.Isin == isin);
        }

        public async Task AddAsync(Asset asset)
        {
            await _context.Assets.AddAsync(asset);
        }

        public void Update(Asset asset)
        {
            _context.Assets.Update(asset);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}