using Application.Gateways.Pecas;
using Application.Interfaces;
using Application.UseCases.Pecas;
using Moq;
using Shared.DTOs.Pecas.Input;
using Shared.DTOs.Pecas.Request;

namespace Application.test.Tests;

public class PecaUseCaseTests
{
    private static Mock<IPecaDataSource> CriarMockDataSource(Guid? idRetorno = null)
    {
        var id = idRetorno ?? Guid.NewGuid();
        var mock = new Mock<IPecaDataSource>();

        mock.Setup(m => m.Create(It.IsAny<PecaInputDTO>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        mock.Setup(m => m.Update(It.IsAny<PecaInputDTO>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        mock.Setup(m => m.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PecaInputDTO { Id = id, Nome = "Filtro de Ar", Descricao = "Filtro premium", MarcaPeca = "Bosch", ValorUnitario = 85.50m });
        mock.Setup(m => m.GetPaginated(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<PecaInputDTO> { new PecaInputDTO { Id = id, Nome = "Filtro de Ar", Descricao = "Filtro premium", MarcaPeca = "Bosch", ValorUnitario = 85.50m } }, 1));

        return mock;
    }

    [Fact]
    public async Task CriarPeca_DeveRetornarGuidValido()
    {
        var mock = CriarMockDataSource();
        var gateway = PecaGateway.Create(mock.Object);
        var useCase = CreateUseCase.Create(gateway);
        var request = new PecaRequestDTO { Nome = "Filtro de Ar", Descricao = "Filtro premium", MarcaPeca = "Bosch", PrecoVenda = 85.50m };

        var id = await useCase.Run(request, Guid.NewGuid(), CancellationToken.None);

        Assert.NotEqual(Guid.Empty, id);
    }

    [Fact]
    public async Task CriarPeca_DeveCallCreateNoDataSource()
    {
        var mock = CriarMockDataSource();
        var gateway = PecaGateway.Create(mock.Object);
        var useCase = CreateUseCase.Create(gateway);
        var request = new PecaRequestDTO { Nome = "Filtro de Ar", Descricao = "Filtro premium", MarcaPeca = "Bosch", PrecoVenda = 85.50m };

        await useCase.Run(request, Guid.NewGuid(), CancellationToken.None);

        mock.Verify(m => m.Create(It.IsAny<PecaInputDTO>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AtualizarPeca_ComDadosValidos_DeveCallUpdateNoDataSource()
    {
        var id = Guid.NewGuid();
        var mock = CriarMockDataSource(id);
        var gateway = PecaGateway.Create(mock.Object);
        var useCase = UpdateUseCase.Create(gateway);
        var request = new PecaRequestDTO { Nome = "Filtro de Óleo", Descricao = "Filtro sintético", MarcaPeca = "Mann", PrecoVenda = 95.00m };

        await useCase.Run(Guid.NewGuid(), id, request, CancellationToken.None);

        mock.Verify(m => m.Update(It.IsAny<PecaInputDTO>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AtualizarPeca_PecaNaoEncontrada_DeveThrowKeyNotFoundException()
    {
        var mock = new Mock<IPecaDataSource>();
        mock.Setup(m => m.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PecaInputDTO)null);
        var gateway = PecaGateway.Create(mock.Object);
        var useCase = UpdateUseCase.Create(gateway);
        var request = new PecaRequestDTO { Nome = "X", Descricao = "Y", MarcaPeca = "Z", PrecoVenda = 10m };

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            useCase.Run(Guid.NewGuid(), Guid.NewGuid(), request, CancellationToken.None));
    }

    [Fact]
    public async Task DeletarPeca_DeveInativarECallUpdate()
    {
        var id = Guid.NewGuid();
        var mock = CriarMockDataSource(id);
        var gateway = PecaGateway.Create(mock.Object);
        var useCase = DeleteUseCase.Create(gateway);

        await useCase.Run(Guid.NewGuid(), id, CancellationToken.None);

        mock.Verify(m => m.Update(It.Is<PecaInputDTO>(dto => dto.Ativo == false), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeletarPeca_PecaNaoEncontrada_DeveThrowKeyNotFoundException()
    {
        var mock = new Mock<IPecaDataSource>();
        mock.Setup(m => m.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PecaInputDTO)null);
        var gateway = PecaGateway.Create(mock.Object);
        var useCase = DeleteUseCase.Create(gateway);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            useCase.Run(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));
    }

    [Fact]
    public async Task ObterPecaPorId_DeveRetornarPecaCorreta()
    {
        var id = Guid.NewGuid();
        var mock = CriarMockDataSource(id);
        var gateway = PecaGateway.Create(mock.Object);
        var useCase = GetByIdUseCase.Create(gateway);

        var peca = await useCase.Run(id, CancellationToken.None);

        Assert.NotNull(peca);
        Assert.Equal(id, peca.Id);
        Assert.Equal("Filtro de Ar", peca.Nome);
    }

    [Fact]
    public async Task ObterPecaPorId_PecaNaoEncontrada_DeveRetornarNull()
    {
        var mock = new Mock<IPecaDataSource>();
        mock.Setup(m => m.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PecaInputDTO)null);
        var gateway = PecaGateway.Create(mock.Object);
        var useCase = GetByIdUseCase.Create(gateway);

        var peca = await useCase.Run(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(peca);
    }

    [Fact]
    public async Task ObterPecasPaginadas_DeveRetornarListaETotal()
    {
        var mock = CriarMockDataSource();
        var gateway = PecaGateway.Create(mock.Object);
        var useCase = GetPaginatedUseCase.Create(gateway);

        var (pecas, total) = await useCase.Run(1, 10, CancellationToken.None);

        Assert.NotEmpty(pecas);
        Assert.Equal(1, total);
    }
}
