using FluentValidation;

namespace FC.Codeflix.AdminCatalog.Application.Categories.Get;

public class GetCategoryQueryValidator : AbstractValidator<GetCategoryQuery>
{
    public GetCategoryQueryValidator()
        => RuleFor(c => c.Id).NotEmpty();
}