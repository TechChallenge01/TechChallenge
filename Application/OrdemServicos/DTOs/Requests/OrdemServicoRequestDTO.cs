using System.ComponentModel.DataAnnotations;

namespace Application.OrdemServicos.DTOs.Requests;

public record OrdemServicoRequestDTO
{
    public string? Cpf { get; init; }
    public string? Cnpj { get; init; }

    [Required(ErrorMessage = "O campo Id do Veículo é obrigatório.")]
    public Guid VeiculoId { get; init; }

    public string? Observacao { get; init; }

    public decimal ValorDesconto { get; init; } = 0;

    public ICollection<OrdemServicoPecaRequestDTO>? Pecas { get; init; }
    public ICollection<OrdemServicoServicoRequestDTO>? Servicos { get; init; }
}
