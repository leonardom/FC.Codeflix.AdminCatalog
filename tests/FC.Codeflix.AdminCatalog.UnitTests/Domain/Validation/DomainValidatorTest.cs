using Bogus;
using FC.Codeflix.AdminCatalog.Domain.Validation;
using Shouldly;

namespace FC.Codeflix.AdminCatalog.UnitTests.Domain.Validation;

public class DomainValidatorTest
{
    private readonly Faker _faker = new();
    
    [Fact]
    public void ShouldNotReturnErrorWhenValueNotIsBlank()
    {
        var input = _faker.Name.FirstName();
        var result = DomainValidator.NotBlank("FieldName", input);
        result.IsSuccess.ShouldBeTrue();
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void ShouldReturnErrorWhenValueIsBlank(string? input)
    {
        var result = DomainValidator.NotBlank("FieldName", input);
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("FieldName must not be blank");
    }

    [Theory]
    [InlineData("abc")]
    [InlineData(123)]
    [InlineData(10.12)]
    public void ShouldNotReturnErrorWhenValueIsNotNull(object input)
    {
        var result = DomainValidator.NotNull("FieldName", input);
        result.IsSuccess.ShouldBeTrue();
    }
    
    [Fact]
    public void ShouldReturnErrorWhenValueIsNull()
    {
        var result = DomainValidator.NotNull("FieldName", null);
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("FieldName must not be null");
    }

    [Theory]
    [InlineData("abcde", 5)]
    [InlineData("abcdef1234xyz", 10)]
    public void ShouldNotReturnErrorWhenStringIsGreaterOrEqualThanMinLength(string? input, int minLength)
    {
        var result = DomainValidator.MinLength("FieldName", minLength, input);
        result.IsSuccess.ShouldBeTrue();
    }
    
    [Theory]
    [InlineData(null, 3)]
    [InlineData("abc", 5)]
    [InlineData("abcdef", 10)]
    public void ShouldReturnErrorWhenStringIsLessThanMinLength(string? input, int minLength)
    {
        var result = DomainValidator.MinLength("FieldName", minLength, input);
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe($"FieldName must contain at least {minLength} characters");
    }
    
    [Theory]
    [InlineData(null, 1)]
    [InlineData("abcde", 5)]
    [InlineData("abcdef1234", 10)]
    public void ShouldNotReturnErrorWhenStringIsLessOrEqualThanMaxLength(string? input, int maxLength)
    {
        var result = DomainValidator.MaxLength("FieldName", maxLength, input);
        result.IsSuccess.ShouldBeTrue();
    }
    
    [Theory]
    [InlineData("abcdef", 3)]
    [InlineData("abcdef1234xyz9", 10)]
    public void ShouldReturnErrorWhenStringIsGreaterThanMaxLength(string? input, int maxLength)
    {
        var result = DomainValidator.MaxLength("FieldName", maxLength, input);
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe($"FieldName must contain at most {maxLength} characters");
    }
}