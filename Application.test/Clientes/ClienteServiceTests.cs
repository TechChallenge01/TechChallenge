using Application.Clientes.DTOs.Requests;
using Application.Clientes.DTOs.Responses;
using Application.Clientes.DTOs.Shared;
using Application.Clientes.Services;
using Application.UnitOfWork;
using Domain.Aggregates.ClienteAggregates.Repositories;
using Shared.DTOs;
using System.Net;

namespace Application.test.Clientes;

public class ClienteServiceTests
{
    private readonly Mock<IClienteRepository> _clienteRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ClienteService _clienteService;

    public ClienteServiceTests()
    {
        _clienteRepositoryMock = new Mock<IClienteRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _clienteService = new ClienteService(_clienteRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Create_ClienteWithValidCpf_ReturnsCreatedResult()
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

        _clienteRepositoryMock.Setup(x => x.Create(It.IsAny<Domain.Aggregates.ClienteAggregates.Cliente>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _clienteService.Create(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(HttpStatusCode.Created, result.StatusCode);
        Assert.NotEmpty(result.Data.ToString());
        _clienteRepositoryMock.Verify(x => x.Create(It.IsAny<Domain.Aggregates.ClienteAggregates.Cliente>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Create_ClienteWithValidCnpj_ReturnsCreatedResult()
    {
        // Arrange
        var request = new ClienteRequestDTO
        {
            Nome = "Empresa LTDA",
            Cnpj = "11222333000181",
            Emails = new List<string> { "empresa@example.com" },
            Telefones = new List<TelefoneDTO>
            {
                new TelefoneDTO { DDD = "11", DDI = "55", Numero = "3333-4444", Tipo = "Comercial" }
            },
            Enderecos = new List<EnderecoDTO>
            {
                new EnderecoDTO 
                { 
                    Logradouro = "Av. Principal", 
                    Numero = "500", 
                    Complemento = "",
                    Bairro = "Centro",
                    Cidade = "São Paulo",
                    Uf = "SP",
                    Cep = "01234-567"
                }
            }
        };

        _clienteRepositoryMock.Setup(x => x.Create(It.IsAny<Domain.Aggregates.ClienteAggregates.Cliente>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _clienteService.Create(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(HttpStatusCode.Created, result.StatusCode);
        _clienteRepositoryMock.Verify(x => x.Create(It.IsAny<Domain.Aggregates.ClienteAggregates.Cliente>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Create_ClienteWithoutCpfAndCnpj_ReturnsBadRequest()
    {
        // Arrange
        var request = new ClienteRequestDTO
        {
            Nome = "João Silva",
            Cpf = null,
            Cnpj = null,
            Emails = new List<string> { "joao@example.com" },
            Telefones = new List<TelefoneDTO>(),
            Enderecos = new List<EnderecoDTO>()
        };

        // Act
        var result = await _clienteService.Create(request, CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.Contains("CPF ou o CNPJ", result.Message);
    }

    [Fact]
    public async Task Create_ClienteWithBothCpfAndCnpj_ReturnsBadRequest()
    {
        // Arrange
        var request = new ClienteRequestDTO
        {
            Nome = "João Silva",
            Cpf = "11144477735",
            Cnpj = "11222333000181",
            Emails = new List<string> { "joao@example.com" },
            Telefones = new List<TelefoneDTO>(),
            Enderecos = new List<EnderecoDTO>()
        };

        // Act
        var result = await _clienteService.Create(request, CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.Contains("ambos", result.Message);
    }

    [Fact]
    public async Task Create_ClienteWithInvalidTelefoneType_ReturnsBadRequest()
    {
        // Arrange
        var request = new ClienteRequestDTO
        {
            Nome = "João Silva",
            Cpf = "11144477735",
            Emails = new List<string> { "joao@example.com" },
            Telefones = new List<TelefoneDTO>
            {
                new TelefoneDTO { DDD = "11", DDI = "55", Numero = "98765-4321", Tipo = "TipoInvalido" }
            },
            Enderecos = new List<EnderecoDTO>()
        };

        // Act
        var result = await _clienteService.Create(request, CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.Contains("Tipo de telefone", result.Message);
    }

    [Fact]
    public async Task Create_ClienteWithRepositoryException_ReturnsInternalServerError()
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
            Enderecos = new List<EnderecoDTO>()
        };

        _clienteRepositoryMock.Setup(x => x.Create(It.IsAny<Domain.Aggregates.ClienteAggregates.Cliente>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _clienteService.Create(request, CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
    }
}
