using System.ComponentModel.DataAnnotations;

namespace Application.OrdemServicos.DTOs.Requests;

public record OrdemServicoServicoRequestDTO
{
    [Required(ErrorMessage = "O ID do serviço é obrigatório.")]
    public Guid ServicoId { get; init; }

    [Required(ErrorMessage = "A quantidade é obrigatória.")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero.")]
    public int Quantidade { get; init; }
}
