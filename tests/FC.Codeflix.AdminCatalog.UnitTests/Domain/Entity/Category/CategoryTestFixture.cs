using FC.Codeflix.AdminCatalog.UnitTests.Common;

namespace FC.Codeflix.AdminCatalog.UnitTests.Domain.Entity.Category;

using DomainEntity = FC.Codeflix.AdminCatalog.Domain.Entity;

public record CategoryInput(string Name, string Description, bool IsActive = true);

public class CategoryTestFixture : BaseFixture
{
    public string GenerateName(int minLength = 3, int maxLength = 255)
    {
        maxLength = minLength > maxLength ? minLength : maxLength;
        var name = Faker.Lorem.Sentence();
        while (name.Length < minLength)
        {
            name += Faker.Lorem.Sentence();
        }
        return name[..(name.Length > maxLength ? maxLength : name.Length)];
    }

    public string GenerateDescription(int minLength = 100, int maxLength = 10_000)
    {
        maxLength = minLength > maxLength ? minLength : maxLength;
        var description = Faker.Lorem.Text();
        while (description.Length < minLength)
        {
            description += Faker.Lorem.Sentence();
        }
        return description[..(description.Length > maxLength ? maxLength : description.Length)];
    }
    
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