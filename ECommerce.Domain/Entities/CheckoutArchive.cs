using System;

namespace ECommerce.Domain.Entities;

public class CheckoutArchive
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string UserId { get; set; } = string.Empty;  
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }

    public DateTime DateCreated { get; set; } = DateTime.UtcNow;

    public Product? Product { get; set; }
}
