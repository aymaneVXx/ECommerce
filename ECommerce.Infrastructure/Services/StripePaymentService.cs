using ECommerce.Application.DTOs.Cart;
using ECommerce.Application.Responses;
using ECommerce.Application.Services;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;

namespace ECommerce.Infrastructure.Services;

public class StripePaymentService : ICheckoutPaymentService
{
    private readonly IConfiguration _config;

    public StripePaymentService(IConfiguration config)
    {
        _config = config;
        StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
    }

    public async Task<ServiceResponse> Pay(
        string userId,
        Guid pendingCheckoutId,
        decimal totalAmount,
        IEnumerable<CartProduct> products,
        IEnumerable<ProcessCart> cart)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userId))
                return ServiceResponse.Fail("User not found.");

            if (pendingCheckoutId == Guid.Empty)
                return ServiceResponse.Fail("Invalid checkout id.");

            var cartList = cart?.ToList() ?? new List<ProcessCart>();
            if (cartList.Count == 0)
                return ServiceResponse.Fail("Cart is empty.");

            var lineItems = new List<SessionLineItemOptions>();

            foreach (var product in products)
            {
                var qty = cartList.FirstOrDefault(x => x.ProductId == product.Id)?.Quantity ?? 1;

                lineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = product.Name,
                            Description = product.Description,
                            Metadata = new Dictionary<string, string>
                            {
                                ["productId"] = product.Id.ToString()
                            }
                        },
                        UnitAmount = (long)(product.Price * 100m)
                    },
                    Quantity = qty
                });
            }

            var clientUrl = _config["Client:BaseUrl"] ?? "https://localhost:7025";

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = $"{clientUrl}/payment-success",
                CancelUrl = $"{clientUrl}/payment-cancel",

                // lien session (user + pendingCheckout)
                ClientReferenceId = userId,
                Metadata = new Dictionary<string, string>
                {
                    ["userId"] = userId,
                    ["pendingCheckoutId"] = pendingCheckoutId.ToString()
                }
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            return ServiceResponse.Ok(session.Url ?? string.Empty);
        }
        catch (Exception ex)
        {
            return ServiceResponse.Fail(ex.Message);
        }
    }
}
