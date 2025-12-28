using ECommerce.Application.DTOs.Cart;
using ECommerce.Application.Responses;

namespace ECommerce.Application.Services;

public interface ICartService
{
    Task<ServiceResponse> SaveCheckoutHistoryAsync(IEnumerable<CreateCheckoutArchiveDto> archives);
    Task<List<GetCheckoutArchiveDto>> GetCheckoutHistoryAsync();
}
