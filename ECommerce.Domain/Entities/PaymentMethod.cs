using System.ComponentModel.DataAnnotations;

namespace ECommerce.Domain.Entities;

public class PaymentMethod
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;
}
