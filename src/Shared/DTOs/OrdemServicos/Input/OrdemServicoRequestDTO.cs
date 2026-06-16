using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.OrdemServicos.Input
{
    public class OrdemServicoRequestDTO
    {
        [Required(ErrorMessage = "O Cliente é obrigatório")]
        public Guid ClienteId { get; set; }

        [Required(ErrorMessage = "O Veículo é obrigatório")]
        public Guid VeiculoId { get; set; }
    }

    public class OrdemServicoUpdateRequestDTO
    {
        public string? StatusOS { get; set; }
        public string? Observacao { get; set; }
        public decimal? ValorDesconto { get; set; }
    }
}
