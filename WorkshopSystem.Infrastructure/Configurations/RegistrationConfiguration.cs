using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkshopSystem.Domain.Entities;

namespace WorkshopSystem.Infrastructure.Configurations;

public class RegistrationConfiguration
    : IEntityTypeConfiguration<Registration>
{
    public void Configure(EntityTypeBuilder<Registration> builder)
    {
        builder.Property(r => r.AmountPaid)
            .HasPrecision(10, 2);

        builder.Property(r => r.RefundedAmount)
            .HasPrecision(10, 2);

        builder.HasOne(r => r.User)
            .WithMany(u => u.Registrations)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Workshop)
            .WithMany(w => w.Registrations)
            .HasForeignKey(r => r.WorkshopId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}