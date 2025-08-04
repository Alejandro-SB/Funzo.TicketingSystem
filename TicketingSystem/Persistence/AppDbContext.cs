using Microsoft.EntityFrameworkCore;
using TicketingSystem.Persistence.Entities;

namespace TicketingSystem.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Ticket> Tickets { get; set; } = null!;
    public DbSet<Comment> Comments { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetLogInformation();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        SetLogInformation();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    public override int SaveChanges()
    {
        SetLogInformation();
        return base.SaveChanges();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        SetLogInformation();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    private void SetLogInformation()
    {
        var entries = ChangeTracker.Entries().Where(e => e.State is EntityState.Added or EntityState.Modified).ToList();

        foreach (var entry in entries)
        {
            if (entry.Entity is not BaseEntity entity)
            {
                continue;
            }

            if (entry.State is EntityState.Added)
            {
                entity.CreatedAt = DateTimeOffset.UtcNow;
            }
            else
            {
                entry.Property(nameof(BaseEntity.CreatedAt)).IsModified = false;
            }

            entity.LastModifiedAt = DateTimeOffset.UtcNow;
        }
    }
}
