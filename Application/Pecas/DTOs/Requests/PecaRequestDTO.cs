using System.ComponentModel.DataAnnotations;

namespace Application.Pecas.DTOs.Requests
{
    public record PecaRequestDTO
    {
        [Required]
        public string Nome { get; set; }
        [Required]
        public string Descricao { get; set; }
        [Required]
        public string MarcaPeca { get; set; }
        [Required]
        public decimal PrecoVenda { get; set; }
    }
}
