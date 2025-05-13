using AssetPriceAPI.Data;
using AssetPriceAPI.Entities;
using AssetPriceApi.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AssetPriceApi.Tests.Repositories;

public class AssetRepositoryTests
{
    private async Task<AppDbContext> GetDbContextAsync()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);
        await context.Database.EnsureCreatedAsync();

        // Seed data
        context.Assets.AddRange(
            new Asset { Id = Guid.NewGuid(), Name = "Apple", Symbol = "AAPL", Isin = "US0378331005" },
            new Asset { Id = Guid.NewGuid(), Name = "Microsoft", Symbol = "MSFT", Isin = "US5949181045" }
        );
        await context.SaveChangesAsync();

        return context;
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllAssets()
    {
        var context = await GetDbContextAsync();
        var repo = new AssetRepository(context);

        var assets = await repo.GetAllAsync();

        Assert.Equal(2, assets.Count());
    }

    [Fact]
    public async Task GetBySymbolAsync_ReturnsCorrectAsset()
    {
        var context = await GetDbContextAsync();
        var repo = new AssetRepository(context);

        var asset = await repo.GetBySymbolAsync("MSFT");

        Assert.NotNull(asset);
        Assert.Equal("Microsoft", asset?.Name);
    }

    [Fact]
    public async Task GetByIsinAsync_ReturnsCorrectAsset()
    {
        var context = await GetDbContextAsync();
        var repo = new AssetRepository(context);

        var asset = await repo.GetByIsinAsync("US0378331005");

        Assert.NotNull(asset);
        Assert.Equal("Apple", asset?.Name);
    }

    [Fact]
    public async Task AddAsync_AddsNewAsset()
    {
        var context = await GetDbContextAsync();
        var repo = new AssetRepository(context);

        var newAsset = new Asset
        {
            Id = Guid.NewGuid(),
            Name = "Tesla",
            Symbol = "TSLA",
            Isin = "US88160R1014"
        };

        await repo.AddAsync(newAsset);
        await repo.SaveChangesAsync();

        var all = await repo.GetAllAsync();
        Assert.Equal(3, all.Count());
    }

    [Fact]
    public async Task UpdateAsync_UpdatesExistingAsset()
    {
        var context = await GetDbContextAsync();
        var repo = new AssetRepository(context);

        var asset = await repo.GetBySymbolAsync("AAPL");
        Assert.NotNull(asset);

        asset!.Name = "Apple Inc.";
        repo.Update(asset);
        await repo.SaveChangesAsync();
        

        var updated = await repo.GetBySymbolAsync("AAPL");
        Assert.Equal("Apple Inc.", updated?.Name);
    }
}
