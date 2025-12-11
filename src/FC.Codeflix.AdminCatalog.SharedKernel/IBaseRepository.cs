namespace FC.Codeflix.AdminCatalog.SharedKernel;

public interface IBaseRepository<TAggregate> 
    where TAggregate : AggregateRoot
{
    public Task<Result> CreateAsync(TAggregate aggregateRoot, CancellationToken cancellationToken = default);
    public Task<Result<TAggregate>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    public Task<Result> UpdateAsync(TAggregate aggregateRoot, CancellationToken cancellationToken = default);
    public Task<Result<SearchOutput<TAggregate>>> ListAsync(SearchInput input, CancellationToken cancellationToken = default);
}