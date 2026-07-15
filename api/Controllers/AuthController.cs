using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaniTydzien.Api.Data;
using TaniTydzien.Api.Dtos;
using TaniTydzien.Api.Models;
using TaniTydzien.Api.Services;

namespace TaniTydzien.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly TokenService _tokens;
    private readonly IEmailSender _mail;
    private readonly IConfiguration _cfg;

    public AuthController(AppDbContext db, TokenService tokens, IEmailSender mail, IConfiguration cfg)
    {
        _db = db;
        _tokens = tokens;
        _mail = mail;
        _cfg = cfg;
    }

    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterRequest req)
    {
        var email = req.Email.Trim().ToLowerInvariant();
        if (await _db.Users.AnyAsync(u => u.Email == email))
            return BadRequest(new { message = "Konto z tym adresem e-mail już istnieje. Zaloguj się." });

        var user = new User
        {
            Email = email,
            PasswordHash = PasswordHasher.Hash(req.Password),
            ActivationToken = Guid.NewGuid().ToString("N"),
            CreatedAt = DateTime.UtcNow
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var baseUrl = (_cfg["App:BaseUrl"] ?? "http://localhost:4200").TrimEnd('/');
        var link = $"{baseUrl}/activate?token={user.ActivationToken}";
        await _mail.SendActivationAsync(email, link);

        // link ujawniamy tylko, gdy maile nie są realnie wysyłane (tryb deweloperski)
        return Ok(new RegisterResponse(
            "Konto utworzone. Kliknij link aktywacyjny z maila, aby rozpocząć 30 dni za darmo.",
            _mail is DevEmailSender ? link : null));
    }

    [HttpPost("activate")]
    public async Task<ActionResult<AccountDto>> Activate([FromBody] ActivateRequest req)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.ActivationToken == req.Token);
        if (user is null)
            return BadRequest(new { message = "Link aktywacyjny jest nieprawidłowy lub został już użyty." });

        user.EmailVerified = true;
        user.ActivationToken = null;
        user.TrialEndsAt = DateTime.UtcNow.AddDays(30);
        await _db.SaveChangesAsync();

        return Ok(ToAccount(user));
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest req)
    {
        var email = req.Email.Trim().ToLowerInvariant();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        // 400 (nie 401) — 401 jest zarezerwowane dla wygasłej sesji i przekierowuje na login
        if (user is null || !PasswordHasher.Verify(req.Password, user.PasswordHash))
            return BadRequest(new { message = "Nieprawidłowy e-mail lub hasło." });

        return Ok(new LoginResponse(_tokens.Create(user), ToAccount(user)));
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<AccountDto>> Me()
    {
        var user = await CurrentUserAsync();
        if (user is null) return Unauthorized(new { message = "Sesja wygasła. Zaloguj się ponownie." });
        return Ok(ToAccount(user));
    }

    private async Task<User?> CurrentUserAsync()
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(idClaim, out var id) ? await _db.Users.FindAsync(id) : null;
    }

    internal static AccountDto ToAccount(User u)
    {
        var status = PlanService.Status(u, DateTime.UtcNow);
        return new AccountDto(u.Email, u.EmailVerified, status.Plan, status.Active, status.TrialDaysLeft, u.TrialEndsAt);
    }
}
