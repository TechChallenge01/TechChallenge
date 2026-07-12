using Domain.Entities;

namespace Domain.test.Tests
{
    public class PecaTests
    {
        private static Peca CriarPecaValida() => new Peca("Filtro de Ar", "Filtro premium", "Bosch", 85.50m, Guid.NewGuid(), DateTime.UtcNow);

        [Fact]
        public void CriarPeca_ComDadosValidos_DeveCriarComSucesso()
        {
            var peca = CriarPecaValida();

            Assert.NotNull(peca);
            Assert.Equal("Filtro de Ar", peca.Nome);
            Assert.Equal("Filtro premium", peca.Descricao);
            Assert.Equal("Bosch", peca.MarcaPeca);
            Assert.Equal(85.50m, peca.ValorUnitario);
            Assert.NotEqual(Guid.Empty, peca.Id);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void CriarPeca_ComNomeInvalido_DeveThrowArgumentException(string nome)
        {
            Assert.Throws<ArgumentException>(() =>
                new Peca(nome, "Filtro premium", "Bosch", 85.50m, Guid.NewGuid(), DateTime.UtcNow));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public void CriarPeca_ComPrecoInvalido_DeveThrowArgumentException(decimal preco)
        {
            Assert.Throws<ArgumentException>(() =>
                new Peca("Filtro de Ar", "Filtro premium", "Bosch", preco, Guid.NewGuid(), DateTime.UtcNow));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void CriarPeca_ComDescricaoInvalida_DeveThrowArgumentException(string descricao)
        {
            Assert.Throws<ArgumentException>(() =>
                new Peca("Filtro de Ar", descricao, "Bosch", 85.50m, Guid.NewGuid(), DateTime.UtcNow));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void CriarPeca_ComMarcaInvalida_DeveThrowArgumentException(string marca)
        {
            Assert.Throws<ArgumentException>(() =>
                new Peca("Filtro de Ar", "Filtro premium", marca, 85.50m, Guid.NewGuid(), DateTime.UtcNow));
        }

        [Fact]
        public void AlterarNome_ComNomeValido_DeveAlterarComSucesso()
        {
            var peca = CriarPecaValida();
            peca.AlterarNome("Filtro de Óleo");
            Assert.Equal("Filtro de Óleo", peca.Nome);
        }

        [Fact]
        public void AlterarNome_ComNomeInvalido_DeveThrowArgumentException()
        {
            var peca = CriarPecaValida();
            Assert.Throws<ArgumentException>(() => peca.AlterarNome(""));
        }

        [Fact]
        public void AlterarDescricao_ComDescricaoValida_DeveAlterarComSucesso()
        {
            var peca = CriarPecaValida();
            peca.AlterarDescricao("Nova descrição da peça");
            Assert.Equal("Nova descrição da peça", peca.Descricao);
        }

        [Fact]
        public void AlterarDescricao_ComDescricaoInvalida_DeveThrowArgumentException()
        {
            var peca = CriarPecaValida();
            Assert.Throws<ArgumentException>(() => peca.AlterarDescricao("   "));
        }

        [Fact]
        public void AlterarMarca_ComMarcaValida_DeveAlterarComSucesso()
        {
            var peca = CriarPecaValida();
            peca.AlterarMarcaPeca("Mann");
            Assert.Equal("Mann", peca.MarcaPeca);
        }

        [Fact]
        public void AlterarMarca_ComMarcaInvalida_DeveThrowArgumentException()
        {
            var peca = CriarPecaValida();
            Assert.Throws<ArgumentException>(() => peca.AlterarMarcaPeca(""));
        }

        [Fact]
        public void AlterarPreco_ComPrecoValido_DeveAlterarComSucesso()
        {
            var peca = CriarPecaValida();
            peca.AlterarPrecoVenda(150.00m);
            Assert.Equal(150.00m, peca.ValorUnitario);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public void AlterarPreco_ComPrecoInvalido_DeveThrowArgumentException(decimal preco)
        {
            var peca = CriarPecaValida();
            Assert.Throws<ArgumentException>(() => peca.AlterarPrecoVenda(preco));
        }
    }
}
