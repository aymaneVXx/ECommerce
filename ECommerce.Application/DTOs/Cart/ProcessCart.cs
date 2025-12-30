using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Cart;

public class ProcessCart
{
    [Required]
    public Guid ProductId { get; set; }

    [Required]
    public int Quantity { get; set; }
}
