using FC.Codeflix.AdminCatalog.Application.Categories.Shared;
using FC.Codeflix.AdminCatalog.Domain.Categories;
using FC.Codeflix.AdminCatalog.SharedKernel;

namespace FC.Codeflix.AdminCatalog.Application.Categories.Create;

public class CreateCategoryCommandHandler(IUnitOfWork unitOfWork, ICategoryRepository repository)
{
    public async Task<Result<CategoryResponse>> Handle(CreateCategoryCommand command, CancellationToken cancellationToken = default)
    {
        var categoryResult = Category.Create(command.Name, command.Description);
        if (categoryResult.IsFailure) return Result.Failure<CategoryResponse>(categoryResult.Error);
        
        var createResult = await repository.CreateAsync(categoryResult.Value, cancellationToken);
        if (createResult.IsFailure) return Result.Failure<CategoryResponse>(createResult.Error);
        
        await unitOfWork.CommitAsync(cancellationToken);
        
        var category = categoryResult.Value;
        return Result.Success(CategoryResponse.From(category));
    }
}