using System.ComponentModel.DataAnnotations;

namespace Application.OrdemServicos.DTOs.Requests
{
    public record OrdemServicoInsumoRequestDTO
    {
        [Required(ErrorMessage = "O campo InsumoId é obrigatório.")]
        public Guid InsumoId { get; set; }

        [Required(ErrorMessage = "O campo Quantidade é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero.")]
        public int Quantidade { get; set; }
    }
}
