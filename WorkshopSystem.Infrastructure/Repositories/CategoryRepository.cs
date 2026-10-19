using Microsoft.EntityFrameworkCore;
using WorkshopSystem.Application.Interfaces.Repositories;
using WorkshopSystem.Domain.Entities;
using WorkshopSystem.Infrastructure.Data;

namespace WorkshopSystem.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;

    public CategoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Category>> GetAllAsync(
        CancellationToken ct)
    {
        return await _context.Categories
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<Category?> GetByIdAsync(
        int id, CancellationToken ct)
    {
        return await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task<bool> ExistsByNameAsync(
        string name, int? excludeId, CancellationToken ct)
    {
        return await _context.Categories
            .AsNoTracking()
            .AnyAsync(c =>
                c.Name == name &&
                (!excludeId.HasValue || c.Id != excludeId.Value),
                ct);
    }

    public async Task<bool> IsInUseAsync(
        int categoryId, CancellationToken ct)
    {
        return await _context.Workshops
            .AsNoTracking()
            .AnyAsync(w => w.CategoryId == categoryId, ct);
    }

    public async Task AddAsync(
        Category category, CancellationToken ct)
    {
        await _context.Categories.AddAsync(category, ct);
    }

    public void Remove(Category category)
    {
        _context.Categories.Remove(category);
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct)
    {
        return await _context.SaveChangesAsync(ct);
    }
}