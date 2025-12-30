namespace ECommerce.Application.DTOs.Cart;

public class CreateCheckoutArchiveDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal AmountPaid { get; set; }
}


