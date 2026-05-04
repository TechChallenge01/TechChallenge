using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Application.OrdemServicos.DTOs.Requests;

public record OrdemServicoPecaRequestDTO
{
    [Required(ErrorMessage = "O campo PecaId é obrigatório.")]
    public Guid PecaId { get; init; }

    [Required(ErrorMessage = "O campo Quantidade é obrigatório.")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero.")]
    public int Quantidade { get; init; }
}
