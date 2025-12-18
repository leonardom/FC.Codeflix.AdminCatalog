using FC.Codeflix.AdminCatalog.Domain.Categories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FC.Codeflix.AdminCatalog.Infra.Data.EF.Configs;

public class CategoryConfig : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name)
            .HasMaxLength(255);
        builder.Property(c => c.Description)
            .HasMaxLength(10_000);
    }
}