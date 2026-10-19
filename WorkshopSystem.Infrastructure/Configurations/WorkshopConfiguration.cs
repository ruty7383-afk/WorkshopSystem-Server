using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkshopSystem.Domain.Entities;

namespace WorkshopSystem.Infrastructure.Configurations;

public class WorkshopConfiguration : IEntityTypeConfiguration<Workshop>
{
    public void Configure(EntityTypeBuilder<Workshop> builder)
    {
        builder.Property(w => w.Price)
            .HasPrecision(10, 2);
        builder.Property(w => w.Price)
           .HasPrecision(10, 2);
    }

}