using Ardalis.EFCore.Extensions;
using ProjectManagement.Core.ProjectAggregate;
using ProjectManagement.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ProjectManagement.Infrastructure.Data;

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

    modelBuilder.ApplyAllConfigurationsFromCurrentAssembly();

    // alternately this is built-in to EF Core 2.2
    //modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }

  public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
  {
    int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

    // ignore events if no dispatcher provided
    if (_mediator == null) return result;

    // dispatch events only if save was successful
    var baseEntities = ChangeTracker.Entries().Where(entry => entry.Entity.GetType().BaseType?.GetGenericTypeDefinition() == typeof(BaseEntity<>))
      .Select(e => e.Entity).ToList();

    foreach (var baseEntity in baseEntities)
    {
      var eventsField = baseEntity.GetType().GetField(nameof(BaseEntity<int>.Events));
      var listOfDomainEvents = (List<BaseDomainEvent>)eventsField!.GetValue(baseEntity)!;
      if (listOfDomainEvents.Any())
      {
        foreach (var domainEvent in listOfDomainEvents)
        {
          await _mediator.Publish(domainEvent).ConfigureAwait(false);
        }
      }
    }

    return result;
  }

  public override int SaveChanges()
  {
    return SaveChangesAsync().GetAwaiter().GetResult();
  }
}
