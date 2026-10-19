using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkshopSystem.Domain.Entities;

namespace WorkshopSystem.Infrastructure.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasIndex(c => c.Name)
            .IsUnique();

        builder.HasMany(c => c.Workshops)
            .WithOne(w => w.Category)
            .HasForeignKey(w => w.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}