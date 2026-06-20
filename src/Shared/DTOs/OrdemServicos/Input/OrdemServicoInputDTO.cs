using Shared.DTOs.OrdemServicos.Shared;

namespace Shared.DTOs.OrdemServicos.Input
{
    public record OrdemServicoInputDTO
    {
        public Guid Id { get; init; }
        public Guid ClienteId { get; init; }
        public Guid VeiculoId { get; init; }
        public string StatusOS { get; init; }
        public string? Observacao { get; init; }
        public decimal ValorTotal { get; init; }
        public decimal ValorDesconto { get; init; }
        public DateTime? InicioExecucao { get; init; }
        public DateTime? TerminoExecucao { get; init; }
        public Guid IdUsuarioCriacao { get; init; }
        public DateTime DataCriacao { get; init; }
        public Guid? IdUsuarioAtualizacao { get; init; }
        public DateTime? DataAtualizacao { get; init; }
        public bool Ativo { get; init; }
        public ICollection<OrdemServicoPecaDTO>? Pecas { get; init; } = new List<OrdemServicoPecaDTO>();
        public ICollection<OrdemServicoServicoDTO>? Servicos { get; init; } = new List<OrdemServicoServicoDTO>();
        public ICollection<OrdemServicoInsumoDTO>? Insumos { get; init; } = new List<OrdemServicoInsumoDTO>();
    }
}
