using Domain.Aggregates.OrdemServicoAggregates;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.test.Tests
{
    public class OrdemServicoTests
    {
        [Fact]
        public void CriarOrdemServico_ComDadosValidos_DeveCriarComSucesso()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var veiculoId = Guid.NewGuid();
            var usuarioId = Guid.NewGuid();

            // Act
            var ordemServico = new OrdemServico(clienteId, veiculoId, usuarioId);

            // Assert
            Assert.NotNull(ordemServico);
            Assert.Equal(clienteId, ordemServico.ClienteId);
            Assert.Equal(veiculoId, ordemServico.VeiculoId);
        }

        [Fact]
        public void CriarOrdemServico_ComClienteIdVazio_DeveThrowArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new OrdemServico(Guid.Empty, Guid.NewGuid(), Guid.NewGuid()));
        }

        [Fact]
        public void CriarOrdemServico_ComVeiculoIdVazio_DeveThrowArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new OrdemServico(Guid.NewGuid(), Guid.Empty, Guid.NewGuid()));
        }

        [Fact]
        public void CriarOrdemServico_ComDadosInsumosVazios_DeveThrowArgumentException()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var veiculoId = Guid.NewGuid();
            var usuarioId = Guid.NewGuid();

            // Act
            var ordemServico = new OrdemServico(clienteId, veiculoId, usuarioId);

            // Assert
            Assert.Throws<ArgumentException>(() => ordemServico.AlterarInsumo(new List<OrdemServicoInsumo>()));
        }

        [Fact]
        public void CriarOrdemServico_ComDadosPecasVazias_DeveThrowArgumentException()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var veiculoId = Guid.NewGuid();
            var usuarioId = Guid.NewGuid();

            // Act
            var ordemServico = new OrdemServico(clienteId, veiculoId, usuarioId);

            // Assert
            Assert.Throws<ArgumentException>(() => ordemServico.AlterarPeca(new List<OrdemServicoPeca>()));
        }

        [Fact]
        public void CriarOrdemServico_ComDadosServicpsVazios_DeveThrowArgumentException()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var veiculoId = Guid.NewGuid();
            var usuarioId = Guid.NewGuid();

            // Act
            var ordemServico = new OrdemServico(clienteId, veiculoId, usuarioId);

            // Assert
            Assert.Throws<ArgumentException>(() => ordemServico.AlterarServico(new List<OrdemServicoServico>()));
        }

        [Fact]
        public void CriarOrdemServico_AlterarStatusParaEmDiagnostico_DeveAlterarComSucesso()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var veiculoId = Guid.NewGuid();
            var usuarioId = Guid.NewGuid();

            // Act
            var ordemServico = new OrdemServico(clienteId, veiculoId, usuarioId);
            ordemServico.IniciarDiagnostico();

            // Assert
            Assert.Equal(ordemServico.StatusOS, EStatusOS.EmDiagnostico.ToString());
        }

        [Fact]
        public void CriarOrdemServico_RealizarDiagnostico_DeveRealizarComSucesso()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var veiculoId = Guid.NewGuid();
            var usuarioId = Guid.NewGuid();

            // Act
            var ordemServico = new OrdemServico(clienteId, veiculoId, usuarioId);
            ordemServico.IniciarDiagnostico();
            ordemServico.RegistrarDiagnostico("ok");

            // Assert
            Assert.Equal(ordemServico.StatusOS, EStatusOS.AguardandoAprovacao.ToString());
        }

        [Fact]
        public void CriarOrdemServico_RealizarDiagnosticoNoStatusInvalido_DeveThrowInvalidOperationException()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var veiculoId = Guid.NewGuid();
            var usuarioId = Guid.NewGuid();

            // Act
            var ordemServico = new OrdemServico(clienteId, veiculoId, usuarioId);

            // Assert
            Assert.Throws<InvalidOperationException>(() => ordemServico.RegistrarDiagnostico("ok"));
        }

        [Fact]
        public void CriarOrdemServico_RealizarDiagnosticoComObservacaoNulla_DeveThrowInvalidOperationException()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var veiculoId = Guid.NewGuid();
            var usuarioId = Guid.NewGuid();

            // Act
            var ordemServico = new OrdemServico(clienteId, veiculoId, usuarioId);
            ordemServico.IniciarDiagnostico();

            // Assert
            Assert.Throws<InvalidOperationException>(() => ordemServico.RegistrarDiagnostico(string.Empty));
        }

        [Fact]
        public void CriarOrdemServico_adicionarPeca_DeveAdicionarComSucesso()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var veiculoId = Guid.NewGuid();
            var usuarioId = Guid.NewGuid();
            var ordemServico = new OrdemServico(clienteId, veiculoId, usuarioId);
            var pecas = new List<OrdemServicoPeca>
            {
                new OrdemServicoPeca(ordemServico.Id, Guid.NewGuid(), 10, 1)
            };
            // Act
            ordemServico.IniciarDiagnostico();
            ordemServico.AlterarPeca(pecas);

            // Assert
            Assert.Equal(ordemServico.Pecas, pecas);
        }

        [Fact]
        public void CriarOrdemServico_adicionarInsumo_DeveAdicionarComSucesso()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var veiculoId = Guid.NewGuid();
            var usuarioId = Guid.NewGuid();
            var ordemServico = new OrdemServico(clienteId, veiculoId, usuarioId);
            var insumos = new List<OrdemServicoInsumo>
            {
                new OrdemServicoInsumo(ordemServico.Id, Guid.NewGuid(), 10, 1)
            };
            // Act
            ordemServico.IniciarDiagnostico();
            ordemServico.AlterarInsumo(insumos);

            // Assert
            Assert.Equal(ordemServico.Insumos, insumos);
        }

        [Fact]
        public void CriarOrdemServico_adicionarServicos_DeveAdicionarComSucesso()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var veiculoId = Guid.NewGuid();
            var usuarioId = Guid.NewGuid();
            var ordemServico = new OrdemServico(clienteId, veiculoId, usuarioId);
            var servicos = new List<OrdemServicoServico>
            {
                new OrdemServicoServico(ordemServico.Id, Guid.NewGuid(), 1, 1)
            };
            // Act
            ordemServico.IniciarDiagnostico();
            ordemServico.AlterarServico(servicos);

            // Assert
            Assert.Equal(ordemServico.Servicos, servicos);
        }

        [Fact]
        public void CriarOrdemServico_adicionardesconto_DeveAdicionarComSucesso()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var veiculoId = Guid.NewGuid();
            var usuarioId = Guid.NewGuid();
            var desconto = 2;

            var ordemServico = new OrdemServico(clienteId, veiculoId, usuarioId);
            var servicos = new List<OrdemServicoServico>
            {
                new OrdemServicoServico(ordemServico.Id, Guid.NewGuid(), 10, 1)
            };

            // Act
            ordemServico.IniciarDiagnostico();
            ordemServico.AlterarServico(servicos);
            var valorTotal = ordemServico.ValorTotal;
            ordemServico.AplicarDesconto(desconto);

            // Assert
            Assert.Equal(ordemServico.ValorTotal, valorTotal - desconto);
        }

        [Fact]
        public void CriarOrdemServico_adicionarDescontoMaiorQueOValorTotal_DeveThrowArgumentException()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var veiculoId = Guid.NewGuid();
            var usuarioId = Guid.NewGuid();
            var desconto = 2;

            var ordemServico = new OrdemServico(clienteId, veiculoId, usuarioId);
            var servicos = new List<OrdemServicoServico>
            {
                new OrdemServicoServico(ordemServico.Id, Guid.NewGuid(), 1, 1)
            };

            // Act
            ordemServico.IniciarDiagnostico();
            ordemServico.AlterarServico(servicos);
            var valorTotal = ordemServico.ValorTotal;

            // Assert
            Assert.Throws<ArgumentException>(() => ordemServico.AplicarDesconto(desconto));
        }

        [Fact]
        public void CriarOrdemServico_adicionarDescontoNegativo_DeveThrowArgumentException()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var veiculoId = Guid.NewGuid();
            var usuarioId = Guid.NewGuid();
            var desconto = -2;

            var ordemServico = new OrdemServico(clienteId, veiculoId, usuarioId);
            var servicos = new List<OrdemServicoServico>
            {
                new OrdemServicoServico(ordemServico.Id, Guid.NewGuid(), 1, 1)
            };

            // Act
            ordemServico.IniciarDiagnostico();
            ordemServico.AlterarServico(servicos);
            var valorTotal = ordemServico.ValorTotal;

            // Assert
            Assert.Throws<ArgumentException>(() => ordemServico.AplicarDesconto(desconto));
        }

        [Fact]
        public void CriarOrdemServico_adicionarServicosComStatusInvalido_DeveThrowInvalidOperationException()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var veiculoId = Guid.NewGuid();
            var usuarioId = Guid.NewGuid();
            var ordemServico = new OrdemServico(clienteId, veiculoId, usuarioId);
            var servicos = new List<OrdemServicoServico>
            {
                new OrdemServicoServico(ordemServico.Id, Guid.NewGuid(), 1, 1)
            };
            // Act
            ordemServico.IniciarDiagnostico();
            ordemServico.RegistrarDiagnostico("ok");


            // Assert
            Assert.Throws<InvalidOperationException>(() => ordemServico.AlterarServico(servicos));
        }

        [Fact]
        public void CriarOrdemServico_Aprovar_DeveRealizarComSucesso()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var veiculoId = Guid.NewGuid();
            var usuarioId = Guid.NewGuid();
            var ordemServico = new OrdemServico(clienteId, veiculoId, usuarioId);

            var servicos = new List<OrdemServicoServico>{
                new OrdemServicoServico(ordemServico.Id, Guid.NewGuid(), 1, 1)
            };

            // Act
            ordemServico.IniciarDiagnostico();
            ordemServico.AlterarServico(servicos);
            ordemServico.RegistrarDiagnostico("ok");
            ordemServico.AprovarOrdemServico();

            // Assert
            Assert.Equal(ordemServico.StatusOS, EStatusOS.EmExecucao.ToString());
            Assert.True(Math.Abs((ordemServico.Servicos.First().DataInicioExecucao!.Value - ordemServico.InicioExecucao!.Value).TotalMinutes) < 1);

        }

        [Fact]
        public void CriarOrdemServico_Cancelar_DeveRealizarComSucesso()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var veiculoId = Guid.NewGuid();
            var usuarioId = Guid.NewGuid();
            var ordemServico = new OrdemServico(clienteId, veiculoId, usuarioId);

            var servicos = new List<OrdemServicoServico>{
                new OrdemServicoServico(ordemServico.Id, Guid.NewGuid(), 1, 1)
            };

            // Act
            ordemServico.IniciarDiagnostico();
            ordemServico.AlterarServico(servicos);
            ordemServico.RegistrarDiagnostico("ok");
            ordemServico.CancelarOrdemServico();

            // Assert
            Assert.Equal(ordemServico.StatusOS, EStatusOS.Cancelada.ToString());
        }

        [Fact]
        public void CriarOrdemServico_FinalizarServico_DeveRealizarComSucesso()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var veiculoId = Guid.NewGuid();
            var usuarioId = Guid.NewGuid();
            var ordemServico = new OrdemServico(clienteId, veiculoId, usuarioId);

            var servicos = new List<OrdemServicoServico>{
                new OrdemServicoServico(ordemServico.Id, Guid.NewGuid(), 1, 1)
            };

            // Act
            ordemServico.IniciarDiagnostico();
            ordemServico.AlterarServico(servicos);
            ordemServico.RegistrarDiagnostico("ok");
            ordemServico.AprovarOrdemServico();
            ordemServico.FinalizarOrdemServico(servicos.Select(s => s.ServicoId).ToList());

            // Assert
            Assert.Equal(ordemServico.StatusOS, EStatusOS.Finalizada.ToString());
            Assert.Equal(ordemServico.TempoExecucao, ordemServico.TerminoExecucao - ordemServico.InicioExecucao);
            Assert.True(Math.Abs((ordemServico.Servicos.First().DataTerminoExecucao!.Value - ordemServico.TerminoExecucao!.Value).TotalMinutes) < 1);
            }

        [Fact]
        public void CriarOrdemServico_EntregarServico_DeveRealizarComSucesso()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var veiculoId = Guid.NewGuid();
            var usuarioId = Guid.NewGuid();
            var ordemServico = new OrdemServico(clienteId, veiculoId, usuarioId);

            var servicos = new List<OrdemServicoServico>{
                new OrdemServicoServico(ordemServico.Id, Guid.NewGuid(), 1, 1)
            };

            // Act
            ordemServico.IniciarDiagnostico();
            ordemServico.AlterarServico(servicos);
            ordemServico.RegistrarDiagnostico("ok");
            ordemServico.AprovarOrdemServico();
            ordemServico.FinalizarOrdemServico(servicos.Select(s => s.ServicoId).ToList());
            ordemServico.Entregar();

            // Assert
            Assert.Equal(ordemServico.StatusOS, EStatusOS.Entregue.ToString());
            Assert.Equal(ordemServico.TempoExecucao, ordemServico.TerminoExecucao - ordemServico.InicioExecucao);
            }


    }
}
