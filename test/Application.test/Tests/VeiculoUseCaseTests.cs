using Application.Gateways.Clientes;
using Application.Gateways.Veiculos;
using Application.Interfaces;
using Application.UseCases.Veiculos;
using Moq;
using Shared.DTOs.Clientes.Input;
using Shared.DTOs.Clientes.Shared;
using Shared.DTOs.Veiculos.Input;
using Shared.DTOs.Veiculos.Requests;

namespace Application.test.Tests;

public class VeiculoUseCaseTests
{
    private static Mock<IVeiculoDataSource> CriarMockVeiculo(Guid? idRetorno = null)
    {
        var id = idRetorno ?? Guid.NewGuid();
        var mock = new Mock<IVeiculoDataSource>();

        mock.Setup(m => m.Create(It.IsAny<VeiculoInputDTO>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        mock.Setup(m => m.Update(It.IsAny<VeiculoInputDTO>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        mock.Setup(m => m.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(new VeiculoInputDTO { Id = id, Modelo = "Civic", MarcaVeiculo = "Honda", ClienteId = Guid.NewGuid(), Ano = 2020, Placa = "ABC1234", Cor = "Preto" }));
        mock.Setup(m => m.GetPaginated(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<VeiculoInputDTO> { new VeiculoInputDTO { Id = id, Modelo = "Civic", MarcaVeiculo = "Honda", ClienteId = Guid.NewGuid(), Ano = 2020, Placa = "ABC1234", Cor = "Preto" } }, 1));

        return mock;
    }

    private static Mock<IClienteDataSource> CriarMockCliente(Guid? clienteId = null)
    {
        var id = clienteId ?? Guid.NewGuid();
        var mock = new Mock<IClienteDataSource>();
        var dto = new ClienteInputDTO
        {
            Id = id,
            Nome = "João Silva",
            Cpf = "50872558843",
            Email = "joao@email.com",
            Telefone = new TelefoneDTO { DDD = "11", DDI = "55", Numero = "987654321" },
            Endereco = new EnderecoDTO { Logradouro = "Rua A", Numero = "1", Bairro = "Centro", Cidade = "São Paulo", Uf = "SP", Cep = "01310100" },
            Veiculos = new List<Guid>()
        };
        mock.Setup(m => m.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);
        return mock;
    }

    [Fact]
    public async Task CriarVeiculo_DeveRetornarGuidValido()
    {
        var mockV = CriarMockVeiculo();
        var mockC = CriarMockCliente();
        var gateway = VeiculoGateway.Create(mockV.Object);
        var clienteGateway = ClienteGateway.Create(mockC.Object);
        var useCase = CreateUseCase.Create(gateway, clienteGateway);
        var request = new VeiculoRequestDTO { Modelo = "Civic", MarcaVeiculo = "Honda", ClienteId = Guid.NewGuid(), Ano = 2020, Placa = "ABC1234", Cor = "Preto" };

        var id = await useCase.Run(request, Guid.NewGuid(), CancellationToken.None);

        Assert.NotEqual(Guid.Empty, id);
    }

    [Fact]
    public async Task CriarVeiculo_DeveCallCreateNoDataSource()
    {
        var mockV = CriarMockVeiculo();
        var mockC = CriarMockCliente();
        var gateway = VeiculoGateway.Create(mockV.Object);
        var clienteGateway = ClienteGateway.Create(mockC.Object);
        var useCase = CreateUseCase.Create(gateway, clienteGateway);
        var request = new VeiculoRequestDTO { Modelo = "Civic", MarcaVeiculo = "Honda", ClienteId = Guid.NewGuid(), Ano = 2020, Placa = "ABC1234", Cor = "Preto" };

        await useCase.Run(request, Guid.NewGuid(), CancellationToken.None);

        mockV.Verify(m => m.Create(It.IsAny<VeiculoInputDTO>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AtualizarVeiculo_ComDadosValidos_DeveCallUpdateNoDataSource()
    {
        var id = Guid.NewGuid();
        var mockV = CriarMockVeiculo(id);
        var gateway = VeiculoGateway.Create(mockV.Object);
        var useCase = UpdateUseCase.Create(gateway);
        var request = new VeiculoRequestDTO { Modelo = "HRV", MarcaVeiculo = "Honda", ClienteId = Guid.NewGuid(), Ano = 2022, Placa = "XYZ9876", Cor = "Branco" };

        await useCase.Run(id, Guid.NewGuid(), request, CancellationToken.None);

        mockV.Verify(m => m.Update(It.IsAny<VeiculoInputDTO>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AtualizarVeiculo_VeiculoNaoEncontrado_DeveThrowKeyNotFoundException()
    {
        var mock = new Mock<IVeiculoDataSource>();
        mock.Setup(m => m.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<VeiculoInputDTO>(null));
        var gateway = VeiculoGateway.Create(mock.Object);
        var useCase = UpdateUseCase.Create(gateway);
        var request = new VeiculoRequestDTO { Modelo = "X", MarcaVeiculo = "Y", ClienteId = Guid.NewGuid(), Ano = 2020, Placa = "ABC1234", Cor = "Z" };

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            useCase.Run(Guid.NewGuid(), Guid.NewGuid(), request, CancellationToken.None));
    }

    [Fact]
    public async Task DeletarVeiculo_DeveInativarECallUpdate()
    {
        var id = Guid.NewGuid();
        var mockV = CriarMockVeiculo(id);
        var gateway = VeiculoGateway.Create(mockV.Object);
        var useCase = DeleteUseCase.Create(gateway);

        await useCase.Run(id, Guid.NewGuid(), CancellationToken.None);

        mockV.Verify(m => m.Update(It.IsAny<VeiculoInputDTO>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeletarVeiculo_VeiculoNaoEncontrado_DeveThrowKeyNotFoundException()
    {
        var mock = new Mock<IVeiculoDataSource>();
        mock.Setup(m => m.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<VeiculoInputDTO>(null));
        var gateway = VeiculoGateway.Create(mock.Object);
        var useCase = DeleteUseCase.Create(gateway);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            useCase.Run(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));
    }

    [Fact]
    public async Task ObterVeiculoPorId_DeveRetornarVeiculoCorreto()
    {
        var id = Guid.NewGuid();
        var mockV = CriarMockVeiculo(id);
        var gateway = VeiculoGateway.Create(mockV.Object);
        var useCase = GetByIdUseCase.Create(gateway);

        var veiculo = await useCase.Run(id, CancellationToken.None);

        Assert.NotNull(veiculo);
        Assert.Equal(id, veiculo.Id);
        Assert.Equal("Civic", veiculo.Modelo);
    }

    [Fact]
    public async Task ObterVeiculoPorId_VeiculoNaoEncontrado_DeveRetornarNull()
    {
        var mock = new Mock<IVeiculoDataSource>();
        mock.Setup(m => m.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<VeiculoInputDTO>(null));
        var gateway = VeiculoGateway.Create(mock.Object);
        var useCase = GetByIdUseCase.Create(gateway);

        var veiculo = await useCase.Run(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(veiculo);
    }

    [Fact]
    public async Task ObterVeiculosPaginados_DeveRetornarListaETotal()
    {
        var mockV = CriarMockVeiculo();
        var gateway = VeiculoGateway.Create(mockV.Object);
        var useCase = GetPaginatedUseCase.Create(gateway);

        var (veiculos, total) = await useCase.Run(1, 10, CancellationToken.None);

        Assert.NotEmpty(veiculos);
        Assert.Equal(1, total);
    }
}
