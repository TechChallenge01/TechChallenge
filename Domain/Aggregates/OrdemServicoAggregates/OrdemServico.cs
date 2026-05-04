using Domain.Aggregates.ClienteAggregates;
using Domain.BaseEntity;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Aggregates.OrdemServicoAggregates;
public class OrdemServico : Base
{
    public Guid Id { get; private set; }
    public Guid ClienteId { get; private set; }
    public Guid VeiculoId { get; private set; }
    public string StatusOS { get; private set; }
    public string? Observacao { get; private set; }
    public decimal ValorTotal { get; private set; }
    public decimal ValorDesconto { get; private set; } = 0;
    public DateTime? InicioExecucao { get; private set; }
    public DateTime? TerminoExecucao { get; private set; }

    public ICollection<OrdemServicoServico> Servicos { get; private set; } = new List<OrdemServicoServico>();
    public ICollection<OrdemServicoPeca> Pecas { get; private set; } = new List<OrdemServicoPeca>();
    public ICollection<OrdemServicoInsumo> Insumos { get; private set; } = new List<OrdemServicoInsumo>();

    public virtual Cliente Cliente { get; private set; }
    public virtual Veiculo Veiculo { get; private set; }

    public TimeSpan TempoExecucao => TerminoExecucao.HasValue && InicioExecucao.HasValue ? TerminoExecucao.Value - InicioExecucao.Value : TimeSpan.Zero;

    protected OrdemServico() {}

    public OrdemServico(Guid clienteId, Guid veiculoId,Guid idUsuarioCriacao) : base(idUsuarioCriacao, DateTime.UtcNow, null, null)
    {
        if(clienteId == Guid.Empty) throw new ArgumentException("O cliente é obrigatório.", nameof(clienteId));
        if(veiculoId == Guid.Empty) throw new ArgumentException("O veículo é obrigatório.", nameof(veiculoId));

        Id = Guid.NewGuid();
        ClienteId = clienteId;
        VeiculoId = veiculoId;
        StatusOS = EStatusOS.Recebida.ToString();
        ValorDesconto = 0;
        ValorTotal = 0;
        IdUsuarioCriacao = idUsuarioCriacao;
        DataCriacao = DataCriacao;
    }

    public void IniciarDiagnostico()
    {
        ValidarTransicao(EStatusOS.Recebida, EStatusOS.EmDiagnostico);
        StatusOS = EStatusOS.EmDiagnostico.ToString();
    }

    public void RegistrarDiagnostico(string observacao) 
    {
        ValidarTransicao(EStatusOS.EmDiagnostico, EStatusOS.AguardandoAprovacao);

        if(string.IsNullOrWhiteSpace(observacao)) 
            throw new InvalidOperationException("Observação é obrigatória para registrar um diagnóstico.");

        Observacao = observacao;
        StatusOS = EStatusOS.AguardandoAprovacao.ToString()     ;
    }

    public void Entregar()
    {
        ValidarTransicao(EStatusOS.Finalizada, EStatusOS.Entregue);

        StatusOS = EStatusOS.Entregue.ToString();
    }

    public void AprovarOrdemServico()
    {
        ValidarTransicao(EStatusOS.AguardandoAprovacao, EStatusOS.EmExecucao);

        StatusOS = EStatusOS.EmExecucao.ToString();
        InicioExecucao = DateTime.UtcNow;

        Servicos.ToList().ForEach(s => s.IniciarExecucao());
    }

    public void CancelarOrdemServico()
    {
        ValidarTransicao(EStatusOS.AguardandoAprovacao, EStatusOS.Cancelada);

        StatusOS = EStatusOS.Cancelada.ToString();
    }

    public void FinalizarOrdemServico(ICollection<Guid> servicosId)
    {
        ValidarTransicao(EStatusOS.EmExecucao, EStatusOS.Finalizada);

        var servicos = Servicos.Where(s => servicosId.Contains(s.ServicoId)).ToList();

        servicos.ForEach(s => s.ConcluirExecucao());

        if (!Servicos.Any(s => s.Status == EStatusOS.EmExecucao.ToString()))
        {
            StatusOS = EStatusOS.Finalizada.ToString();
            TerminoExecucao = DateTime.UtcNow;
        }
    }

    private void RecalcularValorTotal()
    {
        var totalServicos = Servicos.Sum(s => s.ValorUnitario);
        var totalPecas = Pecas.Sum(p => p.ValorUnitario * p.Quantidade);
        var totalInsumos = Insumos.Sum(i => i.CustoUnitario * i.Quantidade);
        ValorTotal = totalServicos + totalPecas + totalInsumos - ValorDesconto;
    }

    public void AlterarServico(List<OrdemServicoServico> osServico)
    {
        if (osServico == null)
            throw new ArgumentNullException(nameof(osServico));

        ValidarStatusParaEdicao();

        Servicos = osServico.DistinctBy(os => os.ServicoId).ToList();

        RecalcularValorTotal();
    }

    public void AlterarPeca(List<OrdemServicoPeca> osPeca)
    {
        if (osPeca == null)
            throw new ArgumentNullException(nameof(osPeca));

        ValidarStatusParaEdicao();

        Pecas = osPeca.DistinctBy(op => op.PecaId).ToList();

        RecalcularValorTotal();
    }

    public void AlterarInsumo(List<OrdemServicoInsumo> osInsumo)
    {
        if (osInsumo == null)
            throw new ArgumentNullException(nameof(osInsumo));

        ValidarStatusParaEdicao();

        Insumos = osInsumo.DistinctBy(oi => oi.InsumoId).ToList();

        RecalcularValorTotal();
    }

    public void AplicarDesconto(decimal valorDesconto)
    {
        if (valorDesconto < 0)
            throw new ArgumentException("Valor de desconto não pode ser negativo.");

        if(valorDesconto > ValorTotal)
            throw new ArgumentException("Valor de desconto não pode ser maior que o valor total da OS.");

        ValorDesconto = valorDesconto;
        RecalcularValorTotal();
    }

    public void RegistrarEntrega()
    {
        ValidarTransicao(EStatusOS.Finalizada, EStatusOS.Entregue);
        
        StatusOS = EStatusOS.Entregue.ToString();
    }

    private void ValidarStatusParaEdicao()
    {
        var statusEditaveis = new[]
        {
            EStatusOS.Recebida.ToString(),
            EStatusOS.EmDiagnostico.ToString()
        };

        if (!statusEditaveis.Contains(StatusOS))
            throw new InvalidOperationException(
                $"Não é possível editar itens de uma OS com status '{StatusOS}'.");
    }

    private void ValidarTransicao(EStatusOS statusEsperado, EStatusOS proximoStatus)
    {
        if (StatusOS != statusEsperado.ToString())
            throw new InvalidOperationException(
                $"Transição inválida: OS está '{StatusOS}', esperado '{statusEsperado.ToString()}' para ir para '{proximoStatus.ToString()}'.");
    }
}
