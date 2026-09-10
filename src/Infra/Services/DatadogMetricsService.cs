using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using StatsdClient;

namespace Infra.Services
{
    public class DatadogMetricsService : IMetricsService, IDisposable
    {
        private readonly DogStatsdService _dogStatsd;

        public DatadogMetricsService(IConfiguration config)
        {
            _dogStatsd = new DogStatsdService();
            _dogStatsd.Configure(new StatsdConfig
            {
                StatsdServerName = config["Datadog:AgentHost"] ?? "127.0.0.1",
                StatsdPort = int.TryParse(config["Datadog:DogStatsdPort"], out var port) ? port : 8125,
                Prefix = "techchallenger.os"
            });
        }

        public void IncrementOrdemServicoCriada()
            => _dogStatsd.Increment("criadas");

        public void IncrementOrdemServicoStatus(string status)
            => _dogStatsd.Increment("status_alterado", tags: new[] { $"status:{status}" });

        public void RecordTempoExecucao(string status, TimeSpan duracao)
            => _dogStatsd.Histogram("tempo_execucao_segundos", duracao.TotalSeconds, tags: new[] { $"status:{status}" });

        public void IncrementErro(string operacao)
            => _dogStatsd.Increment("erros", tags: new[] { $"operacao:{operacao}" });

        public void Dispose() => _dogStatsd.Dispose();
    }
}
