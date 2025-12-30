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
            .Include(x => x.Product)
            .OrderByDescending(x => x.DateCreated)
            .ToListAsync();

    public async Task<Guid> CreatePendingCheckoutAsync(PendingCheckout pending)
    {
        _db.PendingCheckouts.Add(pending);
        await _db.SaveChangesAsync();
        return pending.Id;
    }

    public async Task<PendingCheckout?> GetPendingCheckoutAsync(Guid id)
        => await _db.PendingCheckouts.FirstOrDefaultAsync(x => x.Id == id);

    public async Task<bool> IsStripeSessionProcessedAsync(string stripeSessionId)
    {
        if (string.IsNullOrWhiteSpace(stripeSessionId)) return false;

        var existsInArchive = await _db.CheckoutArchives.AnyAsync(x => x.StripeSessionId == stripeSessionId);
        if (existsInArchive) return true;

        var existsInPending = await _db.PendingCheckouts.AnyAsync(x => x.StripeSessionId == stripeSessionId);
        return existsInPending;
    }

    public async Task<bool> TryProcessPendingCheckoutAsync(Guid pendingCheckoutId, string stripeSessionId, IEnumerable<CheckoutArchive> archives)
    {
        if (pendingCheckoutId == Guid.Empty) return false;
        if (string.IsNullOrWhiteSpace(stripeSessionId)) return false;

        await using var tx = await _db.Database.BeginTransactionAsync();

        var pc = await _db.PendingCheckouts.FirstOrDefaultAsync(x => x.Id == pendingCheckoutId);
        if (pc is null) return false;
        if (pc.Processed) return false;

        pc.Processed = true;
        pc.StripeSessionId = stripeSessionId;

        await _db.CheckoutArchives.AddRangeAsync(archives);

        try
        {
            await _db.SaveChangesAsync();
            await tx.CommitAsync();
            return true;
        }
        catch (DbUpdateException)
        {
            await tx.RollbackAsync();
            return false;
        }
    }
}
