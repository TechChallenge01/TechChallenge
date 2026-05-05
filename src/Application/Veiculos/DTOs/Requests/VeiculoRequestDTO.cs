using System.ComponentModel.DataAnnotations;

namespace Application.Veiculos.DTOs.Requests
{
    public class VeiculoRequestDTO
    {
        [Required]
        public string Modelo { get; init; }
        [Required]
        public string MarcaVeiculo { get; init; }
        [Required]
        public Guid ClienteId { get; init; }
        [Required]
        public int Ano { get; init; }
        [Required]
        public string Placa { get; init; }
        [Required]  
        public string Cor { get; init; }
    }
}
