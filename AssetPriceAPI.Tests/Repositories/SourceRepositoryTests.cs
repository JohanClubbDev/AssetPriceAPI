using AssetPriceAPI.Data;
using AssetPriceAPI.Entities;
using AssetPriceAPI.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AssetPriceApi.Tests.Repositories;

public class SourceRepositoryTests
{
    private async Task<AppDbContext> GetDbContextAsync()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);
        await context.Database.EnsureCreatedAsync();

        context.Sources.AddRange(
            new Source { Id = Guid.NewGuid(), Name = "Bloomberg" },
            new Source { Id = Guid.NewGuid(), Name = "Reuters" }
        );

        await context.SaveChangesAsync();
        return context;
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllSources()
    {
        var context = await GetDbContextAsync();
        var repo = new SourceRepository(context);

        var sources = await repo.GetAllSourcesAsync();

        Assert.Equal(2, sources.Count());
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsCorrectSource()
    {
        var context = await GetDbContextAsync();
        var existing = await context.Sources.FirstAsync();
        var repo = new SourceRepository(context);

        var source = await repo.GetSourceByIdAsync(existing.Id);

        Assert.NotNull(source);
        Assert.Equal(existing.Name, source!.Name);
    }

    [Fact]
    public async Task AddAsync_AddsNewSource()
    {
        var context = await GetDbContextAsync();
        var repo = new SourceRepository(context);

        var newSource = new Source
        {
            Id = Guid.NewGuid(),
            Name = "Yahoo Finance"
        };

        await repo.AddSourceAsync(newSource);

        var all = await repo.GetAllSourcesAsync();
        Assert.Equal(3, all.Count());
    }

    [Fact]
    public async Task DeleteAsync_RemovesSource()
    {
        var context = await GetDbContextAsync();
        var existing = await context.Sources.FirstAsync();
        var repo = new SourceRepository(context);

        await repo.DeleteSourceAsync(existing.Id);

        var all = await repo.GetAllSourcesAsync();
        Assert.Single(all);
    }
}