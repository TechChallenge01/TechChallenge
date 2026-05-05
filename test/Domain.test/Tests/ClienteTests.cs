using Domain.Aggregates.ClienteAggregates;
using Domain.ValueObjects;

namespace Domain.test.Tests
{
    public class ClienteTests
    {
        [Fact]
        public void CriarClienteComCpf_ComDadosValidos_DeveCriarComSucesso()
        {
            // Arrange
            var nome = "João Silva";
            var cpf = new Cpf("50872558843");
            var email = new Email("joao@email.com");
            var telefone = new Telefone("11", "55", "987654321");
            var endereco = new Endereco("Rua A", "123", null, "Centro", "São Paulo", "SP", "01310100");
            var usuarioId = Guid.NewGuid();

            // Act
            var cliente = new Cliente(nome, cpf, usuarioId, endereco, telefone, email);

            // Assert
            Assert.NotNull(cliente);
            Assert.Equal(nome, cliente.Nome);
            Assert.Equal(cpf, cliente.Cpf);
        }

        [Fact]
        public void CriarClienteComCnpj_ComDadosValidos_DeveCriarComSucesso()
        {
            // Arrange
            var nome = "Empresa Ltda";
            var cnpj = new Cnpj("12345678000195");
            var email = new Email("empresa@email.com");
            var telefone = new Telefone("11", "55", "33333333");
            var endereco = new Endereco("Av. Tecnológica", "2000", null, "Tecnópolis", "São Paulo", "SP", "01310100");
            var usuarioId = Guid.NewGuid();

            // Act
            var cliente = new Cliente(nome, cnpj, usuarioId, endereco, telefone, email);

            // Assert
            Assert.NotNull(cliente);
            Assert.Equal(nome, cliente.Nome);
            Assert.Equal(cnpj, cliente.Cnpj);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void CriarCliente_ComNomeInvalido_DeveThrowArgumentException(string nome)
        {
            // Arrange
            var cpf = new Cpf("50872558843");
            var email = new Email("joao@email.com");
            var telefone = new Telefone("11", "55", "987654321");
            var endereco = new Endereco("Rua A", "123", null, "Centro", "São Paulo", "SP", "01310100");

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new Cliente(nome, cpf, Guid.NewGuid(), endereco, telefone, email));
        }

        [Fact]
        public void AlterarEmail_ComEmailValido_DeveAlterarComSucesso()
        {
            // Arrange
            var cliente = new Cliente("João Silva", new Cpf("50872558843"), Guid.NewGuid(),
                new Endereco("Rua A", "123", null, "Centro", "São Paulo", "SP", "01310100"),
                new Telefone("11", "55", "987654321"),
                new Email("joao@email.com"));

            var novoEmail = new Email("joao.novo@email.com");

            // Act
            cliente.AlterarEmail(novoEmail);

            // Assert
            Assert.Equal(novoEmail.EnderecoEmail, cliente.Email.EnderecoEmail);
        }
    }
}
