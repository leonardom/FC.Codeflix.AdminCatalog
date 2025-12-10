using FC.Codeflix.AdminCatalog.Domain.Categories;
using FC.Codeflix.AdminCatalog.SharedKernel;

namespace FC.Codeflix.AdminCatalog.Application.Categories.Update;

public class UpdateCategoryCommandHandler(IUnitOfWork unitOfWork, ICategoryRepository repository)
{
    public async Task<Result> Handle(UpdateCategoryCommand command, CancellationToken cancellationToken = default)
    {
        var categoryResult = await repository.GetByIdAsync(command.Id, cancellationToken);
        if (categoryResult.IsFailure) return categoryResult;
        
        var category = categoryResult.Value;
        var updatedCategoryResult = category.Update(command.Name, command.Description);
        if (updatedCategoryResult.IsFailure) return updatedCategoryResult;
        
        var updateResult = await repository.UpdateAsync(updatedCategoryResult.Value, cancellationToken);
        if (updateResult.IsFailure) return updateResult;
        
        await unitOfWork.CommitAsync(cancellationToken);
        return Result.Success();
    }
}