namespace FC.Codeflix.AdminCatalog.Application.Categories.Update;

public record UpdateCategoryCommand(Guid Id, string? Name, string? Description);