using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TaniTydzien.Api.Data;
using TaniTydzien.Api.Services;

namespace TaniTydzien.Api.Filters;

/// <summary>
/// Paywall: przepuszcza tylko użytkowników z aktywnym planem (trial lub premium).
/// 401 — brak/nieprawidłowy token, 402 — konto bez aktywnego planu.
/// </summary>
public class RequireActivePlanAttribute : TypeFilterAttribute
{
    public RequireActivePlanAttribute() : base(typeof(RequireActivePlanFilter)) { }
}

public class RequireActivePlanFilter : IAsyncActionFilter
{
    private readonly AppDbContext _db;

    public RequireActivePlanFilter(AppDbContext db) => _db = db;

    public async Task OnActionExecutionAsync(ActionExecutingContext ctx, ActionExecutionDelegate next)
    {
        var idClaim = ctx.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = int.TryParse(idClaim, out var id) ? await _db.Users.FindAsync(id) : null;
        if (user is null)
        {
            ctx.Result = new UnauthorizedObjectResult(new { message = "Zaloguj się, aby korzystać z planera." });
            return;
        }

        var status = PlanService.Status(user, DateTime.UtcNow);
        if (!status.Active)
        {
            var message = status.Plan == "free"
                ? "Aktywuj konto linkiem z maila, aby rozpocząć 30-dniowy darmowy okres próbny."
                : "Twój okres próbny się skończył. Przejdź na premium (9 zł/mies.), aby dalej korzystać z planera.";
            ctx.Result = new ObjectResult(new { message }) { StatusCode = StatusCodes.Status402PaymentRequired };
            return;
        }

        await next();
    }
}
