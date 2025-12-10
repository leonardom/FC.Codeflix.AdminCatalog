using FC.Codeflix.AdminCatalog.Domain.Categories;

namespace FC.Codeflix.AdminCatalog.Application.Categories.Shared;

public record CategoryResponse(
    Guid Id,
    string Name,
    string Description,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt)
{
    public static CategoryResponse From(Category category)
        => new(category.Id, category.Name, category.Description, category.IsActive, category.CreatedAt, category.UpdatedAt);
}