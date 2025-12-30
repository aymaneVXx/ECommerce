using ECommerce.Application.DTOs.Cart;
using ECommerce.Application.Responses;

namespace ECommerce.Application.Services;

public interface ICartService
{
    Task<ServiceResponse> CheckoutAsync(CheckoutDto dto);

    Task<ServiceResponse> SaveCheckoutHistoryAsync(string userId, IEnumerable<CreateCheckoutArchiveDto> archives);
    Task<List<GetCheckoutArchiveDto>> GetCheckoutHistoryAsync();
}


