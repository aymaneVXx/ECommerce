namespace ECommerce.Application.DTOs.Payment;

public class GetPaymentMethodDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
