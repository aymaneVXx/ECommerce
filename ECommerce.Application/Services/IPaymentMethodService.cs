using ECommerce.Application.DTOs.Cart;
using ECommerce.Application.DTOs.Payment;

namespace ECommerce.Application.Services;

public interface IPaymentMethodService
{
    Task<IEnumerable<GetPaymentMethodDto>> GetPaymentMethodsAsync();
}
