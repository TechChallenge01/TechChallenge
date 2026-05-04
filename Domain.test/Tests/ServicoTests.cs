using Domain.Entities;

namespace Domain.test.Tests
{
    public class ServicoTests
    {
        [Fact]
        public void CriarServico_ComDadosValidos_DeveCriarComSucesso()
        {
            // Arrange
            var nome = "Troca de Óleo";
            var descricao = "Troca de óleo e filtro do motor";
            var preco = 120.00m;
            var usuarioId = Guid.NewGuid();
            var data = DateTime.UtcNow;

            // Act
            var servico = new Servico(nome, descricao, preco, usuarioId, data);

            // Assert
            Assert.NotNull(servico);
            Assert.Equal(nome, servico.Nome);
            Assert.Equal(descricao, servico.Descricao);
            Assert.Equal(preco, servico.ValorUnitario);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void CriarServico_ComNomeInvalido_DeveThrowArgumentException(string nome)
        {
            // Arrange
            var descricao = "Descrição válida";
            var preco = 120.00m;
            var usuarioId = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new Servico(nome, descricao, preco, usuarioId, DateTime.UtcNow));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public void CriarServico_ComPrecoInvalido_DeveThrowArgumentException(decimal preco)
        {
            // Arrange
            var nome = "Troca de Óleo";
            var descricao = "Troca de óleo";
            var usuarioId = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new Servico(nome, descricao, preco, usuarioId, DateTime.UtcNow));
        }
    }
}
