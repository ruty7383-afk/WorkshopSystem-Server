using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WorkshopSystem.Application.DTOs.Categories;

namespace WorkshopSystem.Application.Interfaces.Services;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryResponseDto>> GetAllAsync(
        CancellationToken ct);

    Task<CategoryResponseDto> CreateAsync(
        CreateCategoryRequestDto request,
        CancellationToken ct);

    Task UpdateAsync(
        int id,
        UpdateCategoryRequestDto request,
        CancellationToken ct);

    Task DeleteAsync(
        int id,
        CancellationToken ct);
}
