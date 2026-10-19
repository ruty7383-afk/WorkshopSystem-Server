using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WorkshopSystem.Domain.Entities;

namespace WorkshopSystem.Application.Interfaces.Repositories;

public interface ICategoryRepository
{
    Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken ct);

    Task<Category?> GetByIdAsync(int id, CancellationToken ct);

    Task<bool> ExistsByNameAsync(
        string name,
        int? excludeId,
        CancellationToken ct);

    Task<bool> IsInUseAsync(int categoryId, CancellationToken ct);

    Task AddAsync(Category category, CancellationToken ct);

    void Remove(Category category);

    Task<int> SaveChangesAsync(CancellationToken ct);
}