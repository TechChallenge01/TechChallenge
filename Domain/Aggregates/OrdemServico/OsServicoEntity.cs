using Domain.Enums;

namespace Domain.Aggregates.OrdemServico;

public class OsServicoEntity : Base.BaseEntity
{
    public Guid OsId { get; private set; }
    public Guid ServicoId { get; private set; }
    public decimal Valor { get; private set; }
    public EStatusOS Status { get; private set; }
    public DateTime? DataInicioExecucao { get; private set; }
    public DateTime? DataTerminoExecucao { get; private set; }

    public OsServicoEntity(Guid osId, Guid servicoId, decimal valor, Guid idUsuarioCriacao)
    {
        if (valor <= 0) throw new ArgumentException("Valor do serviço deve ser positivo.");

        OsId = osId;
        ServicoId = servicoId;
        Valor = valor;
        IdUsuarioCriacao = idUsuarioCriacao;
        DataCriacao = DateTime.UtcNow;
    }

    protected OsServicoEntity() { }

    public void IniciarExecucao()
    {
        DataInicioExecucao = DateTime.UtcNow;
        Status = EStatusOS.EmExecucao;
    }
    public void ConcluirExecucao()
    {
        if (DataInicioExecucao == null)
            throw new InvalidOperationException("Serviço ainda não foi iniciado.");

        DataTerminoExecucao = DateTime.UtcNow;
        Status = EStatusOS.Finalizada;
    }

    public double? TempoExecucaoMinutos =>
        DataInicioExecucao.HasValue && DataTerminoExecucao.HasValue
            ? (DataTerminoExecucao.Value - DataInicioExecucao.Value).TotalMinutes
            : null;
}
