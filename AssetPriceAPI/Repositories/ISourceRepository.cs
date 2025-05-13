using AssetPriceAPI.Entities;

namespace AssetPriceAPI.Repositories;

public interface ISourceRepository
{
    Task<Source?> GetSourceByIdAsync(Guid id);
    Task<Source?> GetSourceByNameAsync(string name);
    Task<IEnumerable<Source>> GetAllSourcesAsync();
    Task<Source> AddSourceAsync(Source source);
    Task<Source> UpdateSourceAsync(Source source);
    Task<bool> DeleteSourceAsync(Guid id);
}