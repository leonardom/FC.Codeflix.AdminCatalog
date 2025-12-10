using FC.Codeflix.AdminCatalog.Application.Categories.Get;
using FC.Codeflix.AdminCatalog.Domain.Categories;
using FC.Codeflix.AdminCatalog.SharedKernel;
using Moq;
using Shouldly;

namespace FC.Codeflix.AdminCatalog.UnitTests.Application.Categories.Get;

[Collection(nameof(GetCategoryTestFixture))]
public class GetCategoryQueryHandlerTest(GetCategoryTestFixture fixture)
{
    [Fact]
    public async Task ShouldReturnAnExistingCategory()
    {
        // GIVEN
        var resultCategory = fixture.GetActiveCategory();
        var repositoryMock = fixture.RepositoryMock(resultCategory);
        var query = new GetCategoryQuery(Guid.CreateVersion7());
        
        // WHEN
        var handler = new GetCategoryQueryHandler(repositoryMock.Object);
        var result = await handler.Handle(query);
        
        // THEN
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        var response = result.Value;
        response.Id.ShouldBe(resultCategory.Value.Id);
        response.Name.ShouldBe(resultCategory.Value.Name);
        response.Description.ShouldBe(resultCategory.Value.Description);
        response.IsActive.ShouldBe(resultCategory.Value.IsActive);
        response.CreatedAt.ShouldBe(resultCategory.Value.CreatedAt);
        response.UpdatedAt.ShouldBe(resultCategory.Value.UpdatedAt);
        repositoryMock.Verify(
            repository => repository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), 
            Times.Once
        );
    }
    
    [Fact]
    public async Task ShouldReturnErrorWhenCategoryNotFound()
    {
        // GIVEN
        var resultCategory = fixture.GetActiveCategory();
        var repositoryMock = fixture.RepositoryMock(resultCategory);
        var id = Guid.CreateVersion7(); 
        repositoryMock
            .Setup(repository => repository.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => Result.Failure<Category>($"Category (id: {id}) not found"));
        var query = new GetCategoryQuery(id);
        
        // WHEN
        var handler = new GetCategoryQueryHandler(repositoryMock.Object);
        var result = await handler.Handle(query);
        
        // THEN
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe($"Category (id: {id}) not found");
        repositoryMock.Verify(
            repository => repository.GetByIdAsync(id, It.IsAny<CancellationToken>()), 
            Times.Once
        );
    }
}