using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _db;

    public CategoryRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Category>> GetAllAsync()
        => await _db.Categories.AsNoTracking().ToListAsync();

    public async Task<Category?> GetByIdAsync(Guid id)
        => await _db.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

    public async Task<Category> AddAsync(Category category)
    {
        _db.Categories.Add(category);
        await _db.SaveChangesAsync();
        return category;
    }

    public async Task<Category?> UpdateAsync(Category category)
    {
        var existing = await _db.Categories.FirstOrDefaultAsync(x => x.Id == category.Id);
        if (existing is null) return null;

        existing.Name = category.Name;

        await _db.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _db.Categories.FirstOrDefaultAsync(x => x.Id == id);
        if (existing is null) return false;

        _db.Categories.Remove(existing);
        await _db.SaveChangesAsync();
        return true;
    }
}
