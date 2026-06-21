using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.OrdemServicos.Request
{
    public record FinalizarServicoRequestDTO
    {
        [Required]
        public ICollection<Guid> servicosId { get; init; } = new List<Guid>();
    }
}
