namespace Application.Interfaces;
public interface IMetricsService
{
    void IncrementOrdemServicoCriada();
    void IncrementOrdemServicoStatus(string status);
    void RecordTempoExecucao(string status, TimeSpan duracao);
    void IncrementErro(string operacao);
}
