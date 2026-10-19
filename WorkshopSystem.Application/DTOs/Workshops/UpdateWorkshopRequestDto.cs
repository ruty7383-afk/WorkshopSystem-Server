using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace WorkshopSystem.Application.DTOs.Workshops;

public class UpdateWorkshopRequestDto
{
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime StartDate { get; set; }

    public DayOfWeek DayOfWeek { get; set; }

    public TimeOnly StartTime { get; set; }

    [Range(1, int.MaxValue)]
    public int DurationMinutes { get; set; }

    [Range(1, int.MaxValue)]
    public int NumberOfMeetings { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [Range(1, int.MaxValue)]
    public int Capacity { get; set; }

    [Range(1, int.MaxValue)]
    public int CategoryId { get; set; }
}