namespace FC.Codeflix.AdminCatalog.Application.Shared;

public record ListQueryResult<T>(int Page, int PageSize, int TotalItems, IReadOnlyList<T> Items);