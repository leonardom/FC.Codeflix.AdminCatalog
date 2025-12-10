using FC.Codeflix.AdminCatalog.Application.Categories.Create;
using FC.Codeflix.AdminCatalog.Domain.Categories;
using FC.Codeflix.AdminCatalog.SharedKernel;
using Moq;
using Shouldly;

namespace FC.Codeflix.AdminCatalog.UnitTests.Application.Categories.Create;

[Collection(nameof(CreateCategoryTestFixture))]
public class CreateCategoryCommandHandlerTest(CreateCategoryTestFixture fixture)
{
    [Fact]
    public async Task ShouldCreateCategoryAndReturnResponse()
    {
        // GIVEN
        var repositoryMock = CreateCategoryTestFixture.RepositoryMock();
        var unitOfWorkMock = CreateCategoryTestFixture.UnitOfWorkMock();
        var command = fixture.GetValidCommand();
        
        // WHEN
        var handler = new CreateCategoryCommandHandler(unitOfWorkMock.Object, repositoryMock.Object);
        var result = await handler.Handle(command, CancellationToken.None); 
        
        // THEN
        result.IsSuccess.ShouldBeTrue();
        repositoryMock.Verify(
            repository => repository.CreateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), 
            Times.Once
        );
        unitOfWorkMock.Verify(unitOfWork
            => unitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.Once 
        );
        var response = result.Value;
        response.Id.ShouldNotBe(Guid.Empty);
        response.Name.ShouldBe(command.Name);
        response.Description.ShouldBe(command.Description);
        response.IsActive.ShouldBeTrue();
        response.CreatedAt.ShouldNotBe(default);
    }

    [Fact]
    public async Task ShouldReturnErrorWhenInvalidInputIsProvided()
    {
        // GIVEN
        var repositoryMock = CreateCategoryTestFixture.RepositoryMock();
        var unitOfWorkMock = CreateCategoryTestFixture.UnitOfWorkMock();
        var command = fixture.GetInvalidCommand();
        
        // WHEN
        var handler = new CreateCategoryCommandHandler(unitOfWorkMock.Object, repositoryMock.Object);
        var result = await handler.Handle(command, CancellationToken.None); 
        
        // THEN
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("Name must contain at least 3 characters");
        repositoryMock.Verify(
            repository => repository.CreateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), 
            Times.Never
        );
        unitOfWorkMock.Verify(unitOfWork
            => unitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.Never 
        );
    }
    
    [Fact]
    public async Task ShouldReturnErrorWhenRepositoryReturnsFailure()
    {
        // GIVEN
        var repositoryMock = CreateCategoryTestFixture.RepositoryMock();
        repositoryMock
            .Setup(repository => repository.CreateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure("Cannot create category"));
        var unitOfWorkMock = CreateCategoryTestFixture.UnitOfWorkMock();
        var command = fixture.GetValidCommand();
        
        // WHEN
        var handler = new CreateCategoryCommandHandler(unitOfWorkMock.Object, repositoryMock.Object);
        var result = await handler.Handle(command, CancellationToken.None); 
        
        // THEN
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("Cannot create category");
        repositoryMock.Verify(
            repository => repository.CreateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), 
            Times.Once
        );
        unitOfWorkMock.Verify(unitOfWork
            => unitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.Never 
        );
    }
}