namespace Domain.test.Tests
{
    public class InsumoTests
    {
        [Fact]
        public void CriarInsumo_ComDadosValidos_DeveCriarComSucesso()
        {
            // Arrange
            var nome = "Óleo Lubrificante";
            var descricao = "Óleo sintético";
            var custo = 75.00m;
            var usuarioId = Guid.NewGuid();

            // Act
            var insumo = new Insumo(nome, descricao, custo, usuarioId, DateTime.UtcNow);

            // Assert
            Assert.NotNull(insumo);
            Assert.Equal(nome, insumo.Nome);
            Assert.Equal(descricao, insumo.Descricao);
            Assert.Equal(custo, insumo.CustoUnitario);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void CriarInsumo_ComNomeInvalido_DeveThrowArgumentException(string nome)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new Insumo(nome, "Descrição", 50, Guid.NewGuid(), DateTime.UtcNow));
        }

        [Fact]
        public void AtualizarCusto_ComValorValido_DeveAtualizarComSucesso()
        {
            // Arrange
            var insumo = new Insumo("Óleo", "Óleo sintético", 75.00m, Guid.NewGuid(), DateTime.UtcNow);
            var novoCusto = 80.00m;

            // Act
            insumo.AtualizarCusto(novoCusto);

            // Assert
            Assert.Equal(novoCusto, insumo.CustoUnitario);
        }

        [Fact]
        public void AtualizarCusto_ComValorNegativo_DeveThrowArgumentException()
        {
            // Arrange
            var insumo = new Insumo("Óleo", "Óleo sintético", 75.00m, Guid.NewGuid(), DateTime.UtcNow);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => insumo.AtualizarCusto(-10));
        }
    }
}
