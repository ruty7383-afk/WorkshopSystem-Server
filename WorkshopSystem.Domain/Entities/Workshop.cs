using System.ComponentModel.DataAnnotations;

namespace WorkshopSystem.Domain.Entities;

public class Workshop
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

    public Category Category { get; set; } = null!;

    [ConcurrencyCheck]
    public Guid Version { get; set; } = Guid.NewGuid();

    public ICollection<Registration> Registrations { get; set; }
        = new List<Registration>();
}