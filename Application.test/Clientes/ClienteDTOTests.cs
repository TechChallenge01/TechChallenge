using Application.Clientes.DTOs.Requests;
using Application.Clientes.DTOs.Responses;
using Application.Clientes.DTOs.Shared;

namespace Application.test.Clientes;

public class ClienteDTOTests
{
    [Fact]
    public void ClienteRequestDTO_WithValidData_CreatesSuccessfully()
    {
        // Arrange
        var request = new ClienteRequestDTO
        {
            Nome = "João Silva",
            Cpf = "11144477735",
            Emails = new List<string> { "joao@example.com" },
            Telefones = new List<TelefoneDTO>
            {
                new TelefoneDTO { DDD = "11", DDI = "55", Numero = "98765-4321", Tipo = "Celular" }
            },
            Enderecos = new List<EnderecoDTO>
            {
                new EnderecoDTO
                {
                    Logradouro = "Rua A",
                    Numero = "123",
                    Complemento = "Apto 101",
                    Bairro = "Centro",
                    Cidade = "São Paulo",
                    Uf = "SP",
                    Cep = "01234-567"
                }
            }
        };

        // Assert
        Assert.NotNull(request);
        Assert.Equal("João Silva", request.Nome);
        Assert.Equal("11144477735", request.Cpf);
        Assert.Single(request.Emails);
        Assert.Single(request.Telefones);
        Assert.Single(request.Enderecos);
    }

    [Fact]
    public void EnderecoDTO_WithValidData_CreatesSuccessfully()
    {
        // Arrange
        var endereco = new EnderecoDTO
        {
            Logradouro = "Rua A",
            Numero = "123",
            Complemento = "Apto 101",
            Bairro = "Centro",
            Cidade = "São Paulo",
            Uf = "SP",
            Cep = "01234-567"
        };

        // Assert
        Assert.NotNull(endereco);
        Assert.Equal("Rua A", endereco.Logradouro);
        Assert.Equal("123", endereco.Numero);
        Assert.Equal("São Paulo", endereco.Cidade);
    }

    [Fact]
    public void TelefoneDTO_WithValidData_CreatesSuccessfully()
    {
        // Arrange
        var telefone = new TelefoneDTO
        {
            DDD = "11",
            DDI = "55",
            Numero = "98765-4321",
            Tipo = "Celular"
        };

        // Assert
        Assert.NotNull(telefone);
        Assert.Equal("11", telefone.DDD);
        Assert.Equal("55", telefone.DDI);
        Assert.Equal("98765-4321", telefone.Numero);
        Assert.Equal("Celular", telefone.Tipo);
    }

    [Fact]
    public void ClienteResponseDTO_WithValidData_CreatesSuccessfully()
    {
        // Arrange
        var response = new ClienteResponseDTO
        {
            Id = Guid.NewGuid(),
            Nome = "João Silva",
            Cpf = "11144477735",
            Cnpj = null,
            Emails = new List<string> { "joao@example.com" },
            Telefones = new List<TelefoneDTO>(),
            Enderecos = new List<EnderecoDTO>()
        };

        // Assert
        Assert.NotNull(response);
        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal("João Silva", response.Nome);
    }
}
