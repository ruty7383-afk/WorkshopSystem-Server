using Microsoft.EntityFrameworkCore;
using WorkshopSystem.Application.Interfaces.Repositories;
using WorkshopSystem.Domain.Entities;
using WorkshopSystem.Infrastructure.Data;

namespace WorkshopSystem.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(
        string email, CancellationToken ct)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email, ct);
    }

    public async Task<User?> GetByIdAsync(
        int id, CancellationToken ct)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public async Task<bool> EmailExistsAsync(
        string email, CancellationToken ct)
    {
        return await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Email == email, ct);
    }

    public async Task AddAsync(User user, CancellationToken ct)
    {
        await _context.Users.AddAsync(user, ct);
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct)
    {
        return await _context.SaveChangesAsync(ct);
    }
}