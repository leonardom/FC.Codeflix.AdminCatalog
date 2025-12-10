using FC.Codeflix.AdminCatalog.Application.Categories.Shared;
using FC.Codeflix.AdminCatalog.Domain.Categories;
using FC.Codeflix.AdminCatalog.SharedKernel;

namespace FC.Codeflix.AdminCatalog.Application.Categories.Get;

public class GetCategoryQueryHandler(ICategoryRepository repository)
{
    public async Task<Result<CategoryResponse>> Handle(GetCategoryQuery query, CancellationToken cancellationToken = default)
    {
        var categoryResult = await repository.GetByIdAsync(query.Id, cancellationToken);
        if (categoryResult.IsFailure) return Result.Failure<CategoryResponse>(categoryResult.Error);
        var category = categoryResult.Value;
        return Result.Success(CategoryResponse.From(category));
    }
}