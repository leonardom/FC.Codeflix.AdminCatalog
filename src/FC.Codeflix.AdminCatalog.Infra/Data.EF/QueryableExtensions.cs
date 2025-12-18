using FC.Codeflix.AdminCatalog.SharedKernel;

namespace FC.Codeflix.AdminCatalog.Infra.Data.EF;

using System.Linq.Expressions;

public static class QueryableExtensions
{
    public static IQueryable<T> OrderByPropertyName<T>(this IQueryable<T> source, string propertyName, SearchOrder order)
    {
        if (string.IsNullOrWhiteSpace(propertyName)) return source;

        // 1. Get the property info
        var propertyInfo = typeof(T).GetProperty(propertyName, 
            System.Reflection.BindingFlags.IgnoreCase | 
            System.Reflection.BindingFlags.Public | 
            System.Reflection.BindingFlags.Instance);

        if (propertyInfo == null) return source;

        // 2. Create the expression: x => x.PropertyName
        var parameter = Expression.Parameter(typeof(T), "x");
        var propertyAccess = Expression.MakeMemberAccess(parameter, propertyInfo);
        var lambda = Expression.Lambda(propertyAccess, parameter);

        // 3. Choose the Method (OrderBy or OrderByDescending)
        string methodName = order == SearchOrder.Desc ? "OrderByDescending" : "OrderBy";
        
        var resultExpression = Expression.Call(
            typeof(Queryable), 
            methodName,
            [typeof(T), propertyInfo.PropertyType],
            source.Expression, 
            Expression.Quote(lambda));

        return source.Provider.CreateQuery<T>(resultExpression);
    }
}