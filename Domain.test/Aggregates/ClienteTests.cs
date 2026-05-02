using Domain.Aggregates.ClienteAggregates;
using Domain.ValueObjects;

namespace Domain.test.Aggregates;

public class ClienteTests
{
    private Guid _usuarioId = Guid.NewGuid();

    [Fact]
    public void Constructor_ClienteWithCpf_CreatesClienteSuccessfully()
    {
        // Arrange
        string nome = "João Silva";
        var cpf = new Cpf("11144477735");

        // Act
        var cliente = new Cliente(nome, cpf, _usuarioId);

        // Assert
        Assert.NotNull(cliente);
        Assert.Equal("João Silva", cliente.Nome);
        Assert.NotNull(cliente.Cpf);
        Assert.Equal("11144477735", cliente.Cpf.Valor);
        Assert.NotNull(cliente.Id);
        Assert.True(cliente.Ativo);
    }

    [Fact]
    public void Constructor_ClienteWithCnpj_CreatesClienteSuccessfully()
    {
        // Arrange
        string nome = "Empresa LTDA";
        var cnpj = new Cnpj("11222333000181");

        // Act
        var cliente = new Cliente(nome, cnpj, _usuarioId);

        // Assert
        Assert.NotNull(cliente);
        Assert.Equal("Empresa LTDA", cliente.Nome);
        Assert.NotNull(cliente.Cnpj);
        Assert.Equal("11222333000181", cliente.Cnpj.Valor);
    }

    [Fact]
    public void Constructor_EmptyNome_ThrowsArgumentException()
    {
        // Arrange
        var cpf = new Cpf("11144477735");

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => 
            new Cliente("", cpf, _usuarioId));
        Assert.Contains("não pode ser vazio", ex.Message);
    }

    [Fact]
    public void Constructor_NullCpf_ThrowsArgumentException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => 
            new Cliente("João Silva", (Cpf)null, _usuarioId));
        Assert.Contains("nulo", ex.Message);
    }

    [Fact]
    public void Constructor_NullCnpj_ThrowsArgumentException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => 
            new Cliente("Empresa", (Cnpj)null, _usuarioId));
        Assert.Contains("nulo", ex.Message);
    }

    [Fact]
    public void AlterarNome_ValidNome_ChangesName()
    {
        // Arrange
        var cpf = new Cpf("11144477735");
        var cliente = new Cliente("João Silva", cpf, _usuarioId);
        string novoNome = "João da Silva";

        // Act
        cliente.AlterarNome(novoNome);

        // Assert
        Assert.Equal("João da Silva", cliente.Nome);
    }

    [Fact]
    public void AlterarNome_EmptyNome_ThrowsArgumentException()
    {
        // Arrange
        var cpf = new Cpf("11144477735");
        var cliente = new Cliente("João Silva", cpf, _usuarioId);

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => cliente.AlterarNome(""));
        Assert.Contains("não pode ser vazio", ex.Message);
    }

    [Fact]
    public void AlterarEmails_ValidEmails_UpdatesEmails()
    {
        // Arrange
        var cpf = new Cpf("11144477735");
        var cliente = new Cliente("João Silva", cpf, _usuarioId);
        var emails = new List<Email>
        {
            new Email("joao@example.com"),
            new Email("joao.silva@example.com")
        };

        // Act
        cliente.AlterarEmails(emails);

        // Assert
        Assert.Equal(2, cliente.Emails.Count);
    }

    [Fact]
    public void AlterarEmails_NullEmails_ThrowsArgumentException()
    {
        // Arrange
        var cpf = new Cpf("11144477735");
        var cliente = new Cliente("João Silva", cpf, _usuarioId);

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => cliente.AlterarEmails(null));
        Assert.Contains("não pode ser nulo", ex.Message);
    }

    [Fact]
    public void AlterarEmails_DuplicateEmails_RemovesDuplicates()
    {
        // Arrange
        var cpf = new Cpf("11144477735");
        var cliente = new Cliente("João Silva", cpf, _usuarioId);
        var emails = new List<Email>
        {
            new Email("joao@example.com"),
            new Email("joao@example.com")
        };

        // Act
        cliente.AlterarEmails(emails);

        // Assert
        Assert.Single(cliente.Emails);
    }

    [Fact]
    public void AlterarTelefones_ValidTelefones_UpdatesTelefones()
    {
        // Arrange
        var cpf = new Cpf("11144477735");
        var cliente = new Cliente("João Silva", cpf, _usuarioId);
        var telefones = new List<Telefone>
        {
            new Telefone("11", "55", "98765-4321", Domain.Enums.ETipoTelefone.Celular),
            new Telefone("11", "55", "3333-4444", Domain.Enums.ETipoTelefone.Residencial)
        };

        // Act
        cliente.AlterarTelefones(telefones);

        // Assert
        Assert.Equal(2, cliente.Telefones.Count);
    }

    [Fact]
    public void AlterarTelefones_NullTelefones_ThrowsArgumentException()
    {
        // Arrange
        var cpf = new Cpf("11144477735");
        var cliente = new Cliente("João Silva", cpf, _usuarioId);

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => cliente.AlterarTelefones(null));
        Assert.Contains("não pode ser nulo", ex.Message);
    }

    [Fact]
    public void AlterarTelefones_DuplicateTelefones_RemovesDuplicates()
    {
        // Arrange
        var cpf = new Cpf("11144477735");
        var cliente = new Cliente("João Silva", cpf, _usuarioId);
        var telefones = new List<Telefone>
        {
            new Telefone("11", "55", "98765-4321", Domain.Enums.ETipoTelefone.Celular),
            new Telefone("11", "55", "98765-4321", Domain.Enums.ETipoTelefone.Residencial)
        };

        // Act
        cliente.AlterarTelefones(telefones);

        // Assert
        Assert.Single(cliente.Telefones);
    }

    [Fact]
    public void Inativar_ClienteAtivoBecomesInativo()
    {
        // Arrange
        var cpf = new Cpf("11144477735");
        var cliente = new Cliente("João Silva", cpf, _usuarioId);

        // Act
        cliente.Inativar();

        // Assert
        Assert.False(cliente.Ativo);
    }

    [Fact]
    public void RastrearAlteracao_UpdatesUsuarioAndDataAtualizacao()
    {
        // Arrange
        var cpf = new Cpf("11144477735");
        var cliente = new Cliente("João Silva", cpf, _usuarioId);
        var novoUsuarioId = Guid.NewGuid();
        var dataAtualizacao = DateTime.UtcNow;

        // Act
        cliente.RastrearAlteracao(novoUsuarioId, dataAtualizacao);

        // Assert
        Assert.Equal(novoUsuarioId, cliente.IdUsuarioAtualizacao);
        Assert.Equal(dataAtualizacao, cliente.DataAtualizacao);
    }
}
