using System.ComponentModel.DataAnnotations;

namespace Application.OrdemServicos.DTOs.Requests;

public record OrdemServicoServicoRequestDTO
{
    [Required(ErrorMessage = "O ID do serviço é obrigatório.")]
    public Guid ServicoId { get; set; }

    [Required(ErrorMessage = "A quantidade é obrigatória.")]
    public int Quantidade { get; set; }
}
