using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.OrdemServicos.DTOs.Requests
{
    public record FinalizarServicoDTO
    {
        [Required]
        public ICollection<Guid> ServicosId {  get; init; } = new List<Guid>();
    }
}
