using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.Estoques.DTOs.Requests
{
    public record EstoqueRequestDTO
    {
        [Required]
        public Guid PecaId { get; set; }
        [Required]
        public string TipoMovimentacao { get; set; }
        [Required]
        public int Quantidade { get; set; }
    }
}
