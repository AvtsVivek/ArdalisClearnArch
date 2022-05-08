using Ardalis.EFCore.Extensions;
using SoftDelete.Core.ProjectAggregate;
using SoftDelete.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace SoftDelete.Infrastructure.Data;

public class AppDbContext : DbContext
{
  private readonly IMediator? _mediator;

  //public AppDbContext(DbContextOptions options) : base(options)
  //{
  //}

  public AppDbContext(DbContextOptions<AppDbContext> options, IMediator? mediator)
      : base(options)
  {
    _mediator = mediator;
  }

  public DbSet<ToDoItem> ToDoItems => Set<ToDoItem>();
  public DbSet<Project> Projects => Set<Project>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    foreach (var entityType in modelBuilder.Model.GetEntityTypes())
    {
      //other automated configurations left out
      if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
      {
        entityType.AddSoftDeleteQueryFilter();
      }
    }

    modelBuilder.ApplyAllConfigurationsFromCurrentAssembly();

    // alternately this is built-in to EF Core 2.2
    //modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }

  public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
  {
    foreach (var entry in ChangeTracker.Entries<ISoftDelete>().Where(e => e.State == EntityState.Deleted))
    {
      entry.State = EntityState.Modified;
      entry.CurrentValues["IsSoftDeleted"] = true;
    }

    int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

    // ignore events if no dispatcher provided
    if (_mediator == null) return result;

    // dispatch events only if save was successful
    //var entitiesWithEvents = ChangeTracker.Entries<BaseEntity>()
    //    .Select(e => e.Entity)
    //    .Where(e => e.Events.Any())
    //    .ToArray();

    //foreach (var entity in entitiesWithEvents)
    //{
    //  var events = entity.Events.ToArray();
    //  entity.Events.Clear();
    //  foreach (var domainEvent in events)
    //  {
    //    await _mediator.Publish(domainEvent).ConfigureAwait(false);
    //  }
    //}

    // dispatch events only if save was successful
    await RaiseDominaEventsUsingForLoop().ConfigureAwait(false);

    return result;
  }

  public override int SaveChanges()
  {
    return SaveChangesAsync().GetAwaiter().GetResult();
  }

  private async Task RaiseDominaEvents()
  {
    try
    {
      var baseEntryArray = ChangeTracker.Entries().Where(entry =>
      entry.Entity.GetType().BaseType!.IsGenericType
        && entry.Entity.GetType().BaseType?.GetGenericTypeDefinition() == typeof(BaseEntity<>));

      var baseEntityList = baseEntryArray.Select(e => e.Entity);

      foreach (var baseEntity in baseEntityList)
      {
        var eventsField = baseEntity.GetType().GetField(nameof(BaseEntity<int>.Events));
        var listOfDomainEvents = (List<BaseDomainEvent>)eventsField!.GetValue(baseEntity)!;
        if (listOfDomainEvents.Any())
        {
          foreach (var domainEvent in listOfDomainEvents)
          {
            await _mediator!.Publish(domainEvent).ConfigureAwait(false);
          }
        }
      }
    }
    catch (Exception exception)
    {
      Debugger.Break();
      Debugger.Log(3, "Error in save changes", exception.Message);
      throw;
    }
  }

  private async Task RaiseDominaEventsUsingForLoop()
  {
    try
    {
      var changeTrackerEntries = ChangeTracker.Entries().ToList();

      for (int i = 0; i < changeTrackerEntries.Count(); i++)
      {
        var entry = changeTrackerEntries[i];
        if (entry.Entity.GetType().BaseType!.IsGenericType && entry.Entity.GetType().BaseType?.GetGenericTypeDefinition() == typeof(BaseEntity<>))
        {
          var baseEntity = entry.Entity;
          var eventsField = baseEntity.GetType().GetField(nameof(BaseEntity<int>.Events));
          var listOfDomainEvents = (List<BaseDomainEvent>)eventsField!.GetValue(baseEntity)!;
          if (listOfDomainEvents.Any())
          {
            foreach (var domainEvent in listOfDomainEvents)
            {
              await _mediator!.Publish(domainEvent).ConfigureAwait(false);
            }
          }
        }
      }
    }
    catch (Exception exception)
    {
      Debugger.Break();
      Debugger.Log(3, "Error in save changes", exception.Message);
      throw;
    }
  }
}
