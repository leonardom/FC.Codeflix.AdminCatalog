using FC.Codeflix.AdminCatalog.Domain.Categories;
using FC.Codeflix.AdminCatalog.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.AdminCatalog.Infra.Data.EF.Entities;

public class CategoryRepository(AppDbContext dbContext) : ICategoryRepository
{
    private DbSet<Category> Categories 
        => dbContext.Set<Category>();

    public async Task<Result> CreateAsync(Category aggregateRoot, CancellationToken cancellationToken = default)
    {
        await Categories.AddAsync(aggregateRoot, cancellationToken);
        return Result.Success();
    }

    public async Task<Result<Category>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await Categories.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        return category is null 
            ? Result.Failure<Category>($"Category (id: {id}) not found") 
            : Result.Success(category);
    }

    public async Task<Result> UpdateAsync(Category aggregateRoot, CancellationToken cancellationToken = default)
    {
        var category = await Categories.FindAsync([aggregateRoot.Id], cancellationToken);
        if (category is null) return Result.Failure<Category>($"Category (id: {aggregateRoot.Id}) not found"); 
        Categories.Update(aggregateRoot);
        return Result.Success();
    }

    public async Task<Result<SearchOutput<Category>>> ListAsync(SearchInput input, CancellationToken cancellationToken = default)
    {
        var query = Categories.AsNoTracking();
        var skip = (input.Page - 1) * input.PageSize;
        if (!string.IsNullOrWhiteSpace(input.Search))
        {
            query = query.Where(c => c.Name.Contains(input.Search));
        }
        query = query.OrderByPropertyName(input.Sort, input.Order);
        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip(skip)
            .Take(input.PageSize)
            .ToListAsync(cancellationToken);
        var output = new SearchOutput<Category>(input.Page, input.PageSize, items, total);
        return Result.Success(output);
    }
}