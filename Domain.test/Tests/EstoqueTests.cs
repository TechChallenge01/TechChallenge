using Domain.Aggregates.EstoqueAggregates;
using Xunit;

namespace Domain.test.Tests
{
    public class EstoqueTests
    {
        [Fact]
        public void CriarEstoque_ComPecaValida_DeveCriarComSucesso()
        {
            // Arrange
            var pecaId = Guid.NewGuid();
            var quantidade = 10;
            var usuarioId = Guid.NewGuid();

            // Act
            var estoque = new Estoque(null, pecaId, quantidade, usuarioId, DateTime.UtcNow);

            // Assert
            Assert.NotNull(estoque);
            Assert.Equal(pecaId, estoque.PecaId);
            Assert.Equal(quantidade, estoque.QuantidadeDisponivel);
        }

        [Fact]
        public void CriarEstoque_ComInsumoValido_DeveCriarComSucesso()
        {
            // Arrange
            var insumoId = Guid.NewGuid();
            var quantidade = 20;
            var usuarioId = Guid.NewGuid();

            // Act
            var estoque = new Estoque(insumoId, null, quantidade, usuarioId, DateTime.UtcNow);

            // Assert
            Assert.NotNull(estoque);
            Assert.Equal(insumoId, estoque.InsumoId);
            Assert.Equal(quantidade, estoque.QuantidadeDisponivel);
        }

        [Fact]
        public void CriarEstoque_SemPecaEInsumo_DeveThrowArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new Estoque(null, null, 10, Guid.NewGuid(), DateTime.UtcNow));
        }

        [Fact]
        public void CriarEstoque_ComPecaEInsumo_DeveThrowArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new Estoque(Guid.NewGuid(), Guid.NewGuid(), 10, Guid.NewGuid(), DateTime.UtcNow));
        }

        [Fact]
        public void AdicionarEstoque_ComQuantidadeValida_DeveAdicionarComSucesso()
        {
            // Arrange
            var estoque = new Estoque(null, Guid.NewGuid(), 10, Guid.NewGuid(), DateTime.UtcNow);
            var quantidadeAdicional = 5;

            // Act
            estoque.AdicionarEstoque(quantidadeAdicional, Guid.NewGuid());

            // Assert
            Assert.Equal(15, estoque.QuantidadeDisponivel);
        }

        [Fact]
        public void RetirarEstoque_ComQuantidadeValida_DeveRetirarComSucesso()
        {
            // Arrange
            var estoque = new Estoque(null, Guid.NewGuid(), 10, Guid.NewGuid(), DateTime.UtcNow);

            // Act
            estoque.RetirarEstoque(3, Guid.NewGuid());

            // Assert
            Assert.Equal(7, estoque.QuantidadeDisponivel);
        }

        [Fact]
        public void RetirarEstoque_ComQuantidadeInsuficiente_DeveThrowInvalidOperationException()
        {
            // Arrange
            var estoque = new Estoque(null, Guid.NewGuid(), 5, Guid.NewGuid(), DateTime.UtcNow);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => estoque.RetirarEstoque(10, Guid.NewGuid()));
        }
    }
}
