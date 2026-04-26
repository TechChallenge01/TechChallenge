using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Application.OrdemServicos.DTOs.Requests;

public record OrdemServicoPecaRequestDTO
{
    [Required(ErrorMessage = "O campo PecaId é obrigatório.")]
    public Guid PecaId { get; set; }

    [Required(ErrorMessage = "O campo Quantidade é obrigatório.")]
    public int Quantidade { get; set; }
}
