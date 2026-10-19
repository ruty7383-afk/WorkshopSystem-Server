using Microsoft.EntityFrameworkCore;
using WorkshopSystem.Application.DTOs.Workshops;
using WorkshopSystem.Application.Exceptions;
using WorkshopSystem.Application.Interfaces.Repositories;
using WorkshopSystem.Domain.Entities;
using WorkshopSystem.Domain.Enums;
using WorkshopSystem.Infrastructure.Data;
using Microsoft.Extensions.Logging;
namespace WorkshopSystem.Infrastructure.Repositories;

public class WorkshopRepository : IWorkshopRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<WorkshopRepository> _logger;
    public WorkshopRepository(
    AppDbContext context,
    ILogger<WorkshopRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Workshop?> GetByIdAsync(
        int id, CancellationToken ct)
    {
        return await _context.Workshops
            .AsNoTracking()
            .Include(w => w.Category)
            .FirstOrDefaultAsync(w => w.Id == id, ct);
    }

    public async Task<Workshop?> GetByIdWithRegistrationsAsync(
        int id, CancellationToken ct)
    {
        return await _context.Workshops
            .Include(w => w.Category)
            .Include(w => w.Registrations)
            .FirstOrDefaultAsync(w => w.Id == id, ct);
    }

    public async Task<bool> ExistsAsync(
        int id, CancellationToken ct)
    {
        return await _context.Workshops
            .AsNoTracking()
            .AnyAsync(w => w.Id == id, ct);
    }

    public async Task AddAsync(
        Workshop workshop, CancellationToken ct)
    {
        await _context.Workshops.AddAsync(workshop, ct);
    }

    public void Remove(Workshop workshop)
    {
        _context.Workshops.Remove(workshop);
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct)
    {
        try
        {
            return await _context.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(
                ex,
                "Concurrency conflict while saving Workshop.");

            throw new ConflictException(
                "The workshop was changed by another request. Please try again.");
        }
    }
    public IQueryable<Workshop> Query()
    {
        return _context.Workshops
            .AsNoTracking()
            .Include(w => w.Category)
            .Include(w => w.Registrations);

    }
    public async Task<(IReadOnlyList<Workshop> Items, int TotalCount)> GetPagedAsync(
    WorkshopQueryDto query,
    DateTime now,
    CancellationToken ct)
    {
        var workshopsQuery = _context.Workshops
            .AsNoTracking()
            .Include(w => w.Category)
            .Include(w => w.Registrations)
            .AsQueryable();

        // Search
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            workshopsQuery = workshopsQuery.Where(w =>
                EF.Functions.ILike(w.Name, $"%{query.Search}%"));
        }

        // Category
        if (query.CategoryId.HasValue)
        {
            workshopsQuery = workshopsQuery.Where(w =>
                w.CategoryId == query.CategoryId.Value);
        }

        // Day
        if (query.DayOfWeek.HasValue)
        {
            workshopsQuery = workshopsQuery.Where(w =>
                w.DayOfWeek == query.DayOfWeek.Value);
        }

        // Availability
        if (!string.IsNullOrWhiteSpace(query.Availability))
        {
            var availability = query.Availability.ToLower();

            if (availability == "available")
            {
                workshopsQuery = workshopsQuery.Where(w =>
                    w.Registrations.Count(r =>
                        r.Status == RegistrationStatus.Active) < w.Capacity);
            }
            else if (availability == "full")
            {
                workshopsQuery = workshopsQuery.Where(w =>
                    w.Registrations.Count(r =>
                        r.Status == RegistrationStatus.Active) >= w.Capacity);
            }
        }

        // Status
        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            var status = query.Status.ToLower();

            var today = DateTime.SpecifyKind(
    now.Date,
    DateTimeKind.Utc);

            var currentTime = TimeOnly.FromDateTime(now);

            if (status == "upcoming")
            {
                workshopsQuery = workshopsQuery.Where(w =>
                    w.StartDate.Date > today ||
                    (w.StartDate.Date == today &&
                     w.StartTime > currentTime));
            }
            else if (status == "started")
            {
                workshopsQuery = workshopsQuery.Where(w =>
                    w.StartDate.Date < today ||
                    (w.StartDate.Date == today &&
                     w.StartTime <= currentTime));
            }
        }

        // Sort
        var descending =
            string.Equals(query.SortDirection, "desc",
                StringComparison.OrdinalIgnoreCase);

        switch (query.SortBy?.ToLower())
        {
            case "name":
                workshopsQuery = descending
                    ? workshopsQuery.OrderByDescending(w => w.Name)
                    : workshopsQuery.OrderBy(w => w.Name);
                break;

            case "price":
                workshopsQuery = descending
                    ? workshopsQuery.OrderByDescending(w => w.Price)
                    : workshopsQuery.OrderBy(w => w.Price);
                break;

            case "availableplaces":
                workshopsQuery = descending
                    ? workshopsQuery.OrderByDescending(w =>
                        w.Capacity - w.Registrations.Count(r =>
                            r.Status == RegistrationStatus.Active))
                    : workshopsQuery.OrderBy(w =>
                        w.Capacity - w.Registrations.Count(r =>
                            r.Status == RegistrationStatus.Active));
                break;

            default:
                workshopsQuery = descending
                    ? workshopsQuery
                        .OrderByDescending(w => w.StartDate)
                        .ThenByDescending(w => w.StartTime)
                    : workshopsQuery
                        .OrderBy(w => w.StartDate)
                        .ThenBy(w => w.StartTime);
                break;
        }

        var totalCount = await workshopsQuery.CountAsync(ct);

        var items = await workshopsQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }
    public async Task<IReadOnlyList<Registration>> GetUserRegistrationsAsync(
    int userId,
    CancellationToken ct)
    {
        return await _context.Registrations
            .AsNoTracking()
            .Where(r => r.UserId == userId)
            .Include(r => r.Workshop)
            .OrderByDescending(r => r.RegisteredAtUtc)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Registration>> GetWorkshopRegistrationsAsync(
        int workshopId,
        CancellationToken ct)
    {
        return await _context.Registrations
            .AsNoTracking()
            .Where(r => r.WorkshopId == workshopId)
            .Include(r => r.User)
            .OrderByDescending(r => r.RegisteredAtUtc)
            .ToListAsync(ct);
    }
}