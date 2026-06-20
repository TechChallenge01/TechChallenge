namespace Infra.DbModel
{
    public class OrdemServicoDbModel
    {
        public OrdemServicoDbModel(Guid id, Guid clienteId, Guid veiculoId, string statusOS, string? observacao, decimal valorTotal, decimal valorDesconto, DateTime? inicioExecucao, DateTime? terminoExecucao, Guid idUsuarioCriacao, DateTime dataCriacao, Guid? idUsuarioAtualizacao, DateTime? dataAtualizacao)
        {
            Id = id;
            ClienteId = clienteId;
            VeiculoId = veiculoId;
            StatusOS = statusOS;
            Observacao = observacao;
            ValorTotal = valorTotal;
            ValorDesconto = valorDesconto;
            InicioExecucao = inicioExecucao;
            TerminoExecucao = terminoExecucao;
            IdUsuarioCriacao = idUsuarioCriacao;
            DataCriacao = dataCriacao;
            IdUsuarioAtualizacao = idUsuarioAtualizacao;
            DataAtualizacao = dataAtualizacao;
        }

        public Guid Id { get; set; }
        public Guid ClienteId { get; set; }
        public Guid VeiculoId { get; set; }
        public string StatusOS { get; set; }
        public string? Observacao { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal ValorDesconto { get; set; }
        public DateTime? InicioExecucao { get; set; }
        public DateTime? TerminoExecucao { get; set; }
        public Guid IdUsuarioCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public Guid? IdUsuarioAtualizacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public bool Ativo { get; set; } = true;
        public ICollection<OrdemServicoServicoDbModel> Servicos { get; set; } = new List<OrdemServicoServicoDbModel>();
        public ICollection<OrdemServicoPecaDbModel> Pecas { get; set; } = new List<OrdemServicoPecaDbModel>();
        public ICollection<OrdemServicoInsumoDbModel> Insumos { get; set; } = new List<OrdemServicoInsumoDbModel>();
        public virtual ClienteDbModel Cliente { get; set; }
        public virtual VeiculoDbModel Veiculo { get; set; }
    }
}
