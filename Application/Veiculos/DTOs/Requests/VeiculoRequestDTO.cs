using System.ComponentModel.DataAnnotations;

namespace Application.Veiculos.DTOs.Requests
{
    public class VeiculoRequestDTO
    {
        [Required]
        public string Modelo { get; private set; }
        [Required]
        public string MarcaVeiculo { get; private set; }
        [Required]
        public Guid ClienteId { get; private set; }
        [Required]
        public int Ano { get; private set; }
        [Required]
        public string Placa { get; private set; }
        [Required]  
        public string Cor { get; private set; }
    }
}
