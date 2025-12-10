using FC.Codeflix.AdminCatalog.Application.Categories.Update;
using FC.Codeflix.AdminCatalog.Domain.Categories;
using FC.Codeflix.AdminCatalog.SharedKernel;
using Moq;
using Shouldly;

namespace FC.Codeflix.AdminCatalog.UnitTests.Application.Categories.Update;

[Collection(nameof(UpdateCategoryTestFixture))]
public class UpdateCategoryCommandHandlerTest(UpdateCategoryTestFixture fixture)
{
    [Fact]
    public async Task ShouldUpdateCategory()
    {
        // GIVEN
        var categoryResult = fixture.GetCategory();
        var repositoryMock = UpdateCategoryTestFixture.RepositoryMock(categoryResult);
        var unitOfWorkMock = UpdateCategoryTestFixture.UnitOfWorkMock();
        var command = fixture.GetValidCommand();
        
        // WHEN
        var handler = new UpdateCategoryCommandHandler(unitOfWorkMock.Object, repositoryMock.Object);
        var result = await handler.Handle(command, CancellationToken.None); 
        
        // THEN
        result.IsSuccess.ShouldBeTrue();
        repositoryMock.Verify(
            repository => repository.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), 
            Times.Once
        );
        repositoryMock.Verify(
            repository => repository.UpdateAsync(
                It.Is<Category>(c =>
                    c.Name == command.Name && c.Description == command.Description
                ), 
                It.IsAny<CancellationToken>()
            ), 
            Times.Once
        );
        unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.CommitAsync(It.IsAny<CancellationToken>()), 
            Times.Once 
        );
    }
    
    [Fact]
    public async Task ShouldUpdateOnlyCategoryName()
    {
        // GIVEN
        var categoryResult = fixture.GetCategory();
        var repositoryMock = UpdateCategoryTestFixture.RepositoryMock(categoryResult);
        var unitOfWorkMock = UpdateCategoryTestFixture.UnitOfWorkMock();
        var command = fixture.GetValidCommand() with { Description = null };
        
        // WHEN
        var handler = new UpdateCategoryCommandHandler(unitOfWorkMock.Object, repositoryMock.Object);
        var result = await handler.Handle(command, CancellationToken.None); 
        
        // THEN
        result.IsSuccess.ShouldBeTrue();
        repositoryMock.Verify(
            repository => repository.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), 
            Times.Once
        );
        repositoryMock.Verify(
            repository => repository.UpdateAsync(
                It.Is<Category>(c =>
                    c.Name == command.Name && c.Description == categoryResult.Value.Description
                ), 
                It.IsAny<CancellationToken>()
            ), 
            Times.Once
        );
        unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.CommitAsync(It.IsAny<CancellationToken>()), 
            Times.Once 
        );
    }
    
    [Fact]
    public async Task ShouldUpdateOnlyCategoryDescription()
    {
        // GIVEN
        var categoryResult = fixture.GetCategory();
        var repositoryMock = UpdateCategoryTestFixture.RepositoryMock(categoryResult);
        var unitOfWorkMock = UpdateCategoryTestFixture.UnitOfWorkMock();
        var command = fixture.GetValidCommand() with { Name = null };
        
        // WHEN
        var handler = new UpdateCategoryCommandHandler(unitOfWorkMock.Object, repositoryMock.Object);
        var result = await handler.Handle(command, CancellationToken.None); 
        
        // THEN
        result.IsSuccess.ShouldBeTrue();
        repositoryMock.Verify(
            repository => repository.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), 
            Times.Once
        );
        repositoryMock.Verify(
            repository => repository.UpdateAsync(
                It.Is<Category>(c =>
                    c.Name == categoryResult.Value.Name && c.Description == command.Description
                ), 
                It.IsAny<CancellationToken>()
            ), 
            Times.Once
        );
        unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.CommitAsync(It.IsAny<CancellationToken>()), 
            Times.Once 
        );
    }
    
    [Fact]
    public async Task ShouldReturnErrorWhenInvalidInputIsProvided()
    {
        // GIVEN
        var categoryResult = fixture.GetCategory();
        var repositoryMock = UpdateCategoryTestFixture.RepositoryMock(categoryResult);
        var unitOfWorkMock = UpdateCategoryTestFixture.UnitOfWorkMock();
        var command = fixture.GetInvalidCommand();
        
        // WHEN
        var handler = new UpdateCategoryCommandHandler(unitOfWorkMock.Object, repositoryMock.Object);
        var result = await handler.Handle(command, CancellationToken.None); 
        
        // THEN
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldBe("Name must contain at least 3 characters");
        repositoryMock.Verify(
            repository => repository.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), 
            Times.Once
        );
        repositoryMock.Verify(
            repository => repository.UpdateAsync(
                It.IsAny<Category>(), 
                It.IsAny<CancellationToken>()
            ), 
            Times.Never
        );
        unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.CommitAsync(It.IsAny<CancellationToken>()), 
            Times.Never 
        );
    }
    
    [Fact]
    public async Task ShouldReturnErrorWhenCategoryNotFound()
    {
        // GIVEN
        var categoryResult = fixture.GetCategory();
        var repositoryMock = UpdateCategoryTestFixture.RepositoryMock(categoryResult);
        var command = fixture.GetInvalidCommand();
        repositoryMock
            .Setup(repository => repository.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => Result.Failure<Category>($"Category (id: {command.Id}) not found"));
        var unitOfWorkMock = UpdateCategoryTestFixture.UnitOfWorkMock();
        
        // WHEN
        var handler = new UpdateCategoryCommandHandler(unitOfWorkMock.Object, repositoryMock.Object);
        var result = await handler.Handle(command, CancellationToken.None); 
        
        // THEN
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldBe($"Category (id: {command.Id}) not found");
        repositoryMock.Verify(
            repository => repository.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), 
            Times.Once
        );
        repositoryMock.Verify(
            repository => repository.UpdateAsync(
                It.IsAny<Category>(), 
                It.IsAny<CancellationToken>()
            ), 
            Times.Never
        );
        unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.CommitAsync(It.IsAny<CancellationToken>()), 
            Times.Never 
        );
    }
}