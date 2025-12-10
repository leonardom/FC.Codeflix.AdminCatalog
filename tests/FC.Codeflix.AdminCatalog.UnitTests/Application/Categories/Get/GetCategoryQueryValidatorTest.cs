using FC.Codeflix.AdminCatalog.Application.Categories.Get;
using Shouldly;

namespace FC.Codeflix.AdminCatalog.UnitTests.Application.Categories.Get;

public class GetCategoryQueryValidatorTest
{
    [Fact]
    public void ShouldNotReturnErrorWhenQueryIsValid()
    {
        // GIVEN
        var query = new GetCategoryQuery(Guid.CreateVersion7());
        var validator = new GetCategoryQueryValidator();
        
        // WHEN
        var result = validator.Validate(query);
        
        // THEN
        result.ShouldNotBeNull();
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }
    
    [Fact]
    public void ShouldReturnErrorWhenQueryIsInvalid()
    {
        // GIVEN
        var query = new GetCategoryQuery(Guid.Empty);
        var validator = new GetCategoryQueryValidator();
        
        // WHEN
        var result = validator.Validate(query);
        
        // THEN
        result.ShouldNotBeNull();
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors[0].ErrorMessage.ShouldBe("'Id' must not be empty.");
    }
}