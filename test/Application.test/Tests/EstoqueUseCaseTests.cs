using Application.Gateways.Estoques;
using Application.Interfaces;
using Application.UseCases.Estoques;
using Moq;
using Shared.DTOs.Estoques.Input;
using Shared.DTOs.Estoques.Request;

namespace Application.test.Tests;

public class EstoqueUseCaseTests
{
    private static EstoqueInputDTO CriarEstoqueInputDTO(Guid? id = null, Guid? insumoId = null, Guid? pecaId = null) => new EstoqueInputDTO
    {
        Id = id ?? Guid.NewGuid(),
        InsumoId = insumoId,
        PecaId = pecaId,
        QuantidadeDisponivel = 10,
        QuantidadeReservada = 0,
        IdUsuarioCriacao = Guid.NewGuid(),
        DataCriacao = DateTime.UtcNow,
        Ativo = true,
        Historicos = new List<EstoqueHistoricoInputDTO>()
    };

    private static Mock<IEstoqueDataSource> CriarMockDataSource(Guid? idRetorno = null, Guid? insumoId = null, Guid? pecaId = null)
    {
        var id = idRetorno ?? Guid.NewGuid();
        var iId = insumoId ?? Guid.NewGuid();
        var mock = new Mock<IEstoqueDataSource>();

        mock.Setup(m => m.Update(It.IsAny<EstoqueInputDTO>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        mock.Setup(m => m.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarEstoqueInputDTO(id, insumoId, pecaId));
        mock.Setup(m => m.GetByInsumoId(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarEstoqueInputDTO(id, iId, null));
        mock.Setup(m => m.GetByPecaId(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarEstoqueInputDTO(id, null, pecaId ?? Guid.NewGuid()));
        mock.Setup(m => m.GetPaginated(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<EstoqueInputDTO> { CriarEstoqueInputDTO(id) }, 1));

        return mock;
    }

    [Fact]
    public async Task MovimentarEstoque_Entrada_DeveRetornarGuidValido()
    {
        var insumoId = Guid.NewGuid();
        var mock = CriarMockDataSource(insumoId: insumoId);
        var gateway = EstoqueGateway.Create(mock.Object);
        var useCase = MovimentarUseCase.Create(gateway);
        var request = new EstoqueRequestDTO { InsumoId = insumoId, TipoMovimentacao = "Entrada", Quantidade = 5 };

        var id = await useCase.Run(request, Guid.NewGuid(), CancellationToken.None);

        Assert.NotEqual(Guid.Empty, id);
    }

    [Fact]
    public async Task MovimentarEstoque_SemInsumoEPeca_DeveThrowArgumentException()
    {
        var mock = CriarMockDataSource();
        var gateway = EstoqueGateway.Create(mock.Object);
        var useCase = MovimentarUseCase.Create(gateway);
        var request = new EstoqueRequestDTO { TipoMovimentacao = "Entrada", Quantidade = 5 };

        await Assert.ThrowsAsync<ArgumentException>(() =>
            useCase.Run(request, Guid.NewGuid(), CancellationToken.None));
    }

    [Fact]
    public async Task MovimentarEstoque_ComInsumoEPeca_DeveThrowArgumentException()
    {
        var mock = CriarMockDataSource();
        var gateway = EstoqueGateway.Create(mock.Object);
        var useCase = MovimentarUseCase.Create(gateway);
        var request = new EstoqueRequestDTO { InsumoId = Guid.NewGuid(), PecaId = Guid.NewGuid(), TipoMovimentacao = "Entrada", Quantidade = 5 };

        await Assert.ThrowsAsync<ArgumentException>(() =>
            useCase.Run(request, Guid.NewGuid(), CancellationToken.None));
    }

    [Fact]
    public async Task MovimentarEstoque_TipoInvalido_DeveThrowArgumentException()
    {
        var mock = CriarMockDataSource();
        var gateway = EstoqueGateway.Create(mock.Object);
        var useCase = MovimentarUseCase.Create(gateway);
        var request = new EstoqueRequestDTO { InsumoId = Guid.NewGuid(), TipoMovimentacao = "Invalido", Quantidade = 5 };

        await Assert.ThrowsAsync<ArgumentException>(() =>
            useCase.Run(request, Guid.NewGuid(), CancellationToken.None));
    }

    [Fact]
    public async Task MovimentarEstoque_EstoqueNaoEncontrado_DeveThrowKeyNotFoundException()
    {
        var mock = new Mock<IEstoqueDataSource>();
        mock.Setup(m => m.GetByInsumoId(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((EstoqueInputDTO)null);
        var gateway = EstoqueGateway.Create(mock.Object);
        var useCase = MovimentarUseCase.Create(gateway);
        var request = new EstoqueRequestDTO { InsumoId = Guid.NewGuid(), TipoMovimentacao = "Entrada", Quantidade = 5 };

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            useCase.Run(request, Guid.NewGuid(), CancellationToken.None));
    }

    [Fact]
    public async Task ObterEstoquePorId_DeveRetornarEstoqueCorreto()
    {
        var id = Guid.NewGuid();
        var mock = CriarMockDataSource(id);
        var gateway = EstoqueGateway.Create(mock.Object);
        var useCase = GetByIdUseCase.Create(gateway);

        var estoque = await useCase.Run(id, CancellationToken.None);

        Assert.NotNull(estoque);
        Assert.Equal(id, estoque.Id);
    }

    [Fact]
    public async Task ObterEstoquePorId_NaoEncontrado_DeveRetornarNull()
    {
        var mock = new Mock<IEstoqueDataSource>();
        mock.Setup(m => m.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((EstoqueInputDTO)null);
        var gateway = EstoqueGateway.Create(mock.Object);
        var useCase = GetByIdUseCase.Create(gateway);

        var estoque = await useCase.Run(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(estoque);
    }

    [Fact]
    public async Task ObterEstoquesPaginados_DeveRetornarListaETotal()
    {
        var mock = CriarMockDataSource();
        var gateway = EstoqueGateway.Create(mock.Object);
        var useCase = GetPaginatedUseCase.Create(gateway);

        var (estoques, total) = await useCase.Run(1, 10, CancellationToken.None);

        Assert.NotEmpty(estoques);
        Assert.Equal(1, total);
    }
}
