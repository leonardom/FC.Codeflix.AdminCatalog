using FC.Codeflix.AdminCatalog.SharedKernel;

namespace FC.Codeflix.AdminCatalog.Domain.Validation;

public static class DomainValidator
{
    public static Result NotBlank(string fieldName, string? value)
        => string.IsNullOrWhiteSpace(value) 
            ? Result.Failure($"{fieldName} must not be blank") 
            : Result.Success();
    
    public static Result NotNull(string fieldName, object? value)
        => value is null 
            ? Result.Failure($"{fieldName} must not be null")
            : Result.Success();
    
    public static Result MinLength(string fieldName, int minLength, string? value)
        => (string.IsNullOrWhiteSpace(value) || value.Length < minLength)
            ? Result.Failure($"{fieldName} must contain at least {minLength} characters")
            : Result.Success();
    
    public static Result MaxLength(string fieldName, int maxLength, string? value)
        => (!string.IsNullOrWhiteSpace(value) && value.Length > maxLength)
            ? Result.Failure($"{fieldName} must contain at most {maxLength} characters")
            : Result.Success();
}