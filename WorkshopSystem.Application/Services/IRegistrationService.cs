using WorkshopSystem.Application.DTOs.Registrations;

namespace WorkshopSystem.Application.Interfaces.Services;

public interface IRegistrationService
{
    Task<RegistrationResponseDto> RegisterAsync(
        int workshopId,
        int userId,
        CancellationToken ct);

    Task CancelAsync(
        int workshopId,
        int userId,
        CancellationToken ct);

    Task<IReadOnlyList<RegistrationResponseDto>> GetMyRegistrationsAsync(
        int userId,
        CancellationToken ct);

    Task<IReadOnlyList<WorkshopRegistrantResponseDto>>
        GetWorkshopRegistrationsAsync(
            int workshopId,
            CancellationToken ct);
}