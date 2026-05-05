using System.ComponentModel.DataAnnotations;

namespace Application.OrdemServicos.DTOs.Requests;

public record DiagnosticoRequestDTO
{
    public ICollection<OrdemServicoPecaRequestDTO>? Pecas { get; init; }
    public ICollection<OrdemServicoServicoRequestDTO>? Servicos { get; init; }
    public ICollection<OrdemServicoInsumoRequestDTO>? Insumos { get; init; }

    [MaxLength(500, ErrorMessage = "O campo Observação deve conter no máximo 500 caracteres.")]
    public string? Observacao { get; init; }
}
