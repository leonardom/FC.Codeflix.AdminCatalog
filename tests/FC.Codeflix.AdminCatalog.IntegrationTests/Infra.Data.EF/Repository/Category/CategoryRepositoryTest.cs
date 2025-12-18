using FC.Codeflix.AdminCatalog.Infra.Data.EF.Entities;
using FC.Codeflix.AdminCatalog.SharedKernel;
using Shouldly;

namespace FC.Codeflix.AdminCatalog.IntegrationTests.Infra.Data.EF.Repository.Category;

[Collection(nameof(CategoryRepositoryFixture))]
public class CategoryRepositoryTest(CategoryRepositoryFixture fixture)
    : IDisposable
{
    [Fact]
    public async Task ShouldCreateCategory()
    {
        // GIVEN
        var dbContext = fixture.CreateDbContext();
        var category = fixture.CreateCategory();
        
        // WHEN
        var repository = new CategoryRepository(dbContext);
        await repository.CreateAsync(category, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        // THEN
        var createdCategory = await fixture.CreateDbContext().Categories.FindAsync(category.Id);
        createdCategory.ShouldNotBeNull();
        createdCategory.Id.ShouldBe(category.Id);
        createdCategory.Name.ShouldBe(category.Name);
        createdCategory.Description.ShouldBe(category.Description);
        createdCategory.IsActive.ShouldBe(category.IsActive);
        createdCategory.CreatedAt.ShouldBe(category.CreatedAt);
    }
    
    [Fact]
    public async Task ShouldFindAnExistingCategory()
    {
        // GIVEN
        var dbContext = fixture.CreateDbContext();
        var categories = fixture.CreateCategoryList();
        await dbContext.Categories.AddRangeAsync(categories);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        
        // WHEN
        var repository = new CategoryRepository(fixture.CreateDbContext());
        var result = await repository.GetByIdAsync(categories[0].Id, CancellationToken.None);

        // THEN
        result.IsSuccess.ShouldBeTrue();
        var foundCategory = result.Value;
        foundCategory.ShouldNotBeNull();
        foundCategory.Id.ShouldBe(categories[0].Id);
        foundCategory.Name.ShouldBe(categories[0].Name);
        foundCategory.Description.ShouldBe(categories[0].Description);
        foundCategory.IsActive.ShouldBe(categories[0].IsActive);
        foundCategory.CreatedAt.ShouldBe(categories[0].CreatedAt);
        foundCategory.UpdatedAt.ShouldBe(categories[0].UpdatedAt);
    }
    
    [Fact]
    public async Task ShouldReturnErrorWhenTryingToFindNonExistingCategory()
    {
        // GIVEN
        var dbContext = fixture.CreateDbContext();
        var categories = fixture.CreateCategoryList();
        await dbContext.Categories.AddRangeAsync(categories);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        
        // WHEN
        var id = Guid.NewGuid();
        var repository = new CategoryRepository(fixture.CreateDbContext());
        var result = await repository.GetByIdAsync(id, CancellationToken.None);

        // THEN
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe($"Category (id: {id}) not found");
    }
    
    [Fact]
    public async Task ShouldUpdateAnExistingCategory()
    {
        // GIVEN
        var dbContext = fixture.CreateDbContext();
        var categories = fixture.CreateCategoryList();
        await dbContext.Categories.AddRangeAsync(categories);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var category = categories[0].Update("Name updated").Value;
        
        // WHEN
        var repository = new CategoryRepository(dbContext);
        var updateResult = await repository.UpdateAsync(category, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        // THEN
        updateResult.IsSuccess.ShouldBeTrue();
        var updatedCategory = await fixture.CreateDbContext().Categories.FindAsync(category.Id);
        updatedCategory.ShouldNotBeNull();
        updatedCategory.Id.ShouldBe(category.Id);
        updatedCategory.Name.ShouldBe("Name updated");
        updatedCategory.Description.ShouldBe(category.Description);
        updatedCategory.IsActive.ShouldBe(category.IsActive);
        updatedCategory.CreatedAt.ShouldBe(category.CreatedAt);
        updatedCategory.UpdatedAt.ShouldBe(category.UpdatedAt);
    }

    [Fact]
    public async Task ShouldReturnErrorWhenTryingToUpdateNonExistingCategory()
    {
        // GIVEN
        var dbContext = fixture.CreateDbContext();
        
        // WHEN
        var category = fixture.CreateCategory();
        var repository = new CategoryRepository(dbContext);
        var result = await repository.UpdateAsync(category, CancellationToken.None);

        // THEN
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe($"Category (id: {category.Id}) not found");
    }
    
    [Fact]
    public async Task ShouldReturnAListOfCategories()
    {
        // GIVEN
        var dbContext = fixture.CreateDbContext();
        var categories = fixture.CreateCategoryList();
        await dbContext.Categories.AddRangeAsync(categories);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var input = new SearchInput(Page: 1, PageSize: 10, Search: "", Sort: "", SearchOrder.Asc);
        
        // WHEN
        var repository = new CategoryRepository(fixture.CreateDbContext());
        var result = await repository.ListAsync(input);

        // THEN
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        var output = result.Value;
        output.ShouldNotBeNull();
        output.Items.Count.ShouldBe(input.PageSize);
        output.Page.ShouldBe(input.Page);
        output.PageSize.ShouldBe(input.PageSize);
        output.TotalItems.ShouldBe(categories.Count);
        foreach (var item in output.Items)
        {
            var category = categories.First(x => x.Id == item.Id);
            category.ShouldNotBeNull();
            item.Id.ShouldBe(category.Id);
            item.Name.ShouldBe(category.Name);
            item.Description.ShouldBe(category.Description);
            item.IsActive.ShouldBe(category.IsActive);
            item.CreatedAt.ShouldBe(category.CreatedAt);
            item.UpdatedAt.ShouldBe(category.UpdatedAt);
        }
    }
    
    [Fact]
    public async Task ShouldReturnAEmptyListWhenThereAreNoCategories()
    {
        // GIVEN
        var input = new SearchInput(Page: 1, PageSize: 10, Search: "", Sort: "", SearchOrder.Asc);
        
        // WHEN
        var repository = new CategoryRepository(fixture.CreateDbContext());
        var result = await repository.ListAsync(input);

        // THEN
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        var output = result.Value;
        output.ShouldNotBeNull();
        output.Items.Count.ShouldBe(0);
        output.Page.ShouldBe(input.Page);
        output.PageSize.ShouldBe(input.PageSize);
    }

    [Theory]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task ShouldReturnAListOfCategoriesPaginated(int quantity, 
        int page, int pageSize, int expectedItems)
    {
        // GIVEN
        var dbContext = fixture.CreateDbContext();
        var categories = fixture.CreateCategoryList(quantity);
        await dbContext.Categories.AddRangeAsync(categories);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var input = new SearchInput(Page: page, PageSize: pageSize, Search: "", Sort: "", SearchOrder.Asc);
        
        // WHEN
        var repository = new CategoryRepository(fixture.CreateDbContext());
        var result = await repository.ListAsync(input);

        // THEN
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        var output = result.Value;
        output.ShouldNotBeNull();
        output.Items.Count.ShouldBe(expectedItems);
        output.Page.ShouldBe(input.Page);
        output.PageSize.ShouldBe(input.PageSize);
        output.TotalItems.ShouldBe(categories.Count);
        foreach (var item in output.Items)
        {
            var category = categories.First(x => x.Id == item.Id);
            category.ShouldNotBeNull();
            item.Id.ShouldBe(category.Id);
            item.Name.ShouldBe(category.Name);
            item.Description.ShouldBe(category.Description);
            item.IsActive.ShouldBe(category.IsActive);
            item.CreatedAt.ShouldBe(category.CreatedAt);
            item.UpdatedAt.ShouldBe(category.UpdatedAt);
        }
    }
    
    [Theory]
    [InlineData("Action", 1, 5, 1, 1)]
    [InlineData("Horror", 1, 5, 2, 2)]
    [InlineData("Horror", 2, 5, 0, 2)]
    [InlineData("Sci-fi", 1, 5, 3, 3)]
    [InlineData("Sci-fi", 1, 2, 2, 3)]
    [InlineData("Sci-fi", 2, 2, 1, 3)]
    [InlineData("Comedy", 1, 5, 0, 0)]
    [InlineData("fi", 1, 5, 3, 3)]
    public async Task ShouldReturnAListOfCategoriesFiltered(string search, 
        int page, int pageSize, int expectedReturnedItems, int expectedTotalItems)
    {
        // GIVEN
        var dbContext = fixture.CreateDbContext();
        var categories = fixture.CreateCategoryList([
            "Action", "Horror", "Horror Real", "Sci-fi AI", "Sci-fi Space", "Sci-fi Robots"
        ]);
        await dbContext.Categories.AddRangeAsync(categories);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var input = new SearchInput(Page: page, PageSize: pageSize, Search: search, Sort: "", SearchOrder.Asc);
        
        // WHEN
        var repository = new CategoryRepository(fixture.CreateDbContext());
        var result = await repository.ListAsync(input);

        // THEN
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        var output = result.Value;
        output.ShouldNotBeNull();
        output.Items.Count.ShouldBe(expectedReturnedItems);
        output.Page.ShouldBe(input.Page);
        output.PageSize.ShouldBe(input.PageSize);
        output.TotalItems.ShouldBe(expectedTotalItems);
        foreach (var item in output.Items)
        {
            var category = categories.First(x => x.Id == item.Id);
            category.ShouldNotBeNull();
            item.Id.ShouldBe(category.Id);
            item.Name.ShouldBe(category.Name);
            item.Description.ShouldBe(category.Description);
            item.IsActive.ShouldBe(category.IsActive);
            item.CreatedAt.ShouldBe(category.CreatedAt);
            item.UpdatedAt.ShouldBe(category.UpdatedAt);
        }
    }
    
    [Theory]
    [InlineData("name", "asc")]
    [InlineData("name", "desc")]
    [InlineData("id", "asc")]
    [InlineData("id", "desc")]
    [InlineData("createdAt", "asc")]
    [InlineData("createdAt", "desc")]
    public async Task ShouldReturnAListOfCategoriesSorted(string sortBy, string order)
    {
        // GIVEN
        var dbContext = fixture.CreateDbContext();
        var categories = fixture.CreateCategoryList();
        await dbContext.Categories.AddRangeAsync(categories);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var searchOrder = order == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
        var input = new SearchInput(Page: 1, PageSize: 10, Search: "", Sort: sortBy, searchOrder);
        
        // WHEN
        var repository = new CategoryRepository(fixture.CreateDbContext());
        var result = await repository.ListAsync(input);

        // THEN
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        var output = result.Value;
        output.ShouldNotBeNull();
        output.Items.Count.ShouldBe(categories.Count);
        output.Page.ShouldBe(input.Page);
        output.PageSize.ShouldBe(input.PageSize);
        output.TotalItems.ShouldBe(categories.Count);
        var expectedItems = fixture.OrderCategoryList(categories, sortBy, searchOrder);
        for (var i = 0; i < expectedItems.Count; i++)
        {
            output.Items[i].Name.ShouldBe(expectedItems[i].Name);
            output.Items[i].Description.ShouldBe(expectedItems[i].Description);
            output.Items[i].Id.ShouldBe(expectedItems[i].Id);
            output.Items[i].IsActive.ShouldBe(expectedItems[i].IsActive);
            output.Items[i].CreatedAt.ShouldBe(expectedItems[i].CreatedAt);
            output.Items[i].UpdatedAt.ShouldBe(expectedItems[i].UpdatedAt);
        }
    }

    void IDisposable.Dispose()
    {
        GC.SuppressFinalize(this);
        fixture.ClearDatabase();
    }
}