using FC.Codeflix.AdminCatalog.Application.Categories.List;
using FC.Codeflix.AdminCatalog.Application.Shared;
using FC.Codeflix.AdminCatalog.Domain.Categories;
using FC.Codeflix.AdminCatalog.SharedKernel;
using Moq;
using Shouldly;

namespace FC.Codeflix.AdminCatalog.UnitTests.Application.Categories.List;

[Collection(nameof(ListCategoriesTestFixture))]
public class ListCategoriesQueryHandlerTest(ListCategoriesTestFixture fixture)
{
    [Fact]
    public async Task ShouldReturnAListOfCategoriesSortedByNameAscending()
    {
        // GIVEN
        var list = fixture.GetCategories();
        var repositoryMock = fixture.RepositoryMock(list);
        var query = new ListQuery(Search: "test", Sort: "name");
        
        // WHEN
        var handler = new ListCategoriesQueryHandler(repositoryMock.Object);
        var result = await handler.Handle(query);
        
        // THEN
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Page.ShouldBe(1);
        result.Value.PageSize.ShouldBe(10);
        result.Value.TotalItems.ShouldBe(10);
        result.Value.Items.ShouldNotBeNull();
        result.Value.Items.Count.ShouldBe(10);
        repositoryMock.Verify(
            repository => repository.ListAsync(
                It.Is<SearchInput>(input => input.Page == query.Page 
                                            && input.PageSize == query.PageSize
                                            && input.Search == query.Search
                                            && input.Sort == query.Sort
                                            && input.Order == SearchOrder.Asc
                ), 
                It.IsAny<CancellationToken>()
            ), 
            Times.Once
        );
    }
    
    [Fact]
    public async Task ShouldReturnAListOfCategoriesSortedByNameDescending()
    {
        // GIVEN
        var list = fixture.GetCategories();
        var repositoryMock = fixture.RepositoryMock(list);
        var query = new ListQuery(Search: "test", Sort: "-name");
        
        // WHEN
        var handler = new ListCategoriesQueryHandler(repositoryMock.Object);
        var result = await handler.Handle(query);
        
        // THEN
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Page.ShouldBe(1);
        result.Value.PageSize.ShouldBe(10);
        result.Value.TotalItems.ShouldBe(10);
        result.Value.Items.ShouldNotBeNull();
        result.Value.Items.Count.ShouldBe(10);
        repositoryMock.Verify(
            repository => repository.ListAsync(
                It.Is<SearchInput>(input => input.Page == query.Page 
                                            && input.PageSize == query.PageSize
                                            && input.Search == query.Search
                                            && input.Sort == query.Sort
                                            && input.Order == SearchOrder.Desc
                ), 
                It.IsAny<CancellationToken>()
            ), 
            Times.Once
        );
    }
    
    [Fact]
    public async Task ShouldReturnAListOfCategoriesWhenSearchIsNotProvided()
    {
        // GIVEN
        var list = fixture.GetCategories();
        var repositoryMock = fixture.RepositoryMock(list);
        var query = new ListQuery(Page: 2, PageSize: 5, Sort: "name");
        
        // WHEN
        var handler = new ListCategoriesQueryHandler(repositoryMock.Object);
        var result = await handler.Handle(query);
        
        // THEN
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Page.ShouldBe(1);
        result.Value.PageSize.ShouldBe(10);
        result.Value.TotalItems.ShouldBe(10);
        result.Value.Items.ShouldNotBeNull();
        result.Value.Items.Count.ShouldBe(10);
        repositoryMock.Verify(
            repository => repository.ListAsync(
                It.Is<SearchInput>(input => input.Page == query.Page
                                            && input.PageSize == query.PageSize
                                            && input.Search == query.Search
                                            && input.Sort == query.Sort
                                            && input.Order == SearchOrder.Asc
                ), 
                It.IsAny<CancellationToken>()
            ), 
            Times.Once
        );
    }
    
    [Fact]
    public async Task ShouldReturnEmptyListOfCategories()
    {
        // GIVEN
        var repositoryMock = fixture.RepositoryMock(new List<Category>());
        var query = new ListQuery(Search: "test", Sort: "name");
        
        // WHEN
        var handler = new ListCategoriesQueryHandler(repositoryMock.Object);
        var result = await handler.Handle(query);
        
        // THEN
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Page.ShouldBe(1);
        result.Value.PageSize.ShouldBe(0);
        result.Value.TotalItems.ShouldBe(0);
        result.Value.Items.ShouldNotBeNull();
        result.Value.Items.Count.ShouldBe(0);
        repositoryMock.Verify(
            repository => repository.ListAsync(
                It.Is<SearchInput>(input => input.Page == query.Page 
                                            && input.PageSize == query.PageSize
                                            && input.Search == query.Search
                                            && input.Sort == query.Sort
                                            && input.Order == SearchOrder.Asc
                ), 
                It.IsAny<CancellationToken>()
            ), 
            Times.Once
        );
    }
}