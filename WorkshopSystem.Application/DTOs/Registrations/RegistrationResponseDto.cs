using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkshopSystem.Application.DTOs.Registrations;

public class RegistrationResponseDto
{
    public int Id { get; set; }

    public int WorkshopId { get; set; }
    public string WorkshopName { get; set; } = string.Empty;

    public DateTime RegisteredAtUtc { get; set; }
    public decimal AmountPaid { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime? CancelledAtUtc { get; set; }
    public decimal RefundedAmount { get; set; }

    public DateTime StartDate { get; set; }
    public TimeOnly StartTime { get; set; }
}