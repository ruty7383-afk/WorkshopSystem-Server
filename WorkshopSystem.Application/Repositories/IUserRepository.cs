using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WorkshopSystem.Domain.Entities;

namespace WorkshopSystem.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken ct);

    Task<User?> GetByIdAsync(int id, CancellationToken ct);

    Task<bool> EmailExistsAsync(string email, CancellationToken ct);

    Task AddAsync(User user, CancellationToken ct);

    Task<int> SaveChangesAsync(CancellationToken ct);
}