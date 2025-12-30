namespace ECommerce.Domain.Entities;

public class PendingCheckout
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string UserId { get; set; } = string.Empty;

    // JSON du panier validé côté serveur
    public string CartJson { get; set; } = string.Empty;

    public bool Processed { get; set; } = false;

    public string? StripeSessionId { get; set; }

    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
}
