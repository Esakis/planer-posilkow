using TaniTydzien.Api.Models;

namespace TaniTydzien.Api.Services;

/// <summary>Efektywny stan planu użytkownika w danym momencie.</summary>
public record PlanStatus(string Plan, bool Active, int TrialDaysLeft);

/// <summary>Wylicza efektywny plan z pól konta — jedno źródło prawdy dla paywalla i UI.</summary>
public static class PlanService
{
    /// <summary>
    /// "premium" — opłacona subskrypcja; "trial" — aktywny okres próbny;
    /// "free" — konto przed aktywacją maila; "expired" — trial wykorzystany i nieopłacony.
    /// </summary>
    public static PlanStatus Status(User u, DateTime nowUtc)
    {
        if (u.Plan == "premium")
            return new PlanStatus("premium", Active: true, TrialDaysLeft: 0);

        if (u.EmailVerified && u.TrialEndsAt is { } end && end > nowUtc)
        {
            var daysLeft = (int)Math.Ceiling((end - nowUtc).TotalDays);
            return new PlanStatus("trial", Active: true, daysLeft);
        }

        return new PlanStatus(u.EmailVerified ? "expired" : "free", Active: false, TrialDaysLeft: 0);
    }
}
