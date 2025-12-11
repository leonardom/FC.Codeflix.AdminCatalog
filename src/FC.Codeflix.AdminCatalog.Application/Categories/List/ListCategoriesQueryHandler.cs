using FC.Codeflix.AdminCatalog.Application.Categories.Shared;
using FC.Codeflix.AdminCatalog.Application.Shared;
using FC.Codeflix.AdminCatalog.Domain.Categories;
using FC.Codeflix.AdminCatalog.SharedKernel;

namespace FC.Codeflix.AdminCatalog.Application.Categories.List;

public class ListCategoriesQueryHandler(ICategoryRepository repository)
{
    public async Task<Result<ListQueryResult<CategoryResponse>>> Handle(ListQuery query, CancellationToken cancellationToken = default)
    {
        var result = await repository.ListAsync(query.ToSearchInput(), cancellationToken);
        if (result.IsFailure) return Result.Failure<ListQueryResult<CategoryResponse>>(result.Error);
        var output = result.Value;
        var categoryResponses = output.Items
            .Select(CategoryResponse.From)
            .ToList();
        var response = new ListQueryResult<CategoryResponse>(
            output.Page,
            output.PageSize,
            output.TotalItems,
            categoryResponses
        );
        return Result.Success(response);
    }
}