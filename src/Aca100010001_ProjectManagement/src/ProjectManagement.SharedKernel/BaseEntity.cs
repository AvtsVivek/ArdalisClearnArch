namespace ProjectManagement.SharedKernel;

// This can be modified to BaseEntity<TId> to support multiple key types (e.g. Guid)
public abstract class BaseEntity<Tid>
{
  public Tid Id { get; set; } = default!;

  public List<BaseDomainEvent> Events = new List<BaseDomainEvent>();
}
