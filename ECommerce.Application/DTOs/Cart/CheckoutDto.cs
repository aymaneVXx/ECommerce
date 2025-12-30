namespace ECommerce.Application.DTOs.Cart;

public class CheckoutDto
{
    public Guid PaymentMethodId { get; set; }
    public List<ProcessCart> Carts { get; set; } = new();
}

