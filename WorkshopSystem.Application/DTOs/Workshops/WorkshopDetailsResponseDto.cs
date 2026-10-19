using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkshopSystem.Application.DTOs.Workshops;

public class WorkshopDetailsResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public DateTime StartDate { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }

    public int DurationMinutes { get; set; }
    public int NumberOfMeetings { get; set; }

    public decimal Price { get; set; }
    public int Capacity { get; set; }

    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;

    public int ActiveRegistrationsCount { get; set; }
    public int AvailablePlaces { get; set; }

    public string Status { get; set; } = string.Empty;
}