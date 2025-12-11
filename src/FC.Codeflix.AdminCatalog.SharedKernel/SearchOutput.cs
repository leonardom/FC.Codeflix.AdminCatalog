namespace FC.Codeflix.AdminCatalog.SharedKernel;

public class SearchOutput<T>(int page, int pageSize, IReadOnlyList<T> items, int totalItems)
{
    public int Page { get; } = page;
    public int PageSize { get; } = pageSize;
    public int TotalItems { get;  } = totalItems;
    public IReadOnlyList<T> Items { get; } = items;
}