using FC.Codeflix.AdminCatalog.Domain.Common;
using FC.Codeflix.AdminCatalog.Domain.SeedWork;
using FC.Codeflix.AdminCatalog.Domain.Validation;

namespace FC.Codeflix.AdminCatalog.Domain.Entity;

public class Category : AggregateRoot
{
    private const int MinNameLength = 3;
    private const int MaxNameLength = 255;
    private const int MaxDescriptionLength = 10_000;
    
    public string Name { get; private set; }
    public string Description { get; private set; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; private set; }
    public bool IsActive { get; private set; }
    
    private Category(string name, string description, bool active)
    {
        Name = name;
        Description = description;
        IsActive = active;
        CreatedAt = DateTime.UtcNow;
    }
    
    private Result<Category> Validate()
    {
        var result = DomainValidation.NotBlank(nameof(Name), Name);
        if (result.IsFailure) return Result.Failure<Category>(result.Error);
        
        result = DomainValidation.MinLength(nameof(Name), MinNameLength, Name);
        if (result.IsFailure) return Result.Failure<Category>(result.Error);
        
        result = DomainValidation.MaxLength(nameof(Name), MaxNameLength, Name);
        if (result.IsFailure) return Result.Failure<Category>(result.Error);

        result = DomainValidation.NotBlank(nameof(Description), Description);
        if (result.IsFailure) return Result.Failure<Category>(result.Error);

        result = DomainValidation.MaxLength(nameof(Description), MaxDescriptionLength, Description);
        return result.IsFailure ? Result.Failure<Category>(result.Error) : Result.Success(this);
    }

    public static Result<Category> Create(string name, string description, bool isActive = true)
    {
        return new Category(name, description, isActive)
            .Validate();
    }

    public Result<Category> Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
        return Validate();
    }
    
    public Result<Category> Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
        return Validate();
    }

    public Result<Category> Update(string? name = null, string? description = null)
    {
        Name = name ?? Name;
        Description = description ?? Description;
        UpdatedAt = DateTime.UtcNow;
        return Validate();
    }
}
