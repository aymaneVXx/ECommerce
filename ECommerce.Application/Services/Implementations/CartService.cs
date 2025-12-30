using System.Text.Json;
using ECommerce.Application.DTOs.Cart;
using ECommerce.Application.Responses;
using ECommerce.Application.Services;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using ECommerce.Application.Interfaces.Identity;

namespace ECommerce.Application.Services.Implementations;

public class CartService : ICartService
{
    private readonly ICartRepository _repo;
    private readonly IProductRepository _productRepo;
    private readonly ICheckoutPaymentService _paymentService;
    private readonly IUserManagement _userManagement;

    public CartService(
        ICartRepository repo,
        IProductRepository productRepo,
        ICheckoutPaymentService paymentService,
        IUserManagement userManagement)
    {
        _repo = repo;
        _productRepo = productRepo;
        _paymentService = paymentService;
        _userManagement = userManagement;
    }

    public async Task<ServiceResponse> CheckoutAsync(string userId, CheckoutDto dto)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return ServiceResponse.Fail("User not found.");

        if (dto is null || dto.Carts is null || dto.Carts.Count == 0)
            return ServiceResponse.Fail("No cart items provided.");

        if (dto.PaymentMethodId == Guid.Empty)
            return ServiceResponse.Fail("Payment method is required.");

        const int maxQtyPerItem = 20;
        foreach (var c in dto.Carts)
        {
            if (c.ProductId == Guid.Empty)
                return ServiceResponse.Fail("Invalid product id in cart.");

            if (c.Quantity <= 0 || c.Quantity > maxQtyPerItem)
                return ServiceResponse.Fail($"Invalid quantity. Max allowed per item is {maxQtyPerItem}.");
        }

        var ids = dto.Carts.Select(x => x.ProductId).Distinct().ToList();

        var products = (await _productRepo.GetProductsByIdsAsync(ids)).ToList();
        if (products.Count == 0)
            return ServiceResponse.Fail("No products found for the provided cart.");

        var missing = ids.Except(products.Select(p => p.Id)).ToList();
        if (missing.Count > 0)
            return ServiceResponse.Fail("Some products in cart were not found.");

        // stock check avant de créer la session Stripe
        foreach (var p in products)
        {
            var qty = dto.Carts.First(x => x.ProductId == p.Id).Quantity;
            if (p.Quantity < qty)
                return ServiceResponse.Fail($"Insufficient stock for product '{p.Name}'.");
        }

        var cartProducts = products.Select(p => new CartProduct
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price
        }).ToList();

        decimal totalAmount = 0;
        foreach (var p in products)
        {
            var qty = dto.Carts.First(x => x.ProductId == p.Id).Quantity;
            totalAmount += p.Price * qty;
        }

        // ✅ PendingCheckout côté serveur
        var pending = new PendingCheckout
        {
            UserId = userId,
            CartJson = JsonSerializer.Serialize(dto.Carts),
            Processed = false
        };

        var pendingId = await _repo.CreatePendingCheckoutAsync(pending);

        return await _paymentService.Pay(userId, pendingId, totalAmount, cartProducts, dto.Carts);
    }

    public async Task<ServiceResponse> SaveCheckoutHistoryAsync(string userId, IEnumerable<CreateCheckoutArchiveDto> archives, string? stripeSessionId = null)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return ServiceResponse.Fail("User not found.");

        var list = archives?.ToList() ?? new List<CreateCheckoutArchiveDto>();
        if (list.Count == 0)
            return ServiceResponse.Fail("No cart items provided.");

        var entities = list.Select(x => new CheckoutArchive
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ProductId = x.ProductId,
            Quantity = x.Quantity,
            AmountPaid = x.AmountPaid,
            StripeSessionId = stripeSessionId,
            DateCreated = DateTime.UtcNow
        });

        await _repo.SaveCheckoutHistoryAsync(entities);
        return ServiceResponse.Ok("Checkout history saved.");
    }

    public async Task<List<GetCheckoutArchiveDto>> GetCheckoutHistoryAsync()
    {
        var archives = await _repo.GetCheckoutHistoryAsync();
        if (archives is null || archives.Count == 0)
            return new List<GetCheckoutArchiveDto>();

        var result = new List<GetCheckoutArchiveDto>();

        foreach (var a in archives)
        {
            var user = await _userManagement.GetUserById(a.UserId);

            result.Add(new GetCheckoutArchiveDto
            {
                CustomerName = user?.FullName ?? string.Empty,
                CustomerEmail = user?.Email ?? string.Empty,
                ProductName = a.Product?.Name ?? string.Empty,
                Quantity = a.Quantity,
                AmountPaid = a.AmountPaid,
                DateCreated = a.DateCreated
            });
        }

        return result;
    }
}
