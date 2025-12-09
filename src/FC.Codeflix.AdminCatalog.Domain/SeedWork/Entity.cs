namespace FC.Codeflix.AdminCatalog.Domain.SeedWork;

public abstract class Entity
{
    public Guid Id { get; } = Guid.CreateVersion7();
}