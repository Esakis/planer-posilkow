using System.ComponentModel.DataAnnotations;

namespace TaniTydzien.Api.Dtos;

public record SavedListItemInput(
    [Range(1, int.MaxValue, ErrorMessage = "Nieprawidłowy składnik.")] int IngredientId,
    [Range(1, 50_000, ErrorMessage = "Ilość musi być między 1 a 50 000 g.")] double Grams,
    bool Checked);

public record CreateSavedListRequest(
    [Required(ErrorMessage = "Podaj nazwę listy."), MinLength(1, ErrorMessage = "Podaj nazwę listy."), MaxLength(60, ErrorMessage = "Nazwa listy może mieć maks. 60 znaków.")] string Name,
    SavedListItemInput[]? Items = null);

/// <summary>Pełny zapis stanu listy — nazwa i wszystkie pozycje (zastępują poprzednie).</summary>
public record UpdateSavedListRequest(
    [Required(ErrorMessage = "Podaj nazwę listy."), MinLength(1, ErrorMessage = "Podaj nazwę listy."), MaxLength(60, ErrorMessage = "Nazwa listy może mieć maks. 60 znaków.")] string Name,
    [Required] SavedListItemInput[] Items);

public record SavedListItemDto(int IngredientId, string Name, string Aisle, double Grams, bool Checked);

public record SavedListDto(int Id, string Name, DateTime UpdatedAt, SavedListItemDto[] Items);
