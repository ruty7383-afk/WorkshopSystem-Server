using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkshopSystem.Application.DTOs.Workshops;
using WorkshopSystem.Domain.Entities;

namespace WorkshopSystem.Application.Interfaces.Repositories;

public interface IWorkshopRepository
{
    Task<(IReadOnlyList<Workshop> Items, int TotalCount)> GetPagedAsync(
     WorkshopQueryDto query,
     DateTime now,
     CancellationToken ct);
    Task<Workshop?> GetByIdAsync(int id, CancellationToken ct);

    Task<Workshop?> GetByIdWithRegistrationsAsync(
        int id,
        CancellationToken ct);

    Task<bool> ExistsAsync(int id, CancellationToken ct);

    Task AddAsync(Workshop workshop, CancellationToken ct);

    void Remove(Workshop workshop);

    Task<int> SaveChangesAsync(CancellationToken ct);
    Task<IReadOnlyList<Registration>> GetUserRegistrationsAsync(
    int userId,
    CancellationToken ct);

    Task<IReadOnlyList<Registration>> GetWorkshopRegistrationsAsync(
        int workshopId,
        CancellationToken ct);

}