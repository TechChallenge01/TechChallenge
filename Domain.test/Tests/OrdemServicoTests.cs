using Domain.Aggregates.OrdemServicoAggregates;
using Xunit;

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
    }
}
