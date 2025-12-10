namespace FC.Codeflix.AdminCatalog.SharedKernel;

public interface IUnitOfWork
{
    public Task CommitAsync(CancellationToken cancellationToken);
}