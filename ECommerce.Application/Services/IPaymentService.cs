using ECommerce.Application.DTOs.Payment;

namespace ECommerce.Application.Services;

public interface IPaymentService
{
    Task<List<GetPaymentMethodDto>> GetAllAsync();
}
