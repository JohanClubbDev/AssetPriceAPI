using AssetPriceAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace AssetPriceAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Asset> Assets { get; set; }
    public DbSet<Source> Sources { get; set; }
    public DbSet<Price> Prices { get; set; }
    public DbSet<PriceHistory> PriceHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Asset
        modelBuilder.Entity<Asset>()
            .HasKey(a => a.Id);

        modelBuilder.Entity<Asset>()
            .HasIndex(a => a.Symbol)
            .IsUnique();

        modelBuilder.Entity<Asset>()
            .HasIndex(a => a.Isin)
            .IsUnique();

        modelBuilder.Entity<Asset>()
            .Property(a => a.Name)
            .IsRequired();

        modelBuilder.Entity<Asset>()
            .Property(a => a.Symbol)
            .IsRequired()
            .HasMaxLength(20);

        modelBuilder.Entity<Asset>()
            .Property(a => a.Isin)
            .IsRequired()
            .HasMaxLength(20);

        // Source
        modelBuilder.Entity<Source>()
            .HasKey(s => s.Id);

        modelBuilder.Entity<Source>()
            .HasIndex(s => s.Name)
            .IsUnique();

        modelBuilder.Entity<Source>()
            .Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200);

        // Price
        modelBuilder.Entity<Price>()
            .HasKey(p => p.Id);

        modelBuilder.Entity<Price>()
            .HasOne(p => p.Asset)
            .WithMany(a => a.Prices)
            .HasForeignKey(p => p.AssetId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Price>()
            .HasOne(p => p.Source)
            .WithMany(s => s.Prices)
            .HasForeignKey(p => p.SourceId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Price>()
            .HasIndex(p => new { p.AssetId, p.SourceId, p.PriceDate })
            .IsUnique(); // Enforce one price per asset-source-date

        modelBuilder.Entity<Price>()
            .Property(p => p.PriceValue)
            .HasPrecision(18, 4); // Adjust precision as needed

        // PriceHistory
        modelBuilder.Entity<PriceHistory>()
            .HasKey(ph => ph.Id);

        modelBuilder.Entity<PriceHistory>()
            .HasIndex(ph => new { ph.AssetId, ph.SourceId, ph.PriceDate });

        modelBuilder.Entity<PriceHistory>()
            .Property(ph => ph.PriceValue)
            .HasPrecision(18, 4);

        modelBuilder.Entity<PriceHistory>()
            .Property(ph => ph.ValidTo)
            .IsRequired(false);
    }
}