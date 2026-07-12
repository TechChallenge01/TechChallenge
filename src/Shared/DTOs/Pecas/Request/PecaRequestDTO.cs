using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.Pecas.Request
{
    public record PecaRequestDTO
    {
        [Required]
        public string Nome { get; init; }
        [Required]
        public string Descricao { get; init; }
        [Required]
        public string MarcaPeca { get; init; }
        [Required]
        public decimal PrecoVenda { get; init; }
    }
}
