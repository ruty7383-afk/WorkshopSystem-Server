using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace WorkshopSystem.Application.DTOs.Workshops;

public class WorkshopQueryDto
{
    public string? Search { get; set; }

    public int? CategoryId { get; set; }

    public DayOfWeek? DayOfWeek { get; set; }

    public string? Availability { get; set; }

    public string? Status { get; set; }

    public string? SortBy { get; set; }

    public string? SortDirection { get; set; }

    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(1, 100)]
    public int PageSize { get; set; } = 10;
}