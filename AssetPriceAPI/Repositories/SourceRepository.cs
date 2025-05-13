using AssetPriceAPI.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AssetPriceAPI.Data;
using AssetPriceAPI.Repositories;

namespace AssetPriceAPI.Repositories;

public class SourceRepository : ISourceRepository
{
    private readonly AppDbContext _context;

    public SourceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Source?> GetSourceByIdAsync(Guid id)
    {
        return await _context.Sources
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Source?> GetSourceByNameAsync(string name)
    {
        return await _context.Sources
            .FirstOrDefaultAsync(s => s.Name == name);
    }

    public async Task<IEnumerable<Source>> GetAllSourcesAsync()
    {
        return await _context.Sources
            .ToListAsync();
    }

    public async Task<Source> AddSourceAsync(Source source)
    {
        _context.Sources.Add(source);
        await _context.SaveChangesAsync();
        return source;
    }

    public async Task<Source> UpdateSourceAsync(Source source)
    {
        _context.Sources.Update(source);
        await _context.SaveChangesAsync();
        return source;
    }

    public async Task<bool> DeleteSourceAsync(Guid id)
    {
        var source = await _context.Sources.FindAsync(id);
        if (source == null)
        {
            return false;
        }

        _context.Sources.Remove(source);
        await _context.SaveChangesAsync();
        return true;
    }
}