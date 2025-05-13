using AssetPriceAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace AssetPriceAPI.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Seed Assets
        if (!context.Assets.Any())
        {
            var assets = new List<Asset>
            {
                new() { Id = Guid.NewGuid(), Name = "Apple Inc.", Symbol = "AAPL", Isin = "US0378331005" },
                new() { Id = Guid.NewGuid(), Name = "Microsoft Corp.", Symbol = "MSFT", Isin = "US5949181045" }
            };

            context.Assets.AddRange(assets);
        }

        // Seed Sources
        if (!context.Sources.Any())
        {
            var sources = new List<Source>
            {
                new() { Id = Guid.NewGuid(), Name = "Yahoo Finance" },
                new() { Id = Guid.NewGuid(), Name = "Bloomberg" }
            };

            context.Sources.AddRange(sources);
        }

        await context.SaveChangesAsync();
    }
}