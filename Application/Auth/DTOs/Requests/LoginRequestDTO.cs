using System.ComponentModel.DataAnnotations;

namespace Application.Auth.DTOs.Requests;

public record LoginRequestDTO
{
    [Required(ErrorMessage = "Email é obrigatório.")]
    [EmailAddress(ErrorMessage = "Email inválido.")]
    public string Email { get; init; }

    [Required(ErrorMessage = "Senha é obrigatória.")]
    public string Senha { get; init; }
}