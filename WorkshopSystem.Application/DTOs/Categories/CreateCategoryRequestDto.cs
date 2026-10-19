using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace WorkshopSystem.Application.DTOs.Categories;

public class CreateCategoryRequestDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}
