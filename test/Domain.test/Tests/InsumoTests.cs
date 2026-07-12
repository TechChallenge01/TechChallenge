namespace Domain.test.Tests
{
    public class InsumoTests
    {
        private static Insumo CriarInsumoValido() => new Insumo("Óleo Lubrificante", "Óleo sintético 5W30", 75.00m, Guid.NewGuid(), DateTime.UtcNow);

        [Fact]
        public void CriarInsumo_ComDadosValidos_DeveCriarComSucesso()
        {
            var insumo = CriarInsumoValido();

            Assert.NotNull(insumo);
            Assert.Equal("Óleo Lubrificante", insumo.Nome);
            Assert.Equal("Óleo sintético 5W30", insumo.Descricao);
            Assert.Equal(75.00m, insumo.CustoUnitario);
            Assert.NotEqual(Guid.Empty, insumo.Id);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void CriarInsumo_ComNomeInvalido_DeveThrowArgumentException(string nome)
        {
            Assert.Throws<ArgumentException>(() =>
                new Insumo(nome, "Descrição", 50m, Guid.NewGuid(), DateTime.UtcNow));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void CriarInsumo_ComDescricaoInvalida_DeveThrowArgumentException(string descricao)
        {
            Assert.Throws<ArgumentException>(() =>
                new Insumo("Óleo", descricao, 50m, Guid.NewGuid(), DateTime.UtcNow));
        }

        [Fact]
        public void CriarInsumo_ComCustoNegativo_DeveThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                new Insumo("Óleo", "Descrição", -1m, Guid.NewGuid(), DateTime.UtcNow));
        }

        [Fact]
        public void CriarInsumo_ComCustoZero_DeveCriarComSucesso()
        {
            var insumo = new Insumo("Óleo", "Descrição", 0m, Guid.NewGuid(), DateTime.UtcNow);
            Assert.Equal(0m, insumo.CustoUnitario);
        }

        [Fact]
        public void AtualizarNome_ComNomeValido_DeveAtualizarComSucesso()
        {
            var insumo = CriarInsumoValido();
            insumo.AtualizarNome("Líquido de Freio");
            Assert.Equal("Líquido de Freio", insumo.Nome);
        }

        [Fact]
        public void AtualizarNome_ComNomeInvalido_DeveThrowArgumentException()
        {
            var insumo = CriarInsumoValido();
            Assert.Throws<ArgumentException>(() => insumo.AtualizarNome(""));
        }

        [Fact]
        public void AtualizarDescricao_ComDescricaoValida_DeveAtualizarComSucesso()
        {
            var insumo = CriarInsumoValido();
            insumo.AtualizarDescricao("Nova descrição do insumo");
            Assert.Equal("Nova descrição do insumo", insumo.Descricao);
        }

        [Fact]
        public void AtualizarDescricao_ComDescricaoInvalida_DeveThrowArgumentException()
        {
            var insumo = CriarInsumoValido();
            Assert.Throws<ArgumentException>(() => insumo.AtualizarDescricao("   "));
        }

        [Fact]
        public void AtualizarCusto_ComValorValido_DeveAtualizarComSucesso()
        {
            var insumo = CriarInsumoValido();
            insumo.AtualizarCusto(80.00m);
            Assert.Equal(80.00m, insumo.CustoUnitario);
        }

        [Fact]
        public void AtualizarCusto_ComValorNegativo_DeveThrowArgumentException()
        {
            var insumo = CriarInsumoValido();
            Assert.Throws<ArgumentException>(() => insumo.AtualizarCusto(-10m));
        }

        [Fact]
        public void Inativar_DeveDefinirAtivoFalso()
        {
            var insumo = CriarInsumoValido();
            insumo.Inativar();
            Assert.False(insumo.Ativo);
        }

        [Fact]
        public void RastrearAlteracao_DeveRegistrarUsuarioEData()
        {
            var insumo = CriarInsumoValido();
            var idUsuario = Guid.NewGuid();
            var data = DateTime.UtcNow;

            insumo.RastrearAlteracao(idUsuario, data);

            Assert.Equal(idUsuario, insumo.IdUsuarioAtualizacao);
            Assert.Equal(data, insumo.DataAtualizacao);
        }
    }
}
