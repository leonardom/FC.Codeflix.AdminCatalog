using FC.Codeflix.AdminCatalog.Application.Categories.Update;
using FC.Codeflix.AdminCatalog.Domain.Categories;
using FC.Codeflix.AdminCatalog.SharedKernel;
using FC.Codeflix.AdminCatalog.UnitTests.Common;
using Moq;

namespace FC.Codeflix.AdminCatalog.UnitTests.Application.Categories.Update;

public class UpdateCategoryTestFixture : BaseFixture
{
    public static Mock<ICategoryRepository> RepositoryMock(Result<Category> category)
    {
        var repositoryMock = new Mock<ICategoryRepository>();
        repositoryMock
            .Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);
        repositoryMock
            .Setup(repository => repository.UpdateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());
        return repositoryMock;
    }

    public static Mock<IUnitOfWork> UnitOfWorkMock() => new();
    
    public Result<Category> GetCategory()
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
    
    public UpdateCategoryCommand GetValidCommand() 
        => new( Guid.CreateVersion7(), GenerateName(), GenerateDescription());
    
    public UpdateCategoryCommand GetInvalidCommand() 
        => new(Guid.CreateVersion7(), GenerateName(2, 2), GenerateDescription());
}

[CollectionDefinition(nameof(UpdateCategoryTestFixture))]
public class UpdateCategoryTestFixtureCollection : ICollectionFixture<UpdateCategoryTestFixture>;