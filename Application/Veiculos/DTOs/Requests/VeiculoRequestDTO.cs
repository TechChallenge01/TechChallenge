using System.ComponentModel.DataAnnotations;

namespace Application.Veiculos.DTOs.Requests
{
    public class VeiculoRequestDTO
    {
        [Required]
        public string Modelo { get; set; }
        [Required]
        public string MarcaVeiculo { get; set; }
        [Required]
        public Guid ClienteId { get; set; }
        [Required]
        public int Ano { get; set; }
        [Required]
        public string Placa { get; set; }
        [Required]  
        public string Cor { get; set; }
    }
}
