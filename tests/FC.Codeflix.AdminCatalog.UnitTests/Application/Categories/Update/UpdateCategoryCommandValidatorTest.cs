using FC.Codeflix.AdminCatalog.Application.Categories.Update;
using Shouldly;

namespace FC.Codeflix.AdminCatalog.UnitTests.Application.Categories.Update;

public class UpdateCategoryCommandValidatorTest
{
    [Fact]
    public void ShouldNotReturnErrorWhenCommandContainsValidId()
    {
        // GIVEN
        var command = new UpdateCategoryCommand(Guid.CreateVersion7(), "Name", "Description");
        var validator = new UpdateCategoryCommandValidator();
        
        // WHEN
        var result = validator.Validate(command);
        
        // THEN
        result.ShouldNotBeNull();
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }
    
    [Fact]
    public void ShouldReturnErrorWhenCommandContainsInvalidId()
    {
        // GIVEN
        var command = new UpdateCategoryCommand(Guid.Empty, "Name",  "Description");
        var validator = new UpdateCategoryCommandValidator();
        
        // WHEN
        var result = validator.Validate(command);
        
        // THEN
        result.ShouldNotBeNull();
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors[0].ErrorMessage.ShouldBe("'Id' must not be empty.");
    }
}