using Domain.Entities;
using Domain.ValueObjects;

namespace Domain.test.Tests
{
    public class VeiculoTests
    {
        private static Veiculo CriarVeiculoValido() => new Veiculo("Civic", "Honda", Guid.NewGuid(), 2020, new Placa("ABC1234"), "Preto", Guid.NewGuid());

        [Fact]
        public void CriarVeiculo_ComDadosValidos_DeveCriarComSucesso()
        {
            var veiculo = CriarVeiculoValido();

            Assert.NotNull(veiculo);
            Assert.Equal("Civic", veiculo.Modelo);
            Assert.Equal("Honda", veiculo.MarcaVeiculo);
            Assert.Equal(2020, veiculo.Ano);
            Assert.Equal("Preto", veiculo.Cor);
        }

        [Theory]
        [InlineData(1800)]
        [InlineData(2030)]
        public void CriarVeiculo_ComAnoInvalido_DeveThrowArgumentException(int ano)
        {
            Assert.Throws<ArgumentException>(() =>
                new Veiculo("Civic", "Honda", Guid.NewGuid(), ano, new Placa("ABC1234"), "Preto", Guid.NewGuid()));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void CriarVeiculo_ComModeloInvalido_DeveThrowArgumentException(string modelo)
        {
            Assert.Throws<ArgumentException>(() =>
                new Veiculo(modelo, "Honda", Guid.NewGuid(), 2020, new Placa("ABC1234"), "Preto", Guid.NewGuid()));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void CriarVeiculo_ComMarcaInvalida_DeveThrowArgumentException(string marca)
        {
            Assert.Throws<ArgumentException>(() =>
                new Veiculo("Civic", marca, Guid.NewGuid(), 2020, new Placa("ABC1234"), "Preto", Guid.NewGuid()));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void CriarVeiculo_ComCorInvalida_DeveThrowArgumentException(string cor)
        {
            Assert.Throws<ArgumentException>(() =>
                new Veiculo("Civic", "Honda", Guid.NewGuid(), 2020, new Placa("ABC1234"), cor, Guid.NewGuid()));
        }

        [Fact]
        public void CriarVeiculo_ComPlacaNula_DeveThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                new Veiculo("Civic", "Honda", Guid.NewGuid(), 2020, null, "Preto", Guid.NewGuid()));
        }
        
        [Fact]
        public void AlterarModelo_ComModeloValido_DeveAlterarComSucesso()
        {
            var veiculo = CriarVeiculoValido();
            veiculo.AlterarModelo("HRV");
            Assert.Equal("HRV", veiculo.Modelo);
        }

        [Fact]
        public void AlterarModelo_ComModeloInvalido_DeveThrowArgumentException()
        {
            var veiculo = CriarVeiculoValido();
            Assert.Throws<ArgumentException>(() => veiculo.AlterarModelo(""));
        }

        [Fact]
        public void AlterarMarca_ComMarcaValida_DeveAlterarComSucesso()
        {
            var veiculo = CriarVeiculoValido();
            veiculo.AlterarMarcaVeiculo("Toyota");
            Assert.Equal("Toyota", veiculo.MarcaVeiculo);
        }

        [Fact]
        public void AlterarMarca_ComMarcaInvalida_DeveThrowArgumentException()
        {
            var veiculo = CriarVeiculoValido();
            Assert.Throws<ArgumentException>(() => veiculo.AlterarMarcaVeiculo("   "));
        }

        [Fact]
        public void AlterarAno_ComAnoValido_DeveAlterarComSucesso()
        {
            var veiculo = CriarVeiculoValido();
            veiculo.AlterarAno(2022);
            Assert.Equal(2022, veiculo.Ano);
        }

        [Fact]
        public void AlterarAno_ComAnoInvalido_DeveThrowArgumentException()
        {
            var veiculo = CriarVeiculoValido();
            Assert.Throws<ArgumentException>(() => veiculo.AlterarAno(1800));
        }

        [Fact]
        public void AlterarCor_ComCorValida_DeveAlterarComSucesso()
        {
            var veiculo = CriarVeiculoValido();
            veiculo.AlterarCor("Branco");
            Assert.Equal("Branco", veiculo.Cor);
        }

        [Fact]
        public void AlterarCor_ComCorInvalida_DeveThrowArgumentException()
        {
            var veiculo = CriarVeiculoValido();
            Assert.Throws<ArgumentException>(() => veiculo.AlterarCor(""));
        }

        [Fact]
        public void AlterarCliente_DeveAlterarClienteId()
        {
            var veiculo = CriarVeiculoValido();
            var novoClienteId = Guid.NewGuid();
            veiculo.AlterarCliente(novoClienteId);
            Assert.Equal(novoClienteId, veiculo.ClienteId);
        }
    }
}
