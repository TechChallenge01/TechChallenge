using Domain.Aggregates.OrdemServicoAggregates;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.test.Aggregates;

public class OrdemServicoTests
{
    private Guid _clienteId = Guid.NewGuid();
    private Guid _veiculoId = Guid.NewGuid();
    private Guid _usuarioId = Guid.NewGuid();

    [Fact]
    public void Constructor_ValidOrdemServico_CreatesOrdemServicoSuccessfully()
    {
        // Act
        var os = new OrdemServico(_clienteId, _veiculoId, _usuarioId);

        // Assert
        Assert.NotNull(os);
        Assert.Equal(_clienteId, os.ClienteId);
        Assert.Equal(_veiculoId, os.VeiculoId);
        Assert.Equal(EStatusOS.Recebida.ToString(), os.StatusOS);
        Assert.Equal(0, os.ValorTotal);
        Assert.Equal(0, os.ValorDesconto);
        Assert.True(os.Ativo);
    }

    [Fact]
    public void Constructor_EmptyClienteId_ThrowsArgumentException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => 
            new OrdemServico(Guid.Empty, _veiculoId, _usuarioId));
        Assert.Contains("cliente é obrigatório", ex.Message);
    }

    [Fact]
    public void Constructor_EmptyVeiculoId_ThrowsArgumentException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => 
            new OrdemServico(_clienteId, Guid.Empty, _usuarioId));
        Assert.Contains("veículo é obrigatório", ex.Message);
    }

    [Fact]
    public void IniciarDiagnostico_FromRecebida_ChangesStatusToDiagnostico()
    {
        // Arrange
        var os = new OrdemServico(_clienteId, _veiculoId, _usuarioId);

        // Act
        os.IniciarDiagnostico();

        // Assert
        Assert.Equal(EStatusOS.EmDiagnostico.ToString(), os.StatusOS);
    }

    [Fact]
    public void IniciarDiagnostico_FromInvalidStatus_ThrowsException()
    {
        // Arrange
        var os = new OrdemServico(_clienteId, _veiculoId, _usuarioId);
        os.IniciarDiagnostico();
        os.RegistrarDiagnostico("Diagnóstico feito");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => os.IniciarDiagnostico());
    }

    [Fact]
    public void RegistrarDiagnostico_ValidObservacao_ChangeStatusToAguardandoAprovacao()
    {
        // Arrange
        var os = new OrdemServico(_clienteId, _veiculoId, _usuarioId);
        os.IniciarDiagnostico();
        string observacao = "Motor com problema na corrente";

        // Act
        os.RegistrarDiagnostico(observacao);

        // Assert
        Assert.Equal(EStatusOS.AguardandoAprovacao.ToString(), os.StatusOS);
        Assert.Equal(observacao, os.Observacao);
    }

    [Fact]
    public void RegistrarDiagnostico_EmptyObservacao_ThrowsException()
    {
        // Arrange
        var os = new OrdemServico(_clienteId, _veiculoId, _usuarioId);
        os.IniciarDiagnostico();

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => 
            os.RegistrarDiagnostico(""));
        Assert.Contains("obrigatória", ex.Message);
    }

    [Fact]
    public void AprovarOrdemServico_FromAguardandoAprovacao_ChangesStatusToEmExecucao()
    {
        // Arrange
        var os = new OrdemServico(_clienteId, _veiculoId, _usuarioId);
        os.IniciarDiagnostico();
        os.RegistrarDiagnostico("Diagnóstico feito");

        // Act
        os.AprovarOrdemServico();

        // Assert
        Assert.Equal(EStatusOS.EmExecucao.ToString(), os.StatusOS);
    }

    [Fact]
    public void AprovarOrdemServico_SetsInicioExecucao()
    {
        // Arrange
        var os = new OrdemServico(_clienteId, _veiculoId, _usuarioId);
        os.IniciarDiagnostico();
        os.RegistrarDiagnostico("Diagnóstico feito");
        var beforeApprove = DateTime.UtcNow;

        // Act
        os.AprovarOrdemServico();
        var afterApprove = DateTime.UtcNow;

        // Assert
        Assert.True(beforeApprove <= os.InicioExecucao && os.InicioExecucao <= afterApprove);
    }

    [Fact]
    public void CancelarOrdemServico_FromAguardandoAprovacao_ChangesStatusToCancelada()
    {
        // Arrange
        var os = new OrdemServico(_clienteId, _veiculoId, _usuarioId);
        os.IniciarDiagnostico();
        os.RegistrarDiagnostico("Diagnóstico feito");

        // Act
        os.CancelarOrdemServico();

        // Assert
        Assert.Equal(EStatusOS.Cancelada.ToString(), os.StatusOS);
    }

    [Fact]
    public void AplicarDesconto_ValidDesconto_UpdatesValorDesconto()
    {
        // Arrange
        var os = new OrdemServico(_clienteId, _veiculoId, _usuarioId);
        var servico = new List<Servico>
        {
            new Servico("Troca de oleo", "Troca de oleo", 200, Guid.Empty, DateTime.UtcNow),
            new Servico("Troca de Pneu", "Troca de Pneu", 50, Guid.Empty, DateTime.UtcNow),

        };
        var ordemServicoServico = new List<OrdemServicoServico>
        {
            new OrdemServicoServico(servico[0].Id, 1, servico[0].ValorUnitario),
            new OrdemServicoServico(servico[1].Id, 4, servico[1].ValorUnitario, Guid.Empty),
        };
        
        os.AlterarServico(ordemServicoServico);

        // Act
        os.AplicarDesconto(100);

        // Assert
        Assert.Equal(100, os.ValorDesconto);
    }

    [Fact]
    public void AplicarDesconto_NegativeDesconto_ThrowsArgumentException()
    {
        // Arrange
        var os = new OrdemServico(_clienteId, _veiculoId, _usuarioId);

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => os.AplicarDesconto(-100));
        Assert.Contains("negativo", ex.Message);
    }

    [Fact]
    public void AplicarDesconto_DescontoGreaterThanTotal_ThrowsArgumentException()
    {
        // Arrange
        var os = new OrdemServico(_clienteId, _veiculoId, _usuarioId);

        // Act & Assert
        // Como ValorTotal começa em 0, qualquer desconto > 0 deve lançar exceção
        var ex = Assert.Throws<ArgumentException>(() => os.AplicarDesconto(100));
        Assert.Contains("não pode ser maior que o valor total", ex.Message);
    }

    [Fact]
    public void TempoExecucao_BeforeTermino_ReturnsZero()
    {
        // Arrange
        var os = new OrdemServico(_clienteId, _veiculoId, _usuarioId);

        // Act
        var tempoExecucao = os.TempoExecucao;

        // Assert
        Assert.Equal(TimeSpan.Zero, tempoExecucao);
    }

    [Fact]
    public void Inativar_OrdemServicoBecomesInativo()
    {
        // Arrange
        var os = new OrdemServico(_clienteId, _veiculoId, _usuarioId);

        // Act
        os.Inativar();

        // Assert
        Assert.False(os.Ativo);
    }
}
