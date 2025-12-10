using FC.Codeflix.AdminCatalog.Application.Categories.Create;
using FC.Codeflix.AdminCatalog.Domain.Categories;
using FC.Codeflix.AdminCatalog.SharedKernel;
using FC.Codeflix.AdminCatalog.UnitTests.Common;
using Moq;

namespace FC.Codeflix.AdminCatalog.UnitTests.Application.Categories.Create;


public class CreateCategoryTestFixture : BaseFixture
{
    public static Mock<ICategoryRepository> RepositoryMock()
    {
        var repositoryMock = new Mock<ICategoryRepository>();
        repositoryMock
            .Setup(repository => repository.CreateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());
        return repositoryMock;
    }

    public static Mock<IUnitOfWork> UnitOfWorkMock() => new();
    
    public CreateCategoryCommand GetValidCommand() => new(GenerateName(), GenerateDescription());
    
    public CreateCategoryCommand GetInvalidCommand() => new(GenerateName(2, 2), GenerateDescription());
}

[CollectionDefinition(nameof(CreateCategoryTestFixture))]
public class CreateCategoryTestFixtureCollection : ICollectionFixture<CreateCategoryTestFixture>;