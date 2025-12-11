using FC.Codeflix.AdminCatalog.Domain.Categories;
using FC.Codeflix.AdminCatalog.SharedKernel;
using FC.Codeflix.AdminCatalog.UnitTests.Common;
using Moq;
using Range = System.Range;

namespace FC.Codeflix.AdminCatalog.UnitTests.Application.Categories.List;

public class ListCategoriesTestFixture : BaseFixture
{
    public Mock<ICategoryRepository> RepositoryMock(IEnumerable<Category> categories)
    {
        var repositoryMock = new Mock<ICategoryRepository>();
        var list = categories.ToList();
        var output = new SearchOutput<Category>(page: 1, pageSize: list.Count, items: list, totalItems: list.Count);
        repositoryMock
            .Setup(repository => repository.ListAsync(
                It.IsAny<SearchInput>(), It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(Result.Success(output));
        return repositoryMock;
    }

    public IEnumerable<Category> GetCategories(int count = 10)
        => Enumerable.Range(0, count).Select(x => GetCategory()).ToList();
    
    public Category GetCategory()
        => Category.Restore(
            id: Guid.CreateVersion7(), 
            name: GenerateName(), 
            description: GenerateDescription(), 
            isActive: true, 
            createdAt: DateTime.UtcNow, 
            updatedAt: DateTime.UtcNow
        ).Value;
}

[CollectionDefinition(nameof(ListCategoriesTestFixture))]
public class ListCategoriesTestFixtureCollection : ICollectionFixture<ListCategoriesTestFixture>;