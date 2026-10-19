using WorkshopSystem.Application.DTOs.Common;
using WorkshopSystem.Application.DTOs.Workshops;

namespace WorkshopSystem.Application.Interfaces.Services;

public interface IWorkshopService
{
    Task<PagedResponse<WorkshopResponseDto>> GetPagedAsync(
        WorkshopQueryDto query,
        CancellationToken ct);

    Task<WorkshopDetailsResponseDto> GetByIdAsync(
        int id,
        CancellationToken ct);

    Task<WorkshopDetailsResponseDto> CreateAsync(
        CreateWorkshopRequestDto request,
        CancellationToken ct);

    Task UpdateAsync(
        int id,
        UpdateWorkshopRequestDto request,
        CancellationToken ct);

    Task DeleteAsync(
        int id,
        CancellationToken ct);
}