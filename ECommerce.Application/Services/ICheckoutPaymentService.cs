using ECommerce.Application.DTOs.Cart;
using ECommerce.Application.Responses;

namespace ECommerce.Application.Services;

public interface ICheckoutPaymentService
{
    Task<ServiceResponse> Pay(
        string userId,
        Guid pendingCheckoutId,
        decimal totalAmount,
        IEnumerable<CartProduct> products,
        IEnumerable<ProcessCart> cart);
}
