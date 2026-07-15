using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using TaniTydzien.Api.Data;
using TaniTydzien.Api.Dtos;
using TaniTydzien.Api.Models;
using TaniTydzien.Api.Services;

namespace TaniTydzien.Api.Controllers;

/// <summary>Subskrypcja premium 9 zł/mies. przez Stripe Checkout.</summary>
[ApiController]
[Route("api/billing")]
public class BillingController : ControllerBase
{
    /// <summary>Klucz, po którym odnajdujemy (lub zakładamy) cenę 9 zł/mies. na koncie Stripe.</summary>
    private const string PriceLookupKey = "tanitydzien_premium_9";

    private readonly AppDbContext _db;
    private readonly IConfiguration _cfg;
    private readonly ILogger<BillingController> _log;

    public BillingController(AppDbContext db, IConfiguration cfg, ILogger<BillingController> log)
    {
        _db = db;
        _cfg = cfg;
        _log = log;
    }

    private bool StripeConfigured => !string.IsNullOrWhiteSpace(_cfg["Stripe:SecretKey"]);

    [Authorize]
    [HttpPost("checkout")]
    public async Task<ActionResult<CheckoutResponse>> Checkout()
    {
        if (!StripeConfigured)
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                message = "Płatności nie są jeszcze skonfigurowane. Dodaj testowe klucze Stripe w api/appsettings.json (sekcja Stripe)."
            });

        var user = await CurrentUserAsync();
        if (user is null) return Unauthorized(new { message = "Sesja wygasła. Zaloguj się ponownie." });

        var baseUrl = (_cfg["App:BaseUrl"] ?? "http://localhost:4200").TrimEnd('/');
        try
        {
            var session = await new SessionService().CreateAsync(new SessionCreateOptions
            {
                Mode = "subscription",
                LineItems = new List<SessionLineItemOptions>
                {
                    new() { Price = await ResolvePriceIdAsync(), Quantity = 1 }
                },
                ClientReferenceId = user.Id.ToString(),
                Customer = user.StripeCustomerId,
                CustomerEmail = user.StripeCustomerId is null ? user.Email : null,
                // session_id pozwala potwierdzić płatność bez webhooka (lokalny dev bez Stripe CLI)
                SuccessUrl = $"{baseUrl}/account?session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"{baseUrl}/account?payment=cancel"
            });
            return Ok(new CheckoutResponse(session.Url));
        }
        catch (StripeException e)
        {
            _log.LogError(e, "Stripe: nie udało się utworzyć sesji Checkout");
            return StatusCode(StatusCodes.Status502BadGateway,
                new { message = $"Stripe odrzucił żądanie: {e.StripeError?.Message ?? e.Message}" });
        }
    }

    /// <summary>Potwierdzenie po powrocie z Checkout — działa też bez webhooka (lokalny dev).</summary>
    [Authorize]
    [HttpPost("confirm")]
    public async Task<ActionResult<AccountDto>> Confirm([FromBody] ConfirmPaymentRequest req)
    {
        if (!StripeConfigured)
            return StatusCode(StatusCodes.Status503ServiceUnavailable,
                new { message = "Płatności nie są jeszcze skonfigurowane." });

        var user = await CurrentUserAsync();
        if (user is null) return Unauthorized(new { message = "Sesja wygasła. Zaloguj się ponownie." });

        Session session;
        try
        {
            session = await new SessionService().GetAsync(req.SessionId);
        }
        catch (StripeException e)
        {
            return BadRequest(new { message = $"Nie znaleziono sesji płatności: {e.StripeError?.Message ?? e.Message}" });
        }

        if (session.ClientReferenceId != user.Id.ToString())
            return BadRequest(new { message = "Ta płatność nie należy do Twojego konta." });
        if (session.PaymentStatus != "paid")
            return BadRequest(new { message = "Płatność nie została jeszcze zaksięgowana." });

        ApplyPremium(user, session);
        await _db.SaveChangesAsync();
        return Ok(AuthController.ToAccount(user));
    }

    /// <summary>Webhook Stripe — źródło prawdy o subskrypcji (opłacenie, anulowanie).</summary>
    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook()
    {
        var json = await new StreamReader(Request.Body).ReadToEndAsync();
        Event stripeEvent;
        try
        {
            var secret = _cfg["Stripe:WebhookSecret"];
            stripeEvent = string.IsNullOrWhiteSpace(secret)
                // bez sekretu (lokalny dev) nie weryfikujemy podpisu — na produkcji sekret jest wymagany
                ? EventUtility.ParseEvent(json)
                : EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], secret);
        }
        catch (StripeException e)
        {
            _log.LogWarning(e, "Stripe webhook: nieprawidłowy podpis lub treść");
            return BadRequest();
        }

        switch (stripeEvent.Type)
        {
            case "checkout.session.completed" when stripeEvent.Data.Object is Session session:
            {
                if (int.TryParse(session.ClientReferenceId, out var userId) &&
                    await _db.Users.FindAsync(userId) is { } user)
                {
                    ApplyPremium(user, session);
                    await _db.SaveChangesAsync();
                    _log.LogInformation("Stripe: użytkownik {Email} przeszedł na premium", user.Email);
                }
                break;
            }
            case "customer.subscription.deleted" when stripeEvent.Data.Object is Subscription sub:
            {
                var user = await _db.Users.FirstOrDefaultAsync(u => u.StripeSubscriptionId == sub.Id);
                if (user is not null)
                {
                    user.Plan = "free";
                    user.StripeSubscriptionId = null;
                    await _db.SaveChangesAsync();
                    _log.LogInformation("Stripe: subskrypcja {Email} anulowana", user.Email);
                }
                break;
            }
        }

        return Ok();
    }

    private static void ApplyPremium(User user, Session session)
    {
        user.Plan = "premium";
        user.StripeCustomerId = session.CustomerId;
        user.StripeSubscriptionId = session.SubscriptionId;
    }

    /// <summary>Price ID z konfiguracji, a gdy brak — znajduje lub zakłada cenę 9 zł/mies. po lookup key.</summary>
    private async Task<string> ResolvePriceIdAsync()
    {
        var configured = _cfg["Stripe:PriceId"];
        if (!string.IsNullOrWhiteSpace(configured)) return configured;

        var prices = new PriceService();
        var found = await prices.ListAsync(new PriceListOptions
        {
            LookupKeys = new List<string> { PriceLookupKey },
            Active = true
        });
        if (found.Data.FirstOrDefault() is { } existing) return existing.Id;

        var created = await prices.CreateAsync(new PriceCreateOptions
        {
            UnitAmount = 900, // 9,00 zł
            Currency = "pln",
            Recurring = new PriceRecurringOptions { Interval = "month" },
            LookupKey = PriceLookupKey,
            ProductData = new PriceProductDataOptions { Name = "TaniTydzień Premium" }
        });
        return created.Id;
    }

    private async Task<User?> CurrentUserAsync()
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(idClaim, out var id) ? await _db.Users.FindAsync(id) : null;
    }
}
