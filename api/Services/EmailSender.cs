namespace TaniTydzien.Api.Services;

/// <summary>Wysyłka e-maili transakcyjnych (aktywacja konta).</summary>
public interface IEmailSender
{
    Task SendActivationAsync(string email, string activationLink);
}

/// <summary>
/// Tryb deweloperski: zamiast wysyłać maila, loguje link aktywacyjny.
/// Docelowa wysyłka (SMTP/Resend) — podmienić rejestrację w Program.cs.
/// </summary>
public class DevEmailSender : IEmailSender
{
    private readonly ILogger<DevEmailSender> _log;

    public DevEmailSender(ILogger<DevEmailSender> log) => _log = log;

    public Task SendActivationAsync(string email, string activationLink)
    {
        _log.LogInformation("[DEV MAIL] Aktywacja konta {Email}: {Link}", email, activationLink);
        return Task.CompletedTask;
    }
}
