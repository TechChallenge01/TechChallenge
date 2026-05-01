using System.ComponentModel.DataAnnotations;

namespace Application.Insumos.DTOs.Requests
{
    public class InsumoRequestDTO
    {
        [Required]
        public string Nome { get; init; }
        public string Descricao { get; init; }
        [Required]
        public decimal CustoUnitario { get; init; }
    }
}
