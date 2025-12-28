namespace ECommerce.Application.DTOs.Cart;

public class GetCheckoutArchiveDto
{
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal AmountPaid { get; set; }
    public DateTime DateCreated { get; set; }
}
