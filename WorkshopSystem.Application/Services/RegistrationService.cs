using AutoMapper;
using WorkshopSystem.Application.DTOs.Registrations;
using WorkshopSystem.Application.Exceptions;
using WorkshopSystem.Application.Interfaces.Repositories;
using WorkshopSystem.Application.Interfaces.Services;
using WorkshopSystem.Domain.Entities;
using WorkshopSystem.Domain.Enums;

namespace WorkshopSystem.Application.Services;

public class RegistrationService : IRegistrationService
{
    private readonly IWorkshopRepository _workshopRepository;
    private readonly IMapper _mapper;

    public RegistrationService(
        IWorkshopRepository workshopRepository,
        IMapper mapper)
    {
        _workshopRepository = workshopRepository;
        _mapper = mapper;
    }

    public async Task<RegistrationResponseDto> RegisterAsync(
        int workshopId,
        int userId,
        CancellationToken ct)
    {
        var workshop =
            await _workshopRepository.GetByIdWithRegistrationsAsync(
                workshopId, ct);

        if (workshop == null)
            throw new NotFoundException("Workshop not found.");

        var startDateTime =
            workshop.StartDate.Date +
            workshop.StartTime.ToTimeSpan();

        if (DateTime.Now >= startDateTime)
            throw new ConflictException(
                "Cannot register after the workshop has started.");

        var alreadyRegistered = workshop.Registrations.Any(r =>
            r.UserId == userId &&
            r.Status == RegistrationStatus.Active);

        if (alreadyRegistered)
            throw new ConflictException(
                "You are already registered for this workshop.");

        var activeCount = workshop.Registrations.Count(r =>
            r.Status == RegistrationStatus.Active);

        if (activeCount >= workshop.Capacity)
            throw new ConflictException(
                "Workshop is full.");

        var registration = new Registration
        {
            UserId = userId,
            WorkshopId = workshopId,
            Workshop = workshop,
            RegisteredAtUtc = DateTime.UtcNow,
            AmountPaid = workshop.Price,
            Status = RegistrationStatus.Active,
            CancelledAtUtc = null,
            RefundedAmount = 0
        };

        workshop.Registrations.Add(registration);

        // חשוב ל-Concurrency:
        // ה-Workshop חייב להיות Modified באותה יחידת עבודה.
        workshop.Version = Guid.NewGuid();

        await _workshopRepository.SaveChangesAsync(ct);

        return _mapper.Map<RegistrationResponseDto>(registration);
    }

    public async Task CancelAsync(
        int workshopId,
        int userId,
        CancellationToken ct)
    {
        var workshop =
            await _workshopRepository.GetByIdWithRegistrationsAsync(
                workshopId, ct);

        if (workshop == null)
            throw new NotFoundException("Workshop not found.");

        var registration = workshop.Registrations
            .FirstOrDefault(r =>
                r.UserId == userId &&
                r.Status == RegistrationStatus.Active);

        if (registration == null)
            throw new NotFoundException(
                "Active registration not found.");

        var startDateTime =
            workshop.StartDate.Date +
            workshop.StartTime.ToTimeSpan();

        if (DateTime.Now >= startDateTime)
            throw new ConflictException(
                "Cannot cancel after the workshop has started.");

        registration.Status = RegistrationStatus.Cancelled;
        registration.CancelledAtUtc = DateTime.UtcNow;

        // לפי הדרישה - החזר מלא
        registration.RefundedAmount = registration.AmountPaid;

        // גורם ל-Workshop להשתתף ב-concurrency
        workshop.Version = Guid.NewGuid();

        await _workshopRepository.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<RegistrationResponseDto>>
        GetMyRegistrationsAsync(
            int userId,
            CancellationToken ct)
    {
        var registrations =
            await _workshopRepository.GetUserRegistrationsAsync(
                userId, ct);

        return _mapper.Map<
            IReadOnlyList<RegistrationResponseDto>>(registrations);
    }

    public async Task<IReadOnlyList<WorkshopRegistrantResponseDto>>
        GetWorkshopRegistrationsAsync(
            int workshopId,
            CancellationToken ct)
    {
        if (!await _workshopRepository.ExistsAsync(workshopId, ct))
            throw new NotFoundException("Workshop not found.");

        var registrations =
            await _workshopRepository.GetWorkshopRegistrationsAsync(
                workshopId, ct);

        return _mapper.Map<
            IReadOnlyList<WorkshopRegistrantResponseDto>>(registrations);
    }
}