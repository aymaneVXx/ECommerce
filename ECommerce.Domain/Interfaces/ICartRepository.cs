using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces;

public interface ICartRepository
{
    Task<bool> SaveCheckoutHistoryAsync(IEnumerable<CheckoutArchive> archives);
    Task<List<CheckoutArchive>> GetCheckoutHistoryAsync();

    // Pending checkout
    Task<Guid> CreatePendingCheckoutAsync(PendingCheckout pending);
    Task<PendingCheckout?> GetPendingCheckoutAsync(Guid id);

    // traitement atomique (idempotence + pas de doublons en concurrence)
    Task<bool> TryProcessPendingCheckoutAsync(Guid pendingCheckoutId, string stripeSessionId, IEnumerable<CheckoutArchive> archives);

    // helper
    Task<bool> IsStripeSessionProcessedAsync(string stripeSessionId);
}
