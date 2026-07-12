using Application.Gateways.OrdemServicos;
using Application.Gateways.Servicos;
using Application.Interfaces;
using Application.UseCases.OrdensServicos;
using Moq;
using Shared.DTOs.Estoques.Input;
using Shared.DTOs.OrdemServicos.Input;
using Shared.DTOs.OrdemServicos.Request;
using Shared.DTOs.OrdemServicos.Shared;
using Shared.DTOs.Servicos.Input;

namespace Application.test.Tests;

public class OrdemServicoUseCaseTests
{
    private static OrdemServicoInputDTO CriarOrdemServicoInputDTO(Guid? id = null, string status = "Recebida") => new OrdemServicoInputDTO
    {
        Id = id ?? Guid.NewGuid(),
        ClienteId = Guid.NewGuid(),
        VeiculoId = Guid.NewGuid(),
        StatusOS = status,
        ValorTotal = 0,
        ValorDesconto = 0,
        IdUsuarioCriacao = Guid.NewGuid(),
        DataCriacao = DateTime.UtcNow,
        Ativo = true,
        Pecas = new List<OrdemServicoPecaDTO>(),
        Servicos = new List<OrdemServicoServicoDTO>(),
        Insumos = new List<OrdemServicoInsumoDTO>()
    };

    private static Mock<IOrdemServicoDataSource> CriarMockDataSource(Guid? idRetorno = null, string status = "Recebida")
    {
        var id = idRetorno ?? Guid.NewGuid();
        var mock = new Mock<IOrdemServicoDataSource>();

        mock.Setup(m => m.Create(It.IsAny<OrdemServicoInputDTO>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        mock.Setup(m => m.Update(It.IsAny<OrdemServicoInputDTO>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        mock.Setup(m => m.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarOrdemServicoInputDTO(id, status));
        mock.Setup(m => m.GetPaginated(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<OrdemServicoInputDTO> { CriarOrdemServicoInputDTO(id) }, 1));
        mock.Setup(m => m.GetByClienteId(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<OrdemServicoInputDTO> { CriarOrdemServicoInputDTO(id) });
        mock.Setup(m => m.GetByStatus(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<OrdemServicoInputDTO> { CriarOrdemServicoInputDTO(id, status) });
        mock.Setup(m => m.GetByIdsSTimeSpanDataExecucao(It.IsAny<ICollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TimeSpan?> { TimeSpan.FromMinutes(30) });

        return mock;
    }

    [Fact]
    public async Task ObterOrdemServicoPorId_DeveRetornarOrdemCorreta()
    {
        var id = Guid.NewGuid();
        var mock = CriarMockDataSource(id);
        var gateway = OrdemServicoGateway.Create(mock.Object);
        var useCase = GetByIdUseCase.Create(gateway);

        var os = await useCase.Run(id, CancellationToken.None);

        Assert.NotNull(os);
        Assert.Equal(id, os.Id);
    }

    [Fact]
    public async Task ObterOrdemServicoPorId_NaoEncontrada_DeveRetornarNull()
    {
        var mock = new Mock<IOrdemServicoDataSource>();
        mock.Setup(m => m.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<OrdemServicoInputDTO?>(null));
        var gateway = OrdemServicoGateway.Create(mock.Object);
        var useCase = GetByIdUseCase.Create(gateway);

        var os = await useCase.Run(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(os);
    }

    [Fact]
    public async Task ObterOrdensServicosPaginadas_DeveRetornarListaETotal()
    {
        var mock = CriarMockDataSource();
        var gateway = OrdemServicoGateway.Create(mock.Object);
        var useCase = GetPaginatedUseCase.Create(gateway);

        var (ordens, total) = await useCase.Run(1, 10, CancellationToken.None);

        Assert.NotEmpty(ordens);
        Assert.Equal(1, total);
    }

    [Fact]
    public async Task IniciarDiagnostico_DeveCallUpdateNoDataSource()
    {
        var id = Guid.NewGuid();
        var mock = CriarMockDataSource(id, "Recebida");
        var gateway = OrdemServicoGateway.Create(mock.Object);
        var useCase = IniciarDiagnosticoUseCase.Create(gateway);

        await useCase.Run(id, Guid.NewGuid(), CancellationToken.None);

        mock.Verify(m => m.Update(It.IsAny<OrdemServicoInputDTO>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task IniciarDiagnostico_OrdemNaoEncontrada_DeveThrowKeyNotFoundException()
    {
        var mock = new Mock<IOrdemServicoDataSource>();
        mock.Setup(m => m.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<OrdemServicoInputDTO?>(null));
        var gateway = OrdemServicoGateway.Create(mock.Object);
        var useCase = IniciarDiagnosticoUseCase.Create(gateway);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            useCase.Run(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));
    }

    [Fact]
    public async Task CancelarOrdemServico_DeveCallUpdateNoDataSource()
    {
        var id = Guid.NewGuid();
        var mockOS = CriarMockDataSource(id, "AguardandoAprovacao");
        var mockEstoque = new Mock<IEstoqueDataSource>();
        mockEstoque.Setup(m => m.UpdateEstoques(It.IsAny<ICollection<EstoqueInputDTO>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        mockEstoque.Setup(m => m.GetByPecasIds(It.IsAny<ICollection<Guid>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<List<EstoqueInputDTO>?>(new List<EstoqueInputDTO>()));
        mockEstoque.Setup(m => m.GetByInsumosIds(It.IsAny<ICollection<Guid>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<List<EstoqueInputDTO>?>(new List<EstoqueInputDTO>()));

        var osGateway = OrdemServicoGateway.Create(mockOS.Object);
        var pecaGateway = Application.Gateways.Pecas.PecaGateway.Create(new Mock<IPecaDataSource>().Object);
        var insumoGateway = Application.Gateways.Insumos.InsumoGateway.Create(new Mock<IInsumoDataSource>().Object);
        var estoqueGateway = Application.Gateways.Estoques.EstoqueGateway.Create(mockEstoque.Object);
        var useCase = CancelarOrdemServicoUseCase.Create(osGateway, pecaGateway, insumoGateway, estoqueGateway);

        await useCase.Run(id, Guid.NewGuid(), CancellationToken.None);

        mockOS.Verify(m => m.Update(It.IsAny<OrdemServicoInputDTO>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CancelarOrdemServico_OrdemNaoEncontrada_DeveThrowKeyNotFoundException()
    {
        var mock = new Mock<IOrdemServicoDataSource>();
        mock.Setup(m => m.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<OrdemServicoInputDTO?>(null));
        var osGateway = OrdemServicoGateway.Create(mock.Object);
        var pecaGateway = Application.Gateways.Pecas.PecaGateway.Create(new Mock<IPecaDataSource>().Object);
        var insumoGateway = Application.Gateways.Insumos.InsumoGateway.Create(new Mock<IInsumoDataSource>().Object);
        var estoqueGateway = Application.Gateways.Estoques.EstoqueGateway.Create(new Mock<IEstoqueDataSource>().Object);
        var useCase = CancelarOrdemServicoUseCase.Create(osGateway, pecaGateway, insumoGateway, estoqueGateway);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            useCase.Run(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));
    }

    [Fact]
    public async Task RegistrarEntrega_DeveCallUpdateNoDataSource()
    {
        var id = Guid.NewGuid();
        var mock = CriarMockDataSource(id, "Finalizada");
        var gateway = OrdemServicoGateway.Create(mock.Object);
        var useCase = RegistrarEntregaUseCase.Create(gateway);

        await useCase.Run(id, Guid.NewGuid(), CancellationToken.None);

        mock.Verify(m => m.Update(It.IsAny<OrdemServicoInputDTO>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegistrarEntrega_OrdemNaoEncontrada_DeveThrowKeyNotFoundException()
    {
        var mock = new Mock<IOrdemServicoDataSource>();
        mock.Setup(m => m.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<OrdemServicoInputDTO?>(null));
        var gateway = OrdemServicoGateway.Create(mock.Object);
        var useCase = RegistrarEntregaUseCase.Create(gateway);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            useCase.Run(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));
    }

    [Fact]
    public async Task AprovarOrdemServico_DeveCallUpdateNoDataSource()
    {
        var id = Guid.NewGuid();
        var mockOS = CriarMockDataSource(id, "AguardandoAprovacao");
        var mockEstoque = new Mock<IEstoqueDataSource>();
        mockEstoque.Setup(m => m.UpdateEstoques(It.IsAny<ICollection<EstoqueInputDTO>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        mockEstoque.Setup(m => m.GetByPecasIds(It.IsAny<ICollection<Guid>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<List<EstoqueInputDTO>?>(new List<EstoqueInputDTO>()));
        mockEstoque.Setup(m => m.GetByInsumosIds(It.IsAny<ICollection<Guid>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<List<EstoqueInputDTO>?>(new List<EstoqueInputDTO>()));

        var osGateway = OrdemServicoGateway.Create(mockOS.Object);
        var pecaGateway = Application.Gateways.Pecas.PecaGateway.Create(new Mock<IPecaDataSource>().Object);
        var insumoGateway = Application.Gateways.Insumos.InsumoGateway.Create(new Mock<IInsumoDataSource>().Object);
        var estoqueGateway = Application.Gateways.Estoques.EstoqueGateway.Create(mockEstoque.Object);
        var useCase = AprovarOrdemServicoUseCase.Create(osGateway, pecaGateway, insumoGateway, estoqueGateway);

        await useCase.Run(id, Guid.NewGuid(), CancellationToken.None);

        mockOS.Verify(m => m.Update(It.IsAny<OrdemServicoInputDTO>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AprovarOrdemServico_OrdemNaoEncontrada_DeveThrowKeyNotFoundException()
    {
        var mock = new Mock<IOrdemServicoDataSource>();
        mock.Setup(m => m.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<OrdemServicoInputDTO?>(null));
        var osGateway = OrdemServicoGateway.Create(mock.Object);
        var pecaGateway = Application.Gateways.Pecas.PecaGateway.Create(new Mock<IPecaDataSource>().Object);
        var insumoGateway = Application.Gateways.Insumos.InsumoGateway.Create(new Mock<IInsumoDataSource>().Object);
        var estoqueGateway = Application.Gateways.Estoques.EstoqueGateway.Create(new Mock<IEstoqueDataSource>().Object);
        var useCase = AprovarOrdemServicoUseCase.Create(osGateway, pecaGateway, insumoGateway, estoqueGateway);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            useCase.Run(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));
    }

    [Fact]
    public async Task FinalizarServico_DeveCallUpdateNoDataSource()
    {
        var id = Guid.NewGuid();
        var servicoId = Guid.NewGuid();
        var mockOS = CriarMockDataSource(id, "EmExecucao");
        var mockServico = new Mock<IServicoDataSource>();
        mockServico.Setup(m => m.GetByIds(It.IsAny<ICollection<Guid>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<ICollection<ServicoInputDTO>>(new List<ServicoInputDTO>()));
        mockServico.Setup(m => m.UpdateServicos(It.IsAny<ICollection<ServicoInputDTO>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var osGateway = OrdemServicoGateway.Create(mockOS.Object);
        var servicoGateway = ServicoGateway.Create(mockServico.Object);
        var useCase = FinalizarServicoUseCase.Create(osGateway, servicoGateway);
        var request = new FinalizarServicoRequestDTO { servicosId = new List<Guid>() };

        await useCase.Run(request, id, Guid.NewGuid(), CancellationToken.None);

        mockOS.Verify(m => m.Update(It.IsAny<OrdemServicoInputDTO>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FinalizarServico_OrdemNaoEncontrada_DeveThrowKeyNotFoundException()
    {
        var mock = new Mock<IOrdemServicoDataSource>();
        mock.Setup(m => m.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<OrdemServicoInputDTO?>(null));
        var osGateway = OrdemServicoGateway.Create(mock.Object);
        var servicoGateway = ServicoGateway.Create(new Mock<IServicoDataSource>().Object);
        var useCase = FinalizarServicoUseCase.Create(osGateway, servicoGateway);
        var request = new FinalizarServicoRequestDTO { servicosId = new List<Guid>() };

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            useCase.Run(request, Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));
    }
}
