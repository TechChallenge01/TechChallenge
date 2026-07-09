using Application.Gateways.Clientes;
using Application.Interfaces;
using Application.UseCases.Clientes;
using Moq;
using Shared.DTOs.Clientes.Input;
using Shared.DTOs.Clientes.Request;
using Shared.DTOs.Clientes.Shared;

namespace Application.test.Tests;

public class ClienteUseCaseTests
{
    private static ClienteInputDTO CriarClienteInputDTO(Guid? id = null) => new ClienteInputDTO
    {
        Id = id ?? Guid.NewGuid(),
        Nome = "João Silva",
        Cpf = "50872558843",
        Email = "joao@email.com",
        Telefone = new TelefoneDTO { DDD = "11", DDI = "55", Numero = "987654321" },
        Endereco = new EnderecoDTO { Logradouro = "Rua A", Numero = "1", Bairro = "Centro", Cidade = "São Paulo", Uf = "SP", Cep = "01310100" },
        Veiculos = new List<Guid>()
    };

    private static ClienteRequestDTO CriarClienteRequestDTO() => new ClienteRequestDTO
    {
        Nome = "João Silva",
        Cpf = "50872558843",
        Email = "joao@email.com",
        Telefone = new TelefoneDTO { DDD = "11", DDI = "55", Numero = "987654321" },
        Endereco = new EnderecoDTO { Logradouro = "Rua A", Numero = "1", Bairro = "Centro", Cidade = "São Paulo", Uf = "SP", Cep = "01310100" },
        Veiculos = new List<Guid>()
    };

    private static Mock<IClienteDataSource> CriarMockDataSource(Guid? idRetorno = null)
    {
        var id = idRetorno ?? Guid.NewGuid();
        var mock = new Mock<IClienteDataSource>();

        mock.Setup(m => m.Create(It.IsAny<ClienteInputDTO>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        mock.Setup(m => m.Update(It.IsAny<ClienteInputDTO>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        mock.Setup(m => m.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarClienteInputDTO(id));
        mock.Setup(m => m.GetByCpf(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ClienteInputDTO)null);
        mock.Setup(m => m.GetByCnpj(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ClienteInputDTO)null);
        mock.Setup(m => m.GetPaginated(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<ClienteInputDTO> { CriarClienteInputDTO(id) }, 1));

        return mock;
    }

    [Fact]
    public async Task CriarCliente_ComCpf_DeveRetornarGuidValido()
    {
        var mock = CriarMockDataSource();
        var gateway = ClienteGateway.Create(mock.Object);
        var useCase = CreateUseCase.Create(gateway);

        var id = await useCase.Run(CriarClienteRequestDTO(), Guid.NewGuid(), CancellationToken.None);

        Assert.NotEqual(Guid.Empty, id);
    }

    [Fact]
    public async Task CriarCliente_ComCpfJaCadastrado_DeveThrowInvalidOperationException()
    {
        var mock = CriarMockDataSource();
        mock.Setup(m => m.GetByCpf(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarClienteInputDTO());
        var gateway = ClienteGateway.Create(mock.Object);
        var useCase = CreateUseCase.Create(gateway);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            useCase.Run(CriarClienteRequestDTO(), Guid.NewGuid(), CancellationToken.None));
    }

    [Fact]
    public async Task CriarCliente_SemCpfECnpj_DeveThrowArgumentException()
    {
        var mock = CriarMockDataSource();
        var gateway = ClienteGateway.Create(mock.Object);
        var useCase = CreateUseCase.Create(gateway);
        var request = new ClienteRequestDTO
        {
            Nome = "João",
            Email = "joao@email.com",
            Telefone = new TelefoneDTO { DDD = "11", DDI = "55", Numero = "987654321" },
            Endereco = new EnderecoDTO { Logradouro = "Rua A", Numero = "1", Bairro = "Centro", Cidade = "São Paulo", Uf = "SP", Cep = "01310100" }
        };

        await Assert.ThrowsAsync<ArgumentException>(() =>
            useCase.Run(request, Guid.NewGuid(), CancellationToken.None));
    }

    [Fact]
    public async Task AtualizarCliente_ComDadosValidos_DeveCallUpdateNoDataSource()
    {
        var id = Guid.NewGuid();
        var mock = CriarMockDataSource(id);
        var gateway = ClienteGateway.Create(mock.Object);
        var useCase = UpdateUseCase.Create(gateway);

        await useCase.Run(Guid.NewGuid(), id, CriarClienteRequestDTO(), CancellationToken.None);

        mock.Verify(m => m.Update(It.IsAny<ClienteInputDTO>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AtualizarCliente_ClienteNaoEncontrado_DeveThrowKeyNotFoundException()
    {
        var mock = new Mock<IClienteDataSource>();
        mock.Setup(m => m.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ClienteInputDTO)null);
        var gateway = ClienteGateway.Create(mock.Object);
        var useCase = UpdateUseCase.Create(gateway);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            useCase.Run(Guid.NewGuid(), Guid.NewGuid(), CriarClienteRequestDTO(), CancellationToken.None));
    }

    [Fact]
    public async Task DeletarCliente_DeveInativarECallUpdate()
    {
        var id = Guid.NewGuid();
        var mock = CriarMockDataSource(id);
        var gateway = ClienteGateway.Create(mock.Object);
        var useCase = DeleteUseCase.Create(gateway);

        await useCase.Run(Guid.NewGuid(), id, CancellationToken.None);

        mock.Verify(m => m.Update(It.Is<ClienteInputDTO>(dto => dto.Ativo == false), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeletarCliente_ClienteNaoEncontrado_DeveThrowKeyNotFoundException()
    {
        var mock = new Mock<IClienteDataSource>();
        mock.Setup(m => m.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ClienteInputDTO)null);
        var gateway = ClienteGateway.Create(mock.Object);
        var useCase = DeleteUseCase.Create(gateway);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            useCase.Run(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));
    }

    [Fact]
    public async Task ObterClientePorId_DeveRetornarClienteCorreto()
    {
        var id = Guid.NewGuid();
        var mock = CriarMockDataSource(id);
        var gateway = ClienteGateway.Create(mock.Object);
        var useCase = GetByIdUseCase.Create(gateway);

        var cliente = await useCase.Run(id, CancellationToken.None);

        Assert.NotNull(cliente);
        Assert.Equal(id, cliente.Id);
        Assert.Equal("João Silva", cliente.Nome);
    }

    [Fact]
    public async Task ObterClientesPaginados_DeveRetornarListaETotal()
    {
        var mock = CriarMockDataSource();
        var gateway = ClienteGateway.Create(mock.Object);
        var useCase = GetPaginatedUseCase.Create(gateway);

        var (clientes, total) = await useCase.Run(1, 10, CancellationToken.None);

        Assert.NotEmpty(clientes);
        Assert.Equal(1, total);
    }
}
