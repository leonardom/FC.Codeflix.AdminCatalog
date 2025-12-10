using FC.Codeflix.AdminCatalog.UnitTests.Common;

namespace FC.Codeflix.AdminCatalog.UnitTests.Domain.Entity.Category;

using DomainEntity = FC.Codeflix.AdminCatalog.Domain.Categories;

public record CategoryInput(string Name, string Description, bool IsActive = true);

public class CategoryTestFixture : BaseFixture
{
    public CategoryInput GetInput() =>
        new(GenerateName(), GenerateDescription());

    public DomainEntity.Category GetActiveCategory()
    {
        var input = GetInput();
        var result = DomainEntity.Category.Create(input.Name, input.Description);
        if (result.IsFailure || result.Value is null)
        {
            throw new InvalidOperationException(result.Error);
        }
        return result.Value;
    }
    
    public DomainEntity.Category GetInactiveCategory()
    {
        var input = GetInput();
        var result = DomainEntity.Category.Create(input.Name, input.Description, false);
        if (result.IsFailure || result.Value is null)
        {
            throw new InvalidOperationException(result.Error);
        }
        return result.Value;
    }
}

[CollectionDefinition(nameof(CategoryTestFixture))]
public class CategoryTestFixtureCollection : ICollectionFixture<CategoryTestFixture>;