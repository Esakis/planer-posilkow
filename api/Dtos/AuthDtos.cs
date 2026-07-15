using System.ComponentModel.DataAnnotations;

namespace TaniTydzien.Api.Dtos;

public record RegisterRequest(
    [Required(ErrorMessage = "Podaj adres e-mail."), EmailAddress(ErrorMessage = "Nieprawidłowy adres e-mail.")] string Email,
    [Required(ErrorMessage = "Podaj hasło."), MinLength(8, ErrorMessage = "Hasło musi mieć co najmniej 8 znaków.")] string Password);

/// <summary>DevActivationLink zwracany tylko w środowisku Development (mail nie jest realnie wysyłany).</summary>
public record RegisterResponse(string Message, string? DevActivationLink);

public record LoginRequest(
    [Required(ErrorMessage = "Podaj adres e-mail.")] string Email,
    [Required(ErrorMessage = "Podaj hasło.")] string Password);

public record ActivateRequest([Required(ErrorMessage = "Brak tokenu aktywacyjnego.")] string Token);

/// <summary>Plan: "free" (przed aktywacją maila) | "trial" | "premium" | "expired".</summary>
public record AccountDto(
    string Email,
    bool EmailVerified,
    string Plan,
    bool Active,
    int TrialDaysLeft,
    DateTime? TrialEndsAt);

public record LoginResponse(string Token, AccountDto Account);

public record CheckoutResponse(string Url);

public record ConfirmPaymentRequest([Required(ErrorMessage = "Brak identyfikatora sesji płatności.")] string SessionId);
