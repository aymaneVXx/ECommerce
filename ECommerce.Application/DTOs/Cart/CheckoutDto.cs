namespace ECommerce.Application.DTOs.Cart;

public class CheckoutDto
{
    public Guid PaymentMethodId { get; set; }
    public List<ProcessCartDto> Carts { get; set; } = new();
}

