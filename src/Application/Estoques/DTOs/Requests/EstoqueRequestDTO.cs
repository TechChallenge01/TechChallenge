using System.ComponentModel.DataAnnotations;

namespace Application.Estoques.DTOs.Requests
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
