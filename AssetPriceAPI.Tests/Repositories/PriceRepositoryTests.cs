using AssetPriceAPI.Data;
using AssetPriceAPI.Entities;
using AssetPriceAPI.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AssetPriceApi.Tests.Repositories;

public class PriceRepositoryTests
{
    private async Task<AppDbContext> GetDbContextAsync()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var assetId = Guid.NewGuid();
        var sourceId = Guid.NewGuid();
        var priceDate = new DateOnly(2024, 1, 1);

        var asset = new Asset { Id = assetId, Name = "Asset 1", Symbol = "AST1", Isin = "ISIN1" };
        var source = new Source { Id = sourceId, Name = "Source 1" };

        var price = new Price
        {
            Id = Guid.NewGuid(),
            AssetId = assetId,
            SourceId = sourceId,
            PriceDate = priceDate,
            PriceValue = 123.45m,
            LastUpdated = DateTime.UtcNow,
            Asset = asset,
            Source = source
        };

        var history = new PriceHistory
        {
            Id = Guid.NewGuid(),
            AssetId = assetId,
            SourceId = sourceId,
            PriceDate = priceDate,
            PriceValue = 123.45m,
            ValidFrom = DateTime.UtcNow
        };

        context.Assets.Add(asset);
        context.Sources.Add(source);
        context.Prices.Add(price);
        context.PriceHistories.Add(history);

        await context.SaveChangesAsync();
        return context;
    }

    [Fact]
    public async Task GetByAssetSourceDateAsync_ReturnsCorrectPrice()
    {
        var context = await GetDbContextAsync();
        var repo = new PriceRepository(context);

        var existing = context.Prices.First();
        var result = await repo.GetByAssetSourceDateAsync(existing.AssetId, existing.SourceId, existing.PriceDate);

        Assert.NotNull(result);
        Assert.Equal(existing.PriceValue, result!.PriceValue);
    }

    [Fact]
    public async Task GetByAssetIdsAndDateAsync_ReturnsFilteredPrices()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);

        var assetId = Guid.NewGuid();
        var otherAssetId = Guid.NewGuid();
        var sourceId = Guid.NewGuid();
        var otherSourceId = Guid.NewGuid();
        var targetDate = new DateOnly(2024, 1, 1);
        var wrongDate = new DateOnly(2023, 12, 31);

        var asset = new Asset { Id = assetId, Name = "Asset A", Symbol = "AAA", Isin = "ISINA" };
        var otherAsset = new Asset { Id = otherAssetId, Name = "Asset B", Symbol = "BBB", Isin = "ISINB" };
        var source = new Source { Id = sourceId, Name = "Source A" };
        var otherSource = new Source { Id = otherSourceId, Name = "Source B" };

        var matchingPrice = new Price
        {
            Id = Guid.NewGuid(),
            AssetId = assetId,
            SourceId = sourceId,
            PriceDate = targetDate,
            PriceValue = 100,
            LastUpdated = DateTime.UtcNow,
            Asset = asset,
            Source = source
        };

        var wrongAssetPrice = new Price
        {
            Id = Guid.NewGuid(),
            AssetId = otherAssetId,
            SourceId = sourceId,
            PriceDate = targetDate,
            PriceValue = 200,
            LastUpdated = DateTime.UtcNow,
            Asset = otherAsset,
            Source = source
        };

        var wrongDatePrice = new Price
        {
            Id = Guid.NewGuid(),
            AssetId = assetId,
            SourceId = sourceId,
            PriceDate = wrongDate,
            PriceValue = 300,
            LastUpdated = DateTime.UtcNow,
            Asset = asset,
            Source = source
        };

        var wrongSourcePrice = new Price
        {
            Id = Guid.NewGuid(),
            AssetId = assetId,
            SourceId = otherSourceId,
            PriceDate = targetDate,
            PriceValue = 400,
            LastUpdated = DateTime.UtcNow,
            Asset = asset,
            Source = otherSource
        };

        await context.Assets.AddRangeAsync(asset, otherAsset);
        await context.Sources.AddRangeAsync(source, otherSource);
        await context.Prices.AddRangeAsync(matchingPrice, wrongAssetPrice, wrongDatePrice, wrongSourcePrice);
        await context.SaveChangesAsync();

        var repo = new PriceRepository(context);

        // Act
        var result = await repo.GetByAssetIdsAndDateAsync(
            new List<Guid> { assetId },
            targetDate,
            sourceId
        );

        // Assert
        Assert.Single(result);
        var returned = result.First();
        Assert.Equal(matchingPrice.PriceValue, returned.PriceValue);
        Assert.Equal(assetId, returned.AssetId);
        Assert.Equal(sourceId, returned.SourceId);
    }

    [Fact]
    public async Task GetCurrentHistoryEntryAsync_ReturnsLatestHistory()
    {
        var context = await GetDbContextAsync();
        var repo = new PriceRepository(context);

        var existing = context.PriceHistories.First();
        var result = await repo.GetCurrentHistoryEntryAsync(existing.AssetId, existing.SourceId, existing.PriceDate);

        Assert.NotNull(result);
        Assert.Equal(existing.PriceValue, result!.PriceValue);
        Assert.Null(result.ValidTo);
    }

    [Fact]
    public async Task AddPriceAsync_SavesNewPrice()
    {
        var context = await GetDbContextAsync();
        var repo = new PriceRepository(context);

        var newPrice = new Price
        {
            Id = Guid.NewGuid(),
            AssetId = context.Assets.First().Id,
            SourceId = context.Sources.First().Id,
            PriceDate = new DateOnly(2024, 2, 2),
            PriceValue = 999.99m,
            LastUpdated = DateTime.UtcNow,
        };

        await repo.AddPriceAsync(newPrice);
        await repo.SaveChangesAsync();

        var added = await context.Prices.FindAsync(newPrice.Id);
        Assert.NotNull(added);
        Assert.Equal(999.99m, added!.PriceValue);
    }

    [Fact]
    public async Task UpdatePriceAsync_UpdatesPriceCorrectly()
    {
        var context = await GetDbContextAsync();
        var repo = new PriceRepository(context);

        var existing = context.Prices.First();
        existing.PriceValue = 777.77m;

        await repo.UpdatePriceAsync(existing);
        await repo.SaveChangesAsync();

        var updated = await context.Prices.FindAsync(existing.Id);
        Assert.Equal(777.77m, updated!.PriceValue);
    }

    [Fact]
    public async Task AddPriceHistoryAsync_SavesNewHistory()
    {
        var context = await GetDbContextAsync();
        var repo = new PriceRepository(context);

        var newHistory = new PriceHistory
        {
            Id = Guid.NewGuid(),
            AssetId = context.Assets.First().Id,
            SourceId = context.Sources.First().Id,
            PriceDate = new DateOnly(2024, 3, 3),
            PriceValue = 88.88m,
            ValidFrom = DateTime.UtcNow
        };

        await repo.AddPriceHistoryAsync(newHistory);
        await repo.SaveChangesAsync();

        var added = await context.PriceHistories.FindAsync(newHistory.Id);
        Assert.NotNull(added);
        Assert.Equal(88.88m, added!.PriceValue);
    }

    [Fact]
    public async Task UpdatePriceHistoryAsync_UpdatesHistory()
    {
        var context = await GetDbContextAsync();
        var repo = new PriceRepository(context);

        var existing = context.PriceHistories.First();
        existing.ValidTo = DateTime.UtcNow;

        await repo.UpdatePriceHistoryAsync(existing);
        await repo.SaveChangesAsync();

        var updated = await context.PriceHistories.FindAsync(existing.Id);
        Assert.NotNull(updated!.ValidTo);
    }
}