using Shared.DTOs.Clientes.Request;
using Shared.DTOs.OrdemServicos.Shared;
using Shared.DTOs.Veiculos.Requests;

namespace Shared.DTOs.OrdemServicos.Request
{
    public class OrdemServicoRequestDTO
    {
        public ClienteRequestDTO Cliente { get; init; }
        public VeiculoRequestDTO Veiculo { get; init; }
        public ICollection<OrdemServicoInsumoRequestDTO>? Insumos { get; set; } = new List<OrdemServicoInsumoRequestDTO>();
        public ICollection<OrdemServicoPecaRequestDTO>? Pecas { get; set; } = new List<OrdemServicoPecaRequestDTO>();
        public ICollection<OrdemServicoServicoRequestDTO>? Servicos { get; set; } = new List<OrdemServicoServicoRequestDTO>();
        public string? Observacao { get; init; }
        public decimal ValorDesconto { get; init; } = 0;
    }
}
