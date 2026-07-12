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

        [Fact]
        public void CriarTelefone_DDDNulo_DeveThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Telefone(string.Empty, "55", "959972016"));
        }

        [Fact]
        public void CriarTelefone_DDINulo_DeveThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Telefone("11", string.Empty, "959972016"));
        }

        [Fact]
        public void CriarTelefone_NumeroNulo_DeveThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Telefone("11", "55", string.Empty));
        }

        [Fact]
        public void CriarTelefone_NumeroInvalido_DeveThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Telefone("11", "55", "1111111111"));
        }
        [Fact]
        public void CriarEndereco_LogradouroNulo_DeveThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Endereco(string.Empty, "10", null, "jabaquara", "são paulo", "SP", "01213001"));
        }
        [Fact]
        public void CriarEndereco_NumeroNulo_DeveThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Endereco("rua cleber", string.Empty, null, "jabaquara", "são paulo", "SP", "01213001"));
        }

        [Fact]
        public void CriarEndereco_cepNulo_DeveThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Endereco("rua cleber", "10", null, "jabaquara", "são paulo", "SP", string.Empty));
        }
        [Fact]
        public void CriarEndereco_CidadeNulo_DeveThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Endereco("rua cleber", "10", null, "jabaquara", string.Empty, "SP", "01213001"));
        }

        [Fact]
        public void CriarEndereco_UfNulo_DeveThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Endereco("rua cleber", "10", null, "jabaquara", "São Paulo", string.Empty, "01213001"));
        }

        [Fact]
        public void AlterarNome_ComNomeValido_DeveAlterarComSucesso()
        {
            var cliente = new Cliente("João Silva", new Cpf("50872558843"), Guid.NewGuid(),
                new Endereco("Rua A", "123", null, "Centro", "São Paulo", "SP", "01310100"),
                new Telefone("11", "55", "987654321"),
                new Email("joao@email.com"));

            cliente.AlterarNome("João Santos");

            Assert.Equal("João Santos", cliente.Nome);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void AlterarNome_ComNomeInvalido_DeveThrowArgumentException(string nome)
        {
            var cliente = new Cliente("João Silva", new Cpf("50872558843"), Guid.NewGuid(),
                new Endereco("Rua A", "123", null, "Centro", "São Paulo", "SP", "01310100"),
                new Telefone("11", "55", "987654321"),
                new Email("joao@email.com"));

            Assert.Throws<ArgumentException>(() => cliente.AlterarNome(nome));
        }

        [Fact]
        public void AlterarTelefone_ComTelefoneValido_DeveAlterarComSucesso()
        {
            var cliente = new Cliente("João Silva", new Cpf("50872558843"), Guid.NewGuid(),
                new Endereco("Rua A", "123", null, "Centro", "São Paulo", "SP", "01310100"),
                new Telefone("11", "55", "987654321"),
                new Email("joao@email.com"));

            var novoTelefone = new Telefone("21", "55", "999888777");
            cliente.AlterarTelefone(novoTelefone);

            Assert.Equal("21", cliente.Telefone.DDD);
        }

        [Fact]
        public void AlterarEndereco_ComEnderecoValido_DeveAlterarComSucesso()
        {
            var cliente = new Cliente("João Silva", new Cpf("50872558843"), Guid.NewGuid(),
                new Endereco("Rua A", "123", null, "Centro", "São Paulo", "SP", "01310100"),
                new Telefone("11", "55", "987654321"),
                new Email("joao@email.com"));

            var novoEndereco = new Endereco("Av. Brasil", "500", null, "Vila Nova", "Rio de Janeiro", "RJ", "20040020");
            cliente.AlterarEndereco(novoEndereco);

            Assert.Equal("Av. Brasil", cliente.Endereco.Logradouro);
        }

        [Fact]
        public void CriarCliente_SemCpfECnpj_DeveThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                new Cliente("João Silva", (Cpf)null, Guid.NewGuid(),
                    new Endereco("Rua A", "123", null, "Centro", "São Paulo", "SP", "01310100"),
                    new Telefone("11", "55", "987654321"),
                    new Email("joao@email.com")));
        }
    }
}
