namespace FC.Codeflix.AdminCatalog.SharedKernel;

public abstract class Entity
{
    public Guid Id { get; protected set; } = Guid.CreateVersion7();
}