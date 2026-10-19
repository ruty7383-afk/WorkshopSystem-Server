using AutoMapper;
using WorkshopSystem.Application.DTOs.Common;
using WorkshopSystem.Application.DTOs.Workshops;
using WorkshopSystem.Application.Exceptions;
using WorkshopSystem.Application.Interfaces.Repositories;
using WorkshopSystem.Application.Interfaces.Services;
using WorkshopSystem.Domain.Entities;
using WorkshopSystem.Domain.Enums;

namespace WorkshopSystem.Application.Services;

public class WorkshopService : IWorkshopService
{
    private readonly IWorkshopRepository _workshopRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public WorkshopService(
        IWorkshopRepository workshopRepository,
        ICategoryRepository categoryRepository,
        IMapper mapper)
    {
        _workshopRepository = workshopRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<PagedResponse<WorkshopResponseDto>> GetPagedAsync(
        WorkshopQueryDto query,
        CancellationToken ct)
    {
        var now = DateTime.Now;

        var result = await _workshopRepository.GetPagedAsync(
            query, now, ct);

        var items = result.Items
            .Select(MapWorkshopResponse)
            .ToList();

        return new PagedResponse<WorkshopResponseDto>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = result.TotalCount,
            TotalPages = (int)Math.Ceiling(
                result.TotalCount / (double)query.PageSize)
        };
    }

    public async Task<WorkshopDetailsResponseDto> GetByIdAsync(
        int id,
        CancellationToken ct)
    {
        var workshop =
            await _workshopRepository.GetByIdWithRegistrationsAsync(id, ct);

        if (workshop == null)
            throw new NotFoundException("Workshop not found.");

        return MapWorkshopDetails(workshop);
    }

    public async Task<WorkshopDetailsResponseDto> CreateAsync(
        CreateWorkshopRequestDto request,
        CancellationToken ct)
    {
        await ValidateCategoryAndDayAsync(
            request.CategoryId,
            request.StartDate,
            request.DayOfWeek,
            ct);
        request.StartDate = DateTime.SpecifyKind(
    request.StartDate.Date,
    DateTimeKind.Utc);

        var workshop = _mapper.Map<Workshop>(request);

        await _workshopRepository.AddAsync(workshop, ct);
        await _workshopRepository.SaveChangesAsync(ct);

        return MapWorkshopDetails(workshop);
    }

    public async Task UpdateAsync(
        int id,
        UpdateWorkshopRequestDto request,
        CancellationToken ct)
    {
        var workshop =
            await _workshopRepository.GetByIdWithRegistrationsAsync(id, ct);

        if (workshop == null)
            throw new NotFoundException("Workshop not found.");

        await ValidateCategoryAndDayAsync(
            request.CategoryId,
            request.StartDate,
            request.DayOfWeek,
            ct);
        request.StartDate = DateTime.SpecifyKind(
    request.StartDate.Date,
    DateTimeKind.Utc);
        var activeCount = workshop.Registrations.Count(r =>
            r.Status == RegistrationStatus.Active);

        if (request.Capacity < activeCount)
        {
            throw new ConflictException(
                "Capacity cannot be smaller than the number of active registrations.");
        }

        _mapper.Map(request, workshop);

        await _workshopRepository.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(
        int id,
        CancellationToken ct)
    {
        var workshop =
            await _workshopRepository.GetByIdWithRegistrationsAsync(id, ct);

        if (workshop == null)
            throw new NotFoundException("Workshop not found.");

        if (workshop.Registrations.Any())
        {
            throw new ConflictException(
                "Cannot delete a workshop with registration history.");
        }

        _workshopRepository.Remove(workshop);
        await _workshopRepository.SaveChangesAsync(ct);
    }

    private async Task ValidateCategoryAndDayAsync(
        int categoryId,
        DateTime startDate,
        DayOfWeek dayOfWeek,
        CancellationToken ct)
    {
        var category =
            await _categoryRepository.GetByIdAsync(categoryId, ct);

        if (category == null)
            throw new NotFoundException("Category not found.");

        if (startDate.DayOfWeek != dayOfWeek)
        {
            throw new BusinessException(
                "StartDate does not match DayOfWeek.");
        }
    }

    private WorkshopResponseDto MapWorkshopResponse(
        Workshop workshop)
    {
        var dto = _mapper.Map<WorkshopResponseDto>(workshop);

        var activeCount = workshop.Registrations.Count(r =>
            r.Status == RegistrationStatus.Active);

        dto.ActiveRegistrationsCount = activeCount;
        dto.AvailablePlaces = workshop.Capacity - activeCount;
        dto.Status = CalculateStatus(workshop);

        return dto;
    }

    private WorkshopDetailsResponseDto MapWorkshopDetails(
        Workshop workshop)
    {
        var dto =
            _mapper.Map<WorkshopDetailsResponseDto>(workshop);

        var activeCount = workshop.Registrations.Count(r =>
            r.Status == RegistrationStatus.Active);

        dto.ActiveRegistrationsCount = activeCount;
        dto.AvailablePlaces = workshop.Capacity - activeCount;
        dto.Status = CalculateStatus(workshop);

        return dto;
    }

    private static string CalculateStatus(Workshop workshop)
    {
        var startDateTime =
            workshop.StartDate.Date +
            workshop.StartTime.ToTimeSpan();

        var lastMeetingStart =
            startDateTime.AddDays(
                7 * (workshop.NumberOfMeetings - 1));

        var finishDateTime =
            lastMeetingStart.AddMinutes(
                workshop.DurationMinutes);

        var now = DateTime.Now;

        if (now < startDateTime)
            return "Upcoming";

        if (now >= finishDateTime)
            return "Finished";

        return "Started";
    }
}