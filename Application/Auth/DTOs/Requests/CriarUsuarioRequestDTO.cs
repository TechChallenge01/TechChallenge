using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.Auth.DTOs.Requests;

public record CriarUsuarioRequestDTO
{
    [Required(ErrorMessage = "Nome é obrigatório.")]
    public string Nome { get; init; }

    [Required(ErrorMessage = "Email é obrigatório.")]
    [EmailAddress(ErrorMessage = "Email inválido.")]
    public string Email { get; init; }

    [Required(ErrorMessage = "Senha é obrigatória.")]
    [MinLength(8, ErrorMessage = "Senha deve ter ao menos 6 caracteres.")]
    public string Senha { get; init; }

    [Required(ErrorMessage = "Perfil é obrigatório.")]
    public EPerfilUsuario Perfil { get; init; }
}
