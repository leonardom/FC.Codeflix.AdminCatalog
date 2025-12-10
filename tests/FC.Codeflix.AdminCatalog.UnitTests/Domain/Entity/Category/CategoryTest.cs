using Shouldly;

namespace FC.Codeflix.AdminCatalog.UnitTests.Domain.Entity.Category;

using DomainEntity = FC.Codeflix.AdminCatalog.Domain.Categories;

[Collection(nameof(CategoryTestFixture))]
public class CategoryTest(CategoryTestFixture fixture)
{
    
    public static IEnumerable<object[]> GenerateInvalidNames(int numberOfInstances)
    {
        var fixture = new CategoryTestFixture();
        for (var i = 0; i < numberOfInstances; i++)
        {
            yield return [ 
                fixture.Faker.Random.AlphaNumeric(i % 2 == 1 ? 1 : 2) 
            ];
        }
    }

    [Fact]
    [Trait("Domain", "Category Aggregate")]
    public void ShouldCreateAnActiveCategoryByDefault()
    {
        // GIVEN
        var input = fixture.GetInput();
        
        // WHEN
        var sut = DomainEntity.Category.Create(input.Name, input.Description);
        
        // THEN
        sut.IsSuccess.ShouldBeTrue();
        sut.Value.ShouldNotBeNull();
        sut.Value.Name.ShouldBe(input.Name);
        sut.Value.Description.ShouldBe(input.Description);
        sut.Value.Id.ShouldNotBe(Guid.Empty); 
        sut.Value.CreatedAt.ShouldNotBe(default);
        sut.Value.IsActive.ShouldBeTrue();
    }

    [Theory]
    [Trait("Domain", "Category Aggregate")]
    [InlineData(true)]
    [InlineData(false)]
    public void ShouldCreateCategoryWithIsActive(bool isActive)
    {
        // GIVEN
        var input = fixture.GetInput();
        
        // WHEN
        var sut = DomainEntity.Category.Create(input.Name, input.Description, isActive);
        
        // THEN
        sut.IsSuccess.ShouldBeTrue();
        sut.Value.ShouldNotBeNull();
        sut.Value.Name.ShouldBe(input.Name);
        sut.Value.Description.ShouldBe(input.Description);
        sut.Value.Id.ShouldNotBe(Guid.Empty);
        sut.Value.CreatedAt.ShouldNotBe(default);
        sut.Value.IsActive.ShouldBe(isActive);
    }

    [Theory]
    [Trait("Domain", "Category Aggregate")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void ShouldNotCreateCategoryWithBlankName(string? name)
    {
        // GIVEN
        var input = fixture.GetInput();
        
        // WHEN
        var sut = DomainEntity.Category.Create(name!, input.Description);
        
        // THEN
        sut.IsFailure.ShouldBeTrue();
        sut.Error.ShouldBe("Name must not be blank");
    }
    
    [Theory]
    [Trait("Domain", "Category Aggregate")]
    [MemberData(nameof(GenerateInvalidNames), parameters: 5)]
    public void ShouldNotCreateCategoryWithNameLessThan3Characters(string? name)
    {
        // GIVEN
        var input = fixture.GetInput();
        
        // WHEN
        var sut = DomainEntity.Category.Create(name!, input.Description);
        
        // THEN
        sut.IsFailure.ShouldBeTrue();
        sut.Error.ShouldBe("Name must contain at least 3 characters");
    }

    [Fact]
    [Trait("Domain", "Category Aggregate")]
    public void ShouldNotCreateCategoryWithNameGreaterThan255Characters()
    {
        // GIVEN
        var input = fixture.GetInput();
        var name = fixture.GenerateName(minLength: 256);
        
        // WHEN
        var sut = DomainEntity.Category.Create(name, input.Description);
        
        // THEN
        sut.IsFailure.ShouldBeTrue();
        sut.Error.ShouldBe("Name must contain at most 255 characters");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void ShouldNotCreateCategoryWithBlankDescription(string? description)
    {
        // GIVEN
        var input = fixture.GetInput();
        
        // WHEN
        var sut = DomainEntity.Category.Create(input.Name, description!);
        
        // THEN
        sut.IsFailure.ShouldBeTrue();
        sut.Error.ShouldBe("Description must not be blank");
    }
    
    [Fact]
    [Trait("Domain", "Category Aggregate")]
    public void ShouldNotCreateCategoryWithDescriptionGreaterThan10000Characters()
    {
        // GIVEN
        var input = fixture.GetInput();
        var description = fixture.GenerateDescription(minLength: 10001);
        
        // WHEN
        var sut = DomainEntity.Category.Create(input.Name, description);
        
        // THEN
        sut.IsFailure.ShouldBeTrue();
        sut.Error.ShouldBe("Description must contain at most 10000 characters");
    }

    [Fact]
    [Trait("Domain", "Category Aggregate")]
    public void ShouldActivateCategory()
    {
        // GIVEN
        var category = fixture.GetInactiveCategory();
        category.IsActive.ShouldBeFalse();
        category.UpdatedAt.ShouldBe(default);

        // WHEN
        var result = category.Activate();
        
        // THEN
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        var updatedCategory = result.Value;
        updatedCategory.IsActive.ShouldBeTrue();
        updatedCategory.UpdatedAt.ShouldNotBe(default);
        updatedCategory.Name.ShouldBe(category.Name);
        updatedCategory.Description.ShouldBe(category.Description);
    }
    
    [Fact]
    [Trait("Domain", "Category Aggregate")]
    public void ShouldDeactivateCategory()
    {
        // GIVEN
        var category = fixture.GetActiveCategory();
        category.IsActive.ShouldBeTrue();
        category.UpdatedAt.ShouldBe(default);
        
        // WHEN
        var result = category.Deactivate();
        
        // THEN
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        var updatedCategory = result.Value;
        updatedCategory.IsActive.ShouldBeFalse();
        updatedCategory.UpdatedAt.ShouldNotBe(default);
        updatedCategory.Name.ShouldBe(category.Name);
        updatedCategory.Description.ShouldBe(category.Description);
    }

    [Fact]
    [Trait("Domain", "Category Aggregate")]
    public void ShouldUpdateCategoryName()
    {
        // GIVEN
        var category = fixture.GetActiveCategory();
        category.UpdatedAt.ShouldBe(default);
        var name = fixture.GenerateName();
        
        // WHEN
        var result = category.Update(name: name);
        
        // THEN
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        var updatedCategory = result.Value;
        updatedCategory.Name.ShouldBe(name);
        updatedCategory.Description.ShouldBe(category.Description);
        updatedCategory.UpdatedAt.ShouldNotBe(default);
    }
    
    [Theory]
    [Trait("Domain", "Category Aggregate")]
    [InlineData("")]
    [InlineData("  ")]
    public void ShouldNotUpdateCategoryNameWhenNameIsBlank(string? invalidName)
    {
        // GIVEN
        var category = fixture.GetActiveCategory();
        category.UpdatedAt.ShouldBe(default);
        
        // WHEN
        var result = category.Update(invalidName);
        
        // THEN
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("Name must not be blank");
    }
    
    [Theory]
    [Trait("Domain", "Category Aggregate")]
    [MemberData(nameof(GenerateInvalidNames), parameters: 5)]
    public void ShouldNotUpdateCategoryNameWhenNameIsLessThan3Characters(string? invalidName)
    {
        // GIVEN
        var category = fixture.GetActiveCategory();
        category.UpdatedAt.ShouldBe(default);
        
        // WHEN
        var result = category.Update(invalidName);
        
        // THEN
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("Name must contain at least 3 characters");
    }
    
    [Fact]
    [Trait("Domain", "Category Aggregate")]
    public void ShouldNotUpdateCategoryNameWhenNameIsGreaterThan255Characters()
    {
        // GIVEN
        var category = fixture.GetActiveCategory();
        category.UpdatedAt.ShouldBe(default);
        var invalidName = fixture.GenerateName(minLength: 256);
        
        // WHEN
        var result = category.Update(name: invalidName);
        
        // THEN
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("Name must contain at most 255 characters");
    }
    
    [Fact]
    [Trait("Domain", "Category Aggregate")]
    public void ShouldUpdateCategoryDescription()
    {
        // GIVEN
        var category = fixture.GetActiveCategory();
        category.UpdatedAt.ShouldBe(default);
        var description = fixture.GenerateDescription();
        
        // WHEN
        var result = category.Update(description: description);
        
        // THEN
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        var updatedCategory = result.Value;
        updatedCategory.Description.ShouldBe(description);
        updatedCategory.Name.ShouldBe(category.Name);
        updatedCategory.UpdatedAt.ShouldNotBe(default);
    }

    [Fact]
    [Trait("Domain", "Category Aggregate")]
    public void ShouldNotUpdateCategoryDescriptionWhenDescriptionIsGreaterThan10000Characters()
    {
        // GIVEN
        var category = fixture.GetActiveCategory();
        category.UpdatedAt.ShouldBe(default);
        var invalidDescription = fixture.GenerateDescription(minLength: 10001);
        
        // WHEN
        var result = category.Update(description: invalidDescription);
        
        // THEN
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("Description must contain at most 10000 characters");
    }
    
    [Fact]
    [Trait("Domain", "Category Aggregate")]
    public void ShouldUpdateCategoryNameAndDescription()
    {
        // GIVEN
        var category = fixture.GetActiveCategory();
        category.UpdatedAt.ShouldBe(default);
        var name = fixture.GenerateName();
        var description = fixture.GenerateDescription();
        
        // WHEN
        var result = category.Update(name, description);
        
        // THEN
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        var updatedCategory = result.Value;
        updatedCategory.Name.ShouldBe(name);
        updatedCategory.Description.ShouldBe(description);
        updatedCategory.UpdatedAt.ShouldNotBe(default);
    }
}