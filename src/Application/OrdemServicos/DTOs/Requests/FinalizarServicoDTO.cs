using System.ComponentModel.DataAnnotations;

namespace Application.OrdemServicos.DTOs.Requests
{
    public record FinalizarServicoDTO
    {
        [Required]
        public ICollection<Guid> ServicosId {  get; init; } = new List<Guid>();
    }
}
