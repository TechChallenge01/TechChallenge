using Domain.ValueObjects;
using Xunit;

namespace Domain.test.Tests
{
    public class ValueObjectsTests
    {
        [Fact]
        public void CriarCpf_ComCpfValido_DeveCriarComSucesso()
        {
            // Act
            var cpf = new Cpf("50872558843");

            // Assert
            Assert.NotNull(cpf);
            Assert.Equal("50872558843", cpf.Valor);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("111111111111")]
        public void CriarCpf_ComCpfInvalido_DeveThrowArgumentException(string cpfInvalido)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new Cpf(cpfInvalido));
        }

        [Fact]
        public void CriarEmail_ComEmailValido_DeveCriarComSucesso()
        {
            // Act
            var email = new Email("teste@email.com");

            // Assert
            Assert.NotNull(email);
            Assert.Equal("teste@email.com", email.EnderecoEmail);
        }

        [Theory]
        [InlineData("")]
        [InlineData("email_invalido")]
        [InlineData(null)]
        public void CriarEmail_ComEmailInvalido_DeveThrowArgumentException(string emailInvalido)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new Email(emailInvalido));
        }

        [Fact]
        public void CriarPlaca_ComPlacaValidaAntiga_DeveCriarComSucesso()
        {
            // Act
            var placa = new Placa("ABC1234");

            // Assert
            Assert.NotNull(placa);
            Assert.Equal("ABC1234", placa.Valor);
        }

        [Fact]
        public void CriarPlaca_ComPlacaValidaMercosul_DeveCriarComSucesso()
        {
            // Act
            var placa = new Placa("ABC1D23");

            // Assert
            Assert.NotNull(placa);
            Assert.Equal("ABC1D23", placa.Valor);
        }

        [Fact]
        public void CriarTelefone_ComDadosValidos_DeveCriarComSucesso()
        {
            // Act
            var telefone = new Telefone("11", "55", "987654321");

            // Assert
            Assert.NotNull(telefone);
            Assert.Equal("11", telefone.DDD);
            Assert.Equal("55", telefone.DDI);
            Assert.Equal("987654321", telefone.Numero);
        }

        [Fact]
        public void CriarEndereco_ComDadosValidos_DeveCriarComSucesso()
        {
            // Act
            var endereco = new Endereco("Rua A", "123", null, "Centro", "São Paulo", "SP", "01310100");

            // Assert
            Assert.NotNull(endereco);
            Assert.Equal("Rua A", endereco.Logradouro);
            Assert.Equal("123", endereco.Numero);
            Assert.Equal("São Paulo", endereco.Cidade);
        }
    }
}
