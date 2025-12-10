using FC.Codeflix.AdminCatalog.Domain.Validation;
using FC.Codeflix.AdminCatalog.SharedKernel;

namespace FC.Codeflix.AdminCatalog.Domain.Categories;

public class Category : AggregateRoot
{
    private const int MinNameLength = 3;
    private const int MaxNameLength = 255;
    private const int MaxDescriptionLength = 10_000;
    
    public string Name { get; private set; }
    public string Description { get; private set; }
    public DateTime CreatedAt { get; private set; }
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
        var result = DomainValidator.NotBlank(nameof(Name), Name);
        if (result.IsFailure) return Result.Failure<Category>(result.Error);
        
        result = DomainValidator.MinLength(nameof(Name), MinNameLength, Name);
        if (result.IsFailure) return Result.Failure<Category>(result.Error);
        
        result = DomainValidator.MaxLength(nameof(Name), MaxNameLength, Name);
        if (result.IsFailure) return Result.Failure<Category>(result.Error);

        result = DomainValidator.NotBlank(nameof(Description), Description);
        if (result.IsFailure) return Result.Failure<Category>(result.Error);

        result = DomainValidator.MaxLength(nameof(Description), MaxDescriptionLength, Description);
        return result.IsFailure ? Result.Failure<Category>(result.Error) : Result.Success(this);
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
    
    public static Result<Category> Create(string name, string description, bool isActive = true)
        => new Category(name, description, isActive).Validate();

    public static Result<Category> Restore(Guid id, string name, string description, bool isActive, DateTime createdAt, DateTime updatedAt)
    {
        var category = new Category(name, description, isActive)
        {
            Id = id,
            CreatedAt = createdAt,
            UpdatedAt = createdAt
        };
        return Result.Success(category);
    }
}
