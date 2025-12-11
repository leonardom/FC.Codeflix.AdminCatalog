using FC.Codeflix.AdminCatalog.SharedKernel;

namespace FC.Codeflix.AdminCatalog.Application.Shared;

public record ListQuery(string Search = "", string Sort = "", int Page = 1, int PageSize = 10)
{
    public SearchInput ToSearchInput()
        => new (Page, PageSize, Search, Sort, Sort.ToSearchOrder());
}