using System.ComponentModel.DataAnnotations;

namespace Application.OrdemServicos.DTOs.Requests;

public record OrdemServicoRequestDTO
{
    [Required(ErrorMessage = "O campo Id do Cliente é obrigatório.")]
    public Guid ClienteId { get; set; }

    [Required(ErrorMessage = "O campo Id do Veículo é obrigatório.")]
    public Guid VeiculoId { get; set; }

    public string? Observacao { get; set; }

    public decimal ValorDesconto { get; set; } = 0;

    public ICollection<OrdemServicoPecaRequestDTO>? Pecas { get; init; }
    public ICollection<OrdemServicoServicoRequestDTO>? Servicos { get; init; }
}
