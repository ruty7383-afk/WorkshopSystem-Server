using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkshopSystem.Application.DTOs.Registrations;

public class WorkshopRegistrantResponseDto
{
    public int RegistrationId { get; set; }

    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime RegisteredAtUtc { get; set; }
    public decimal AmountPaid { get; set; }

    public DateTime? CancelledAtUtc { get; set; }
    public decimal RefundedAmount { get; set; }
}