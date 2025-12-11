namespace FC.Codeflix.AdminCatalog.SharedKernel;

public static class SearchOrderExtensions
{
    public static SearchOrder ToSearchOrder(this string str) 
        => !string.IsNullOrWhiteSpace(str) && str.StartsWith('-')
            ? SearchOrder.Desc
            : SearchOrder.Asc;
}