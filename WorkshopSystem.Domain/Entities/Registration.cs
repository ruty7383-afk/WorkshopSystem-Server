using WorkshopSystem.Domain.Enums;

namespace WorkshopSystem.Domain.Entities;

public class Registration
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public User User { get; set; } = null!;

    public int WorkshopId { get; set; }

    public Workshop Workshop { get; set; } = null!;

    public DateTime RegisteredAtUtc { get; set; }

    public decimal AmountPaid { get; set; }

    public RegistrationStatus Status { get; set; }

    public DateTime? CancelledAtUtc { get; set; }

    public decimal RefundedAmount { get; set; }
}