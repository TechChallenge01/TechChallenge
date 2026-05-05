using Domain.Entities;
using Domain.ValueObjects;

namespace Domain.test.Tests
{
    public class VeiculoTests
    {
        [Fact]
        public void CriarVeiculo_ComDadosValidos_DeveCriarComSucesso()
        {
            // Arrange
            var modelo = "Civic";
            var marca = "Honda";
            var clienteId = Guid.NewGuid();
            var ano = 2020;
            var placa = new Placa("ABC1234");
            var cor = "Preto";
            var usuarioId = Guid.NewGuid();

            // Act
            var veiculo = new Veiculo(modelo, marca, clienteId, ano, placa, cor, usuarioId);

            // Assert
            Assert.NotNull(veiculo);
            Assert.Equal(modelo, veiculo.Modelo);
            Assert.Equal(marca, veiculo.MarcaVeiculo);
            Assert.Equal(ano, veiculo.Ano);
        }

        [Theory]
        [InlineData(1800)]
        [InlineData(2030)]
        public void CriarVeiculo_ComAnoInvalido_DeveThrowArgumentException(int ano)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new Veiculo("Civic", "Honda", Guid.NewGuid(), ano, new Placa("ABC1234"), "Preto", Guid.NewGuid()));
        }
    }
}
