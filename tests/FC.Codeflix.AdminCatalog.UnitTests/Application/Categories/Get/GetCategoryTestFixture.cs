using FC.Codeflix.AdminCatalog.Domain.Categories;
using FC.Codeflix.AdminCatalog.SharedKernel;
using FC.Codeflix.AdminCatalog.UnitTests.Common;
using Moq;

namespace FC.Codeflix.AdminCatalog.UnitTests.Application.Categories.Get;

public class GetCategoryTestFixture : BaseFixture
{
    public Mock<ICategoryRepository> RepositoryMock(Result<Category> category)
    {
        var repositoryMock = new Mock<ICategoryRepository>();
        repositoryMock
            .Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);
        return repositoryMock;
    }

    public Result<Category> GetActiveCategory()
    {
        return Category.Restore(
            id: Guid.CreateVersion7(), 
            name: GenerateName(), 
            description: GenerateDescription(), 
            isActive: true, 
            createdAt: DateTime.UtcNow, 
            updatedAt: DateTime.UtcNow
        );
    }
}

[CollectionDefinition(nameof(GetCategoryTestFixture))]
public class GetCategoryTestFixtureCollection : ICollectionFixture<GetCategoryTestFixture>;