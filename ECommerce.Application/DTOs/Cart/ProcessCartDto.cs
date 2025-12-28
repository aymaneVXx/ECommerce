namespace ECommerce.Application.DTOs.Cart;

public class ProcessCartDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
