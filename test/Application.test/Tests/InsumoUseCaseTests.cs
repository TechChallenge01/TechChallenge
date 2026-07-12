using Application.Gateways.Estoques;
using Application.Gateways.Insumos;
using Application.Interfaces;
using Application.UseCases.Insumos;
using Moq;
using Shared.DTOs.Insumos.Input;
using Shared.DTOs.Insumos.Request;

namespace Application.test.Tests;

public class InsumoUseCaseTests
{
    private static Mock<IInsumoDataSource> CriarMockDataSource(Guid? idRetorno = null)
    {
        var id = idRetorno ?? Guid.NewGuid();
        var mock = new Mock<IInsumoDataSource>();

        mock.Setup(m => m.Create(It.IsAny<InsumoInputDTO>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mock.Setup(m => m.Update(It.IsAny<InsumoInputDTO>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mock.Setup(m => m.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new InsumoInputDTO
            {
                Id = id,
                Nome = "Óleo 5W30",
                Descricao = "Óleo sintético",
                CustoUnitario = 75.00m
            });

        mock.Setup(m => m.GetPaginated(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<InsumoInputDTO>
            {
                new InsumoInputDTO { Id = id, Nome = "Óleo 5W30", Descricao = "Óleo sintético", CustoUnitario = 75.00m }
            }, 1));

        return mock;
    }

    [Fact]
    public async Task CriarInsumo_DeveRetornarGuidValido()
    {
        var mock = CriarMockDataSource();
        var gateway = InsumoGateway.Create(mock.Object);
        var useCase = CreateUseCase.Create(gateway);

        var request = new InsumoRequestDTO { Nome = "Óleo 5W30", Descricao = "Óleo sintético", CustoUnitario = 75.00m };

        var id = await useCase.Run(request, Guid.NewGuid(), CancellationToken.None);

        Assert.NotEqual(Guid.Empty, id);
    }

    [Fact]
    public async Task CriarInsumo_DeveCallCreateNoDataSource()
    {
        var mock = CriarMockDataSource();
        var gateway = InsumoGateway.Create(mock.Object);
        var useCase = CreateUseCase.Create(gateway);

        var request = new InsumoRequestDTO { Nome = "Óleo 5W30", Descricao = "Óleo sintético", CustoUnitario = 75.00m };

        await useCase.Run(request, Guid.NewGuid(), CancellationToken.None);

        mock.Verify(m => m.Create(It.IsAny<InsumoInputDTO>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AtualizarInsumo_ComDadosValidos_DeveCallUpdateNoDataSource()
    {
        var id = Guid.NewGuid();
        var mock = CriarMockDataSource(id);
        var gateway = InsumoGateway.Create(mock.Object);
        var useCase = UpdateUseCase.Create(gateway);

        var request = new InsumoRequestDTO { Nome = "Filtro de Ar", Descricao = "Filtro premium", CustoUnitario = 50.00m };

        await useCase.Run(Guid.NewGuid(), id, request, CancellationToken.None);

        mock.Verify(m => m.Update(It.IsAny<InsumoInputDTO>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AtualizarInsumo_InsumoNaoEncontrado_DeveThrowKeyNotFoundException()
    {
        var mock = new Mock<IInsumoDataSource>();
        mock.Setup(m => m.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((InsumoInputDTO)null);

        var gateway = InsumoGateway.Create(mock.Object);
        var useCase = UpdateUseCase.Create(gateway);

        var request = new InsumoRequestDTO { Nome = "X", Descricao = "Y", CustoUnitario = 10m };

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            useCase.Run(Guid.NewGuid(), Guid.NewGuid(), request, CancellationToken.None));
    }

    [Fact]
    public async Task DeletarInsumo_DeveInativarECallUpdate()
    {
        var id = Guid.NewGuid();
        var mock = CriarMockDataSource(id);
        var gateway = InsumoGateway.Create(mock.Object);
        var mockEstoque = new Mock<IEstoqueDataSource>();
        var estoqueGateway = EstoqueGateway.Create(mockEstoque.Object);
        var useCase = DeleteUseCase.Create(gateway, estoqueGateway);

        await useCase.Run(Guid.NewGuid(), id, CancellationToken.None);

        mock.Verify(m => m.Update(
            It.Is<InsumoInputDTO>(dto => dto.Ativo == false),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeletarInsumo_InsumoNaoEncontrado_DeveThrowKeyNotFoundException()
    {
        var mock = new Mock<IInsumoDataSource>();
        mock.Setup(m => m.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((InsumoInputDTO?)null);

        var gateway = InsumoGateway.Create(mock.Object);
        var mockEstoque = new Mock<IEstoqueDataSource>();
        var estoqueGateway = EstoqueGateway.Create(mockEstoque.Object);
        var useCase = DeleteUseCase.Create(gateway, estoqueGateway);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            useCase.Run(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));
    }

    [Fact]
    public async Task ObterInsumoPorId_DeveRetornarInsumoCorreto()
    {
        var id = Guid.NewGuid();
        var mock = CriarMockDataSource(id);
        var gateway = InsumoGateway.Create(mock.Object);
        var useCase = GetByIdUseCase.Create(gateway);

        var insumo = await useCase.Run(id, CancellationToken.None);

        Assert.NotNull(insumo);
        Assert.Equal(id, insumo.Id);
        Assert.Equal("Óleo 5W30", insumo.Nome);
    }

    [Fact]
    public async Task ObterInsumoPorId_InsumoNaoEncontrado_DeveRetornarNull()
    {
        var mock = new Mock<IInsumoDataSource>();
        mock.Setup(m => m.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((InsumoInputDTO)null);

        var gateway = InsumoGateway.Create(mock.Object);
        var useCase = GetByIdUseCase.Create(gateway);

        var insumo = await useCase.Run(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(insumo);
    }

    [Fact]
    public async Task ObterInsumosPaginados_DeveRetornarListaETotal()
    {
        var mock = CriarMockDataSource();
        var gateway = InsumoGateway.Create(mock.Object);
        var useCase = GetPaginatedUseCase.Create(gateway);

        var (insumos, total) = await useCase.Run(1, 10, CancellationToken.None);

        Assert.NotEmpty(insumos);
        Assert.Equal(1, total);
    }
}
