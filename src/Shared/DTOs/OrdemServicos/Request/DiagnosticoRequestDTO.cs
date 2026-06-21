using Shared.DTOs.OrdemServicos.Shared;
using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.OrdemServicos.Request
{
    public record DiagnosticoRequestDTO
    {
        public ICollection<OrdemServicoServicoRequestDTO> servicos { get; init; }
        public ICollection<OrdemServicoPecaRequestDTO> pecas { get; init; }
        public ICollection<OrdemServicoInsumoRequestDTO> insumos { get; init; }

        [MaxLength(500, ErrorMessage = "O campo Observação deve conter no máximo 500 caracteres.")]
        public string? Observacao { get; init; }
    }
}
