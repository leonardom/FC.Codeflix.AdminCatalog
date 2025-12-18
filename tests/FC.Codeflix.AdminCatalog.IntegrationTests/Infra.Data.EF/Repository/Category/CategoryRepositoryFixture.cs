using FC.Codeflix.AdminCatalog.Infra.Data.EF;
using FC.Codeflix.AdminCatalog.IntegrationTests.Common;
using FC.Codeflix.AdminCatalog.SharedKernel;
using Microsoft.EntityFrameworkCore;
using DomainEntity = FC.Codeflix.AdminCatalog.Domain.Categories;

namespace FC.Codeflix.AdminCatalog.IntegrationTests.Infra.Data.EF.Repository.Category;

public class CategoryRepositoryFixture : BaseFixture
{
    public DomainEntity.Category CreateCategory()
        => DomainEntity.Category.Create(GenerateName(), GenerateDescription()).Value;
    
    public List<DomainEntity.Category> CreateCategoryList(int n = 10)
        => Enumerable.Range(0, n).Select(_ => CreateCategory()).ToList();
    
    public List<DomainEntity.Category> CreateCategoryList(string[] names)
        => names.Select(name =>
        {
            var result = DomainEntity.Category.Create(name, GenerateDescription());
            return result.Value;
        }).ToList();

    public List<DomainEntity.Category> OrderCategoryList(List<DomainEntity.Category> categories, 
        string orderBy, SearchOrder order)
    {
        var list = new List<DomainEntity.Category>(categories);
        var prop = typeof(DomainEntity.Category).GetProperty(orderBy, 
            System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        if (prop == null) return list;
        var ordered = order == SearchOrder.Desc 
            ? list.OrderByDescending(x => prop.GetValue(x, null)) 
            : list.OrderBy(x => prop.GetValue(x, null));
        return ordered.ToList();
    }
    
    public AppDbContext CreateDbContext()
        => new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("integration-test-db")
            .Options);
    
    public void ClearDatabase()
        => CreateDbContext().Database.EnsureDeleted();
}

[CollectionDefinition(nameof(CategoryRepositoryFixture))]
public class CategoryRepositoryFixtureCollection : ICollectionFixture<CategoryRepositoryFixture>;