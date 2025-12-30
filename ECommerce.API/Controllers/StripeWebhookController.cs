using System.Text.Json;
using ECommerce.Application.DTOs.Cart;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/webhooks/stripe")]
[AllowAnonymous]
public class StripeWebhookController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly ICartRepository _cartRepo;
    private readonly IProductRepository _productRepo;
    private readonly ILogger<StripeWebhookController> _logger;

    public StripeWebhookController(
        IConfiguration config,
        ICartRepository cartRepo,
        IProductRepository productRepo,
        ILogger<StripeWebhookController> logger)
    {
        _config = config;
        _cartRepo = cartRepo;
        _productRepo = productRepo;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Handle()
    {
        var webhookSecret = _config["Stripe:WebhookSecret"];

        _logger.LogInformation("Webhook hit");
        _logger.LogInformation("WebhookSecret configured: {Ok}", !string.IsNullOrWhiteSpace(webhookSecret));
        _logger.LogInformation("Stripe-Signature header present: {Ok}", Request.Headers.ContainsKey("Stripe-Signature"));

        if (string.IsNullOrWhiteSpace(webhookSecret))
            return BadRequest("Webhook secret not configured.");

        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var stripeSignature = Request.Headers["Stripe-Signature"].ToString();

        Event stripeEvent;
        try
        {
            stripeEvent = EventUtility.ConstructEvent(
                json,
                stripeSignature,
                webhookSecret,
                throwOnApiVersionMismatch: false
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Stripe signature verification failed.");
            return BadRequest("Invalid Stripe signature.");
        }

        _logger.LogInformation("Stripe event received: {Type}", stripeEvent.Type);

        // on supporte aussi async_payment_succeeded
        if (stripeEvent.Type != "checkout.session.completed" &&
            stripeEvent.Type != "checkout.session.async_payment_succeeded")
        {
            return Ok();
        }

        var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
        if (session is null) return Ok();

        // Pour la carte classique, payment_status doit être paid
        if (!string.Equals(session.PaymentStatus, "paid", StringComparison.OrdinalIgnoreCase))
            return Ok();

        var sessionId = session.Id;
        if (string.IsNullOrWhiteSpace(sessionId))
            return Ok();

        // idempotence simple (en plus du process atomique)
        if (await _cartRepo.IsStripeSessionProcessedAsync(sessionId))
            return Ok();

        if (session.Metadata is null)
            return Ok();

        if (!session.Metadata.TryGetValue("userId", out var userId) || string.IsNullOrWhiteSpace(userId))
            return Ok();

        if (!session.Metadata.TryGetValue("pendingCheckoutId", out var pendingIdStr) || string.IsNullOrWhiteSpace(pendingIdStr))
            return Ok();

        if (!Guid.TryParse(pendingIdStr, out var pendingId))
            return Ok();

        var pending = await _cartRepo.GetPendingCheckoutAsync(pendingId);
        if (pending is null) return Ok();

        // sécurité : pending doit appartenir au user
        if (!string.Equals(pending.UserId, userId, StringComparison.OrdinalIgnoreCase))
            return Ok();

        if (pending.Processed)
            return Ok();

        List<ProcessCart>? carts;
        try
        {
            carts = JsonSerializer.Deserialize<List<ProcessCart>>(pending.CartJson);
        }
        catch
        {
            return Ok();
        }

        if (carts is null || carts.Count == 0)
            return Ok();

        var ids = carts.Select(x => x.ProductId).Distinct().ToList();
        var products = (await _productRepo.GetProductsByIdsAsync(ids)).ToList();
        if (products.Count == 0)
            return Ok();

        var map = products.ToDictionary(p => p.Id, p => p);

        var archives = new List<CheckoutArchive>();
        foreach (var item in carts)
        {
            if (item.ProductId == Guid.Empty || item.Quantity <= 0) continue;
            if (!map.TryGetValue(item.ProductId, out var product)) continue;

            // montant recalculé serveur 
            archives.Add(new CheckoutArchive
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                AmountPaid = product.Price * item.Quantity,
                StripeSessionId = sessionId,
                DateCreated = DateTime.UtcNow
            });
        }

        if (archives.Count == 0)
            return Ok();

        var ok = await _cartRepo.TryProcessPendingCheckoutAsync(pendingId, sessionId, archives);

        if (!ok)
        {
            _logger.LogInformation("Stripe webhook ignored (already processed). SessionId={SessionId} PendingId={PendingId}", sessionId, pendingId);
            return Ok();
        }

        _logger.LogInformation("Stripe webhook processed. SessionId={SessionId} PendingId={PendingId} Items={Count}", sessionId, pendingId, archives.Count);
        return Ok();
    }
}
