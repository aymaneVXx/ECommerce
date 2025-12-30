using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _db;

    public ProductRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Product>> GetAllAsync()
        => await _db.Products
            .AsNoTracking()
            .Include(x => x.Category)
            .OrderByDescending(x => x.DateCreated)
            .ToListAsync();

    public async Task<Product?> GetByIdAsync(Guid id)
        => await _db.Products
            .AsNoTracking()
            .Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<List<Product>> GetByCategoryIdAsync(Guid categoryId)
        => await _db.Products
            .AsNoTracking()
            .Where(x => x.CategoryId == categoryId)
            .OrderByDescending(x => x.DateCreated)
            .ToListAsync();

    public async Task<Product> AddAsync(Product product)
    {
        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        return product;
    }

    public async Task<Product?> UpdateAsync(Product product)
    {
        var existing = await _db.Products.FirstOrDefaultAsync(x => x.Id == product.Id);
        if (existing is null) return null;

        existing.Name = product.Name;
        existing.Price = product.Price;
        existing.Quantity = product.Quantity;
        existing.Description = product.Description;
        existing.Image = product.Image;
        existing.CategoryId = product.CategoryId;

        await _db.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _db.Products.FirstOrDefaultAsync(x => x.Id == id);
        if (existing is null) return false;

        _db.Products.Remove(existing);
        await _db.SaveChangesAsync();
        return true;
    }
    public async Task<List<Product>> GetProductsByIdsAsync(IEnumerable<Guid> ids)
    {
        var list = ids?.Distinct().ToList() ?? new List<Guid>();
        if (list.Count == 0)
            return new List<Product>();

        return await _db.Products
            .AsNoTracking()
            .Where(p => list.Contains(p.Id))
            .ToListAsync();
    }

}
