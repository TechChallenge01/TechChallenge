using Domain.Entities;

namespace Domain.test.Tests
{
    public class PecaTests
    {
        [Fact]
        public void CriarPeca_ComDadosValidos_DeveCriarComSucesso()
        {
            // Arrange
            var nome = "Filtro de Ar";
            var descricao = "Filtro premium";
            var marca = "Bosch";
            var preco = 85.50m;
            var usuarioId = Guid.NewGuid();
            var data = DateTime.UtcNow;

            // Act
            var peca = new Peca(nome, descricao, marca, preco, usuarioId, data);

            // Assert
            Assert.NotNull(peca);
            Assert.Equal(nome, peca.Nome);
            Assert.Equal(descricao, peca.Descricao);
            Assert.Equal(marca, peca.MarcaPeca);
            Assert.Equal(preco, peca.ValorUnitario);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void CriarPeca_ComNomeInvalido_DeveThrowArgumentException(string nome)
        {
            // Arrange
            var descricao = "Filtro premium";
            var marca = "Bosch";
            var preco = 85.50m;
            var usuarioId = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new Peca(nome, descricao, marca, preco, usuarioId, DateTime.UtcNow));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public void CriarPeca_ComPrecoInvalido_DeveThrowArgumentException(decimal preco)
        {
            // Arrange
            var nome = "Filtro de Ar";
            var descricao = "Filtro premium";
            var marca = "Bosch";
            var usuarioId = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new Peca(nome, descricao, marca, preco, usuarioId, DateTime.UtcNow));
        }
    }
}
