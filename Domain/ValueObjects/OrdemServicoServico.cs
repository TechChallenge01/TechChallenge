using Domain.Aggregates.OrdemServicoAggregates;
using Domain.Entities;
using Domain.Enums;

namespace Domain.ValueObjects;

public class OrdemServicoServico
{
    public Guid OrdemServicoId { get; private set; }
    public Guid ServicoId { get; private set; }
    public decimal ValorUnitario { get; private set; }
    public string Status { get; private set; }
    public DateTime? DataInicioExecucao { get; private set; }
    public DateTime? DataTerminoExecucao { get; private set; }
    public int Quantidade { get; private set; }
    public decimal ValorTotal => ValorUnitario * Quantidade;
    public virtual Servico Servico { get; private set; }
    public virtual OrdemServico OrdemServico { get; private set; }

    public OrdemServicoServico(Guid osId, Guid servicoId, int quantidade, decimal valorUnitario)
    {
        if (valorUnitario <= 0) throw new ArgumentException("Valor do serviço deve ser positivo.");
        if (quantidade <= 0) throw new ArgumentException("Quantidade do serviço deve ser positiva.");

        OrdemServicoId = osId;
        ServicoId = servicoId;
        ValorUnitario = valorUnitario;
        Quantidade = quantidade;
        Status = EStatusOS.AguardandoAprovacao.ToString();
    }

    protected OrdemServicoServico() { }

    public void IniciarExecucao()
    {
        DataInicioExecucao = DateTime.UtcNow;
        Status = EStatusOS.EmExecucao.ToString();
    }
    public void ConcluirExecucao()
    {
        if (DataInicioExecucao == null)
            throw new InvalidOperationException("Serviço ainda não foi iniciado.");

        DataTerminoExecucao = DateTime.UtcNow;
        Status = EStatusOS.Finalizada.ToString();
    }
}
