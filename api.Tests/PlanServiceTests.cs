using TaniTydzien.Api.Models;
using TaniTydzien.Api.Services;

namespace TaniTydzien.Api.Tests;

public class PlanServiceTests
{
    private static readonly DateTime Now = new(2026, 7, 15, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Konto_przed_aktywacja_maila_jest_nieaktywne()
    {
        var user = new User { EmailVerified = false };

        var status = PlanService.Status(user, Now);

        Assert.Equal("free", status.Plan);
        Assert.False(status.Active);
    }

    [Fact]
    public void Aktywny_trial_daje_dostep_i_liczbe_dni()
    {
        var user = new User { EmailVerified = true, TrialEndsAt = Now.AddDays(10) };

        var status = PlanService.Status(user, Now);

        Assert.Equal("trial", status.Plan);
        Assert.True(status.Active);
        Assert.Equal(10, status.TrialDaysLeft);
    }

    [Fact]
    public void Koncowka_triala_zaokragla_dni_w_gore()
    {
        var user = new User { EmailVerified = true, TrialEndsAt = Now.AddHours(3) };

        var status = PlanService.Status(user, Now);

        Assert.Equal("trial", status.Plan);
        Assert.Equal(1, status.TrialDaysLeft);
    }

    [Fact]
    public void Wygasly_trial_blokuje_dostep()
    {
        var user = new User { EmailVerified = true, TrialEndsAt = Now.AddDays(-1) };

        var status = PlanService.Status(user, Now);

        Assert.Equal("expired", status.Plan);
        Assert.False(status.Active);
    }

    [Fact]
    public void Premium_daje_dostep_niezaleznie_od_triala()
    {
        var user = new User { Plan = "premium", EmailVerified = true, TrialEndsAt = Now.AddDays(-30) };

        var status = PlanService.Status(user, Now);

        Assert.Equal("premium", status.Plan);
        Assert.True(status.Active);
    }
}
