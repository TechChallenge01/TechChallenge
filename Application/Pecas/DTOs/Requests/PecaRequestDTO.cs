using System.ComponentModel.DataAnnotations;

namespace Application.Pecas.DTOs.Requests
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
