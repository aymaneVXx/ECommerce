using ECommerce.Application.DTOs.Cart;
using ECommerce.Application.Responses;

namespace ECommerce.Application.Services;

public interface ICheckoutPaymentService
{
    Task<ServiceResponse> Pay(decimal totalAmount, IEnumerable<CartProduct> products, IEnumerable<ProcessCart> cart);
}
