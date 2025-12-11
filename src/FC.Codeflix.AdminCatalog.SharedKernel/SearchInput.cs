namespace FC.Codeflix.AdminCatalog.SharedKernel;

public record SearchInput(int Page, int PageSize, string Search, string Sort, SearchOrder Order);