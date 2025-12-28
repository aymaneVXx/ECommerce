using ECommerce.Application.DTOs.Cart;
using ECommerce.Application.Responses;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;

namespace ECommerce.Application.Services.Implementations;

public class CartService : ICartService
{
    private readonly ICartRepository _repo;

    public CartService(ICartRepository repo)
    {
        _repo = repo;
    }

    public async Task<ServiceResponse> SaveCheckoutHistoryAsync(IEnumerable<CreateCheckoutArchiveDto> archives)
    {
        var list = archives?.ToList() ?? new List<CreateCheckoutArchiveDto>();
        if (list.Count == 0)
            return ServiceResponse.Fail("No cart items provided.");

        var entities = list.Select(x => new CheckoutArchive
        {
            Id = Guid.NewGuid(),
            UserId = x.UserId,
            ProductId = x.ProductId,
            Quantity = x.Quantity,
            DateCreated = DateTime.UtcNow
        });

        await _repo.SaveCheckoutHistoryAsync(entities);
        return ServiceResponse.Ok("Checkout history saved.");
    }

    public async Task<List<GetCheckoutArchiveDto>> GetCheckoutHistoryAsync()
    {
        var archives = await _repo.GetCheckoutHistoryAsync();

        return archives.Select(x => new GetCheckoutArchiveDto
        {
            CustomerName = "",
            CustomerEmail = "",
            ProductName = "",
            Quantity = x.Quantity,
            AmountPaid = 0,
            DateCreated = x.DateCreated
        }).ToList();
    }
}
