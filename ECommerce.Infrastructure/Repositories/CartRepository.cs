using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories;

public class CartRepository : ICartRepository
{
    private readonly AppDbContext _db;

    public CartRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<bool> SaveCheckoutHistoryAsync(IEnumerable<CheckoutArchive> archives)
    {
        await _db.CheckoutArchives.AddRangeAsync(archives);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<CheckoutArchive>> GetCheckoutHistoryAsync()
        => await _db.CheckoutArchives
            .AsNoTracking()
            .OrderByDescending(x => x.DateCreated)
            .ToListAsync();
}
