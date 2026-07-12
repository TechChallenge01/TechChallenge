using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.Estoques.Request
{
    public record EstoqueRequestDTO
    {
        public Guid? PecaId { get; init; }
        public Guid? InsumoId { get; init; }

        [Required]
        public string TipoMovimentacao { get; init; }
        [Required]
        public int Quantidade { get; init; }
    }
}
