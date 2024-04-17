using Enable.Presentation.EventSourcing.EventStreaming.Features.EventStreaming.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Enable.Presentation.EventSourcing.Infrastructure.Layer.Data.Context;

/// <summary>
/// A database context example for event sourcing
/// </summary>
public interface IEnablePresentationDbContext
{
    DbSet<EventOutbox> EventOutBox { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// A database context example for event sourcing
/// </summary>
public class EnablePresentationDbContext(DbContextOptions dbContextOptions) : DbContext(dbContextOptions), IEnablePresentationDbContext
{
    public DbSet<EventOutbox> EventOutBox { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
       return await base.SaveChangesAsync(cancellationToken);
    }
}
