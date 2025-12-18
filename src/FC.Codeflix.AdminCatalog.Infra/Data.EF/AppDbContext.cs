using FC.Codeflix.AdminCatalog.Domain.Categories;
using FC.Codeflix.AdminCatalog.Infra.Data.EF.Configs;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.AdminCatalog.Infra.Data.EF;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new CategoryConfig());
    }
}