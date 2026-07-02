using Domain.Entities;

namespace Domain.test.Tests
{
    public class ServicoTests
    {
        private static Servico CriarServicoValido() => new Servico("Troca de Óleo", "Troca de óleo e filtro do motor", 120.00m, Guid.NewGuid(), DateTime.UtcNow);

        [Fact]
        public void CriarServico_ComDadosValidos_DeveCriarComSucesso()
        {
            var servico = CriarServicoValido();

            Assert.NotNull(servico);
            Assert.Equal("Troca de Óleo", servico.Nome);
            Assert.Equal("Troca de óleo e filtro do motor", servico.Descricao);
            Assert.Equal(120.00m, servico.ValorUnitario);
            Assert.NotEqual(Guid.Empty, servico.Id);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void CriarServico_ComNomeInvalido_DeveThrowArgumentException(string nome)
        {
            Assert.Throws<ArgumentException>(() =>
                new Servico(nome, "Descrição válida", 120.00m, Guid.NewGuid(), DateTime.UtcNow));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public void CriarServico_ComPrecoInvalido_DeveThrowArgumentException(decimal preco)
        {
            Assert.Throws<ArgumentException>(() =>
                new Servico("Troca de Óleo", "Descrição", preco, Guid.NewGuid(), DateTime.UtcNow));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void CriarServico_ComDescricaoInvalida_DeveThrowArgumentException(string descricao)
        {
            Assert.Throws<ArgumentException>(() =>
                new Servico("Troca de Óleo", descricao, 120.00m, Guid.NewGuid(), DateTime.UtcNow));
        }

        [Fact]
        public void AlterarNome_ComNomeValido_DeveAlterarComSucesso()
        {
            var servico = CriarServicoValido();
            servico.AlterarNome("Alinhamento");
            Assert.Equal("Alinhamento", servico.Nome);
        }

        [Fact]
        public void AlterarNome_ComNomeInvalido_DeveThrowArgumentException()
        {
            var servico = CriarServicoValido();
            Assert.Throws<ArgumentException>(() => servico.AlterarNome(""));
        }

        [Fact]
        public void AlterarDescricao_ComDescricaoValida_DeveAlterarComSucesso()
        {
            var servico = CriarServicoValido();
            servico.AlterarDescricao("Nova descrição do serviço");
            Assert.Equal("Nova descrição do serviço", servico.Descricao);
        }

        [Fact]
        public void AlterarDescricao_ComDescricaoInvalida_DeveThrowArgumentException()
        {
            var servico = CriarServicoValido();
            Assert.Throws<ArgumentException>(() => servico.AlterarDescricao("   "));
        }

        [Fact]
        public void AlterarPrecoVenda_ComPrecoValido_DeveAlterarComSucesso()
        {
            var servico = CriarServicoValido();
            servico.AlterarPrecoVenda(200.00m);
            Assert.Equal(200.00m, servico.ValorUnitario);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-50)]
        public void AlterarPrecoVenda_ComPrecoInvalido_DeveThrowArgumentException(decimal preco)
        {
            var servico = CriarServicoValido();
            Assert.Throws<ArgumentException>(() => servico.AlterarPrecoVenda(preco));
        }

        [Fact]
        public void AtualizarTempoMedio_ComTemposValidos_DeveCalcularMedia()
        {
            var servico = CriarServicoValido();
            var tempos = new List<TimeSpan>
            {
                TimeSpan.FromMinutes(30),
                TimeSpan.FromMinutes(60),
                TimeSpan.FromMinutes(90)
            };

            servico.AtualizarTempoMedio(tempos);

            Assert.Equal(TimeSpan.FromMinutes(60), servico.TempoMedioExecucao);
        }

        [Fact]
        public void AtualizarTempoMedio_ComListaVazia_NaoDeveAlterarTempoMedio()
        {
            var servico = CriarServicoValido();
            servico.AtualizarTempoMedio(new List<TimeSpan>());
            Assert.Null(servico.TempoMedioExecucao);
        }
    }
}
