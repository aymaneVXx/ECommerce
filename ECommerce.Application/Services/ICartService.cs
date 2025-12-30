using ECommerce.Application.DTOs.Cart;
using ECommerce.Application.Responses;

namespace ECommerce.Application.Services;

public interface ICartService
{
    Task<ServiceResponse> CheckoutAsync(string userId, CheckoutDto dto);

    Task<ServiceResponse> SaveCheckoutHistoryAsync(
        string userId,
        IEnumerable<CreateCheckoutArchiveDto> archives,
        string? stripeSessionId = null);

    Task<List<GetCheckoutArchiveDto>> GetCheckoutHistoryAsync();
}
