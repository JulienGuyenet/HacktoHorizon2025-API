using Microsoft.EntityFrameworkCore;
using FurnitureInventory.Core.Entities;

namespace FurnitureInventory.Infrastructure.Data;

/// <summary>
/// Contexte de base de données pour l'inventaire de meubles
/// </summary>
public class FurnitureInventoryContext : DbContext
{
    public FurnitureInventoryContext(DbContextOptions<FurnitureInventoryContext> options)
        : base(options)
    {
    }

    public DbSet<Furniture> Furnitures => Set<Furniture>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<RfidTag> RfidTags => Set<RfidTag>();
    public DbSet<RfidReader> RfidReaders => Set<RfidReader>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuration de l'entité Furniture
        modelBuilder.Entity<Furniture>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Reference).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Designation).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Famille).HasMaxLength(100);
            entity.Property(e => e.Type).HasMaxLength(100);
            entity.Property(e => e.Fournisseur).HasMaxLength(200);
            entity.Property(e => e.Utilisateur).HasMaxLength(200);
            entity.Property(e => e.CodeBarre).HasMaxLength(100);
            entity.Property(e => e.NumeroSerie).HasMaxLength(100);
            entity.Property(e => e.Site).HasMaxLength(200);

            entity.HasIndex(e => e.Reference);
            entity.HasIndex(e => e.CodeBarre);
            entity.HasIndex(e => e.NumeroSerie);

            entity.HasOne(e => e.Location)
                .WithMany(l => l.Furnitures)
                .HasForeignKey(e => e.LocationId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.RfidTag)
                .WithOne(t => t.Furniture)
                .HasForeignKey<Furniture>(e => e.RfidTagId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configuration de l'entité Location
        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BuildingName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Floor).HasMaxLength(50);
            entity.Property(e => e.Room).HasMaxLength(100);
            entity.Property(e => e.Zone).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);

            entity.HasIndex(e => e.BuildingName);
        });

        // Configuration de l'entité RfidTag
        modelBuilder.Entity<RfidTag>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TagId).IsRequired().HasMaxLength(100);
            entity.Property(e => e.TagType).HasMaxLength(50);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);

            entity.HasIndex(e => e.TagId).IsUnique();

            entity.HasOne(e => e.LastReader)
                .WithMany(r => r.RfidTags)
                .HasForeignKey(e => e.LastReaderId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configuration de l'entité RfidReader
        modelBuilder.Entity<RfidReader>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ReaderId).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Model).HasMaxLength(100);
            entity.Property(e => e.IpAddress).HasMaxLength(45);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);

            entity.HasIndex(e => e.ReaderId).IsUnique();

            entity.HasOne(e => e.Location)
                .WithMany(l => l.RfidReaders)
                .HasForeignKey(e => e.LocationId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                if (entry.Entity.GetType().GetProperty("CreatedAt") != null)
                {
                    entry.Property("CreatedAt").CurrentValue = DateTime.UtcNow;
                }
            }

            if (entry.State == EntityState.Modified)
            {
                if (entry.Entity.GetType().GetProperty("UpdatedAt") != null)
                {
                    entry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
                }
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
