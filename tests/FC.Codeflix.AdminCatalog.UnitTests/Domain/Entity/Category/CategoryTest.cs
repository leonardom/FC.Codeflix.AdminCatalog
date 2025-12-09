using FluentAssertions;

namespace FC.Codeflix.AdminCatalog.UnitTests.Domain.Entity.Category;

using DomainEntity = FC.Codeflix.AdminCatalog.Domain.Entity;

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
        sut.IsSuccess.Should().BeTrue();
        sut.Value.Should().NotBeNull();
        sut.Value.Name.Should().Be(input.Name);
        sut.Value.Description.Should().Be(input.Description);
        sut.Value.Id.Should().NotBeEmpty();
        sut.Value.CreatedAt.Should().NotBe(default);
        sut.Value.IsActive.Should().BeTrue();
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
        sut.IsSuccess.Should().BeTrue();
        sut.Value.Should().NotBeNull();
        sut.Value.Name.Should().Be(input.Name);
        sut.Value.Description.Should().Be(input.Description);
        sut.Value.Id.Should().NotBeEmpty();
        sut.Value.CreatedAt.Should().NotBe(default);
        sut.Value.IsActive.Should().Be(isActive);
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
        sut.IsFailure.Should().BeTrue();
        sut.Error.Should().Be("Name must not be blank");
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
        sut.IsFailure.Should().BeTrue();
        sut.Error.Should().Be("Name must contain at least 3 characters");
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
        sut.IsFailure.Should().BeTrue();
        sut.Error.Should().Be("Name must contain at most 255 characters");
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
        sut.IsFailure.Should().BeTrue();
        sut.Error.Should().Be("Description must not be blank");
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
        sut.IsFailure.Should().BeTrue();
        sut.Error.Should().Be("Description must contain at most 10000 characters");
    }

    [Fact]
    [Trait("Domain", "Category Aggregate")]
    public void ShouldActivateCategory()
    {
        // GIVEN
        var category = fixture.GetInactiveCategory();
        category.IsActive.Should().BeFalse();
        category.UpdatedAt.Should().Be(default);

        // WHEN
        var result = category.Activate();
        
        // THEN
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        var updatedCategory = result.Value;
        updatedCategory.IsActive.Should().BeTrue();
        updatedCategory.UpdatedAt.Should().NotBe(default);
        updatedCategory.Name.Should().Be(category.Name);
        updatedCategory.Description.Should().Be(category.Description);
    }
    
    [Fact]
    [Trait("Domain", "Category Aggregate")]
    public void ShouldDeactivateCategory()
    {
        // GIVEN
        var category = fixture.GetActiveCategory();
        category.IsActive.Should().BeTrue();
        category.UpdatedAt.Should().Be(default);
        
        // WHEN
        var result = category.Deactivate();
        
        // THEN
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        var updatedCategory = result.Value;
        updatedCategory.IsActive.Should().BeFalse();
        updatedCategory.UpdatedAt.Should().NotBe(default);
        updatedCategory.Name.Should().Be(category.Name);
        updatedCategory.Description.Should().Be(category.Description);
    }

    [Fact]
    [Trait("Domain", "Category Aggregate")]
    public void ShouldUpdateCategoryName()
    {
        // GIVEN
        var category = fixture.GetActiveCategory();
        category.UpdatedAt.Should().Be(default);
        var name = fixture.GenerateName();
        
        // WHEN
        var result = category.Update(name: name);
        
        // THEN
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        var updatedCategory = result.Value;
        updatedCategory.Name.Should().Be(name);
        updatedCategory.Description.Should().Be(category.Description);
        updatedCategory.UpdatedAt.Should().NotBe(default);
    }
    
    [Theory]
    [Trait("Domain", "Category Aggregate")]
    [InlineData("")]
    [InlineData("  ")]
    public void ShouldNotUpdateCategoryNameWhenNameIsBlank(string? invalidName)
    {
        // GIVEN
        var category = fixture.GetActiveCategory();
        category.UpdatedAt.Should().Be(default);
        
        // WHEN
        var result = category.Update(invalidName);
        
        // THEN
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Name must not be blank");
    }
    
    [Theory]
    [Trait("Domain", "Category Aggregate")]
    [MemberData(nameof(GenerateInvalidNames), parameters: 5)]
    public void ShouldNotUpdateCategoryNameWhenNameIsLessThan3Characters(string? invalidName)
    {
        // GIVEN
        var category = fixture.GetActiveCategory();
        category.UpdatedAt.Should().Be(default);
        
        // WHEN
        var result = category.Update(invalidName);
        
        // THEN
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Name must contain at least 3 characters");
    }
    
    [Fact]
    [Trait("Domain", "Category Aggregate")]
    public void ShouldNotUpdateCategoryNameWhenNameIsGreaterThan255Characters()
    {
        // GIVEN
        var category = fixture.GetActiveCategory();
        category.UpdatedAt.Should().Be(default);
        var invalidName = fixture.GenerateName(minLength: 256);
        
        // WHEN
        var result = category.Update(name: invalidName);
        
        // THEN
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Name must contain at most 255 characters");
    }
    
    [Fact]
    [Trait("Domain", "Category Aggregate")]
    public void ShouldUpdateCategoryDescription()
    {
        // GIVEN
        var category = fixture.GetActiveCategory();
        category.UpdatedAt.Should().Be(default);
        var description = fixture.GenerateDescription();
        
        // WHEN
        var result = category.Update(description: description);
        
        // THEN
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        var updatedCategory = result.Value;
        updatedCategory.Description.Should().Be(description);
        updatedCategory.Name.Should().Be(category.Name);
        updatedCategory.UpdatedAt.Should().NotBe(default);
    }

    [Fact]
    [Trait("Domain", "Category Aggregate")]
    public void ShouldNotUpdateCategoryDescriptionWhenDescriptionIsGreaterThan10000Characters()
    {
        // GIVEN
        var category = fixture.GetActiveCategory();
        category.UpdatedAt.Should().Be(default);
        var invalidDescription = fixture.GenerateDescription(minLength: 10001);
        
        // WHEN
        var result = category.Update(description: invalidDescription);
        
        // THEN
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Description must contain at most 10000 characters");
    }
    
    [Fact]
    [Trait("Domain", "Category Aggregate")]
    public void ShouldUpdateCategoryNameAndDescription()
    {
        // GIVEN
        var category = fixture.GetActiveCategory();
        category.UpdatedAt.Should().Be(default);
        var name = fixture.GenerateName();
        var description = fixture.GenerateDescription();
        
        // WHEN
        var result = category.Update(name, description);
        
        // THEN
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        var updatedCategory = result.Value;
        updatedCategory.Name.Should().Be(name);
        updatedCategory.Description.Should().Be(description);
        updatedCategory.UpdatedAt.Should().NotBe(default);
    }
}