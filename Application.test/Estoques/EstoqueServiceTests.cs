using Application.Estoques.DTOs.Requests;
using Application.Estoques.Services;
using Application.UnitOfWork;
using Domain.Aggregates.EstoqueAggregates;
using Domain.Aggregates.EstoqueAggregates.Repositories;
using Domain.Entities;
using Domain.Entities.Repositories;
using System.Net;

namespace Application.test.Estoques;

public class EstoqueServiceTests
{
    private readonly Mock<IEstoqueRepository> _estoqueRepositoryMock;
    private readonly Mock<IPecaRepository> _pecaRepositoryMock;
    private readonly Mock<IInsumoRepository> _insumoRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly EstoqueService _estoqueService;

    public EstoqueServiceTests()
    {
        _estoqueRepositoryMock = new Mock<IEstoqueRepository>();
        _pecaRepositoryMock = new Mock<IPecaRepository>();
        _insumoRepositoryMock = new Mock<IInsumoRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _estoqueService = new EstoqueService(_estoqueRepositoryMock.Object, _pecaRepositoryMock.Object, _unitOfWorkMock.Object, _insumoRepositoryMock.Object);
    }

    [Fact]
    public async Task GetPaginated_ReturnsPagedResult()
    {
        // Arrange
        int page = 1;
        int pageSize = 10;
        var estoques = new List<Estoque>
        {
            new Estoque(Guid.NewGuid(), null, 100, Guid.NewGuid(), DateTime.UtcNow),
            new Estoque(Guid.NewGuid(), null, 50, Guid.NewGuid(), DateTime.UtcNow)
        };

        _estoqueRepositoryMock.Setup(x => x.GetPaginated(page, pageSize, It.IsAny<CancellationToken>()))
            .ReturnsAsync((estoques, 2));

        // Act
        var result = await _estoqueService.GetPaginated(page, pageSize, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.TotalItems);
    }

    [Fact]
    public async Task GetById_ValidEstoqueId_ReturnsEstoqueResponseDTO()
    {
        // Arrange
        var estoqueId = Guid.NewGuid();
        var estoque = new Estoque(Guid.NewGuid(), null, 100, Guid.NewGuid(), DateTime.UtcNow);

        _estoqueRepositoryMock.Setup(x => x.GetById(estoqueId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(estoque);

        // Act
        var result = await _estoqueService.GetById(estoqueId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetById_EstoqueNotFound_ReturnsNotFound()
    {
        // Arrange
        var estoqueId = Guid.NewGuid();

        _estoqueRepositoryMock.Setup(x => x.GetById(estoqueId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Estoque)null);

        // Act
        var result = await _estoqueService.GetById(estoqueId, CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task Movimetar_WithoutPecaIdAndInsumoId_ReturnsBadRequest()
    {
        // Arrange
        var request = new EstoqueRequestDTO
        {
            PecaId = null,
            InsumoId = null,
            TipoMovimentacao = "Entrada",
            Quantidade = 10
        };

        // Act
        var result = await _estoqueService.Movimetar(request, Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.Contains("PecaId ou o InsumoId", result.Message);
    }

    [Fact]
    public async Task Movimetar_WithBothPecaIdAndInsumoId_ReturnsBadRequest()
    {
        // Arrange
        var pecaId = Guid.NewGuid();
        var insumoId = Guid.NewGuid();
        var request = new EstoqueRequestDTO
        {
            PecaId = pecaId,
            InsumoId = insumoId,
            TipoMovimentacao = "Entrada",
            Quantidade = 10
        };

        // Act
        var result = await _estoqueService.Movimetar(request, Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.Contains("apenas uma opção", result.Message.ToLower());
    }

    [Fact]
    public async Task Movimetar_WithInvalidTipoMovimentacao_ReturnsBadRequest()
    {
        // Arrange
        var pecaId = Guid.NewGuid();
        var request = new EstoqueRequestDTO
        {
            PecaId = pecaId,
            InsumoId = null,
            TipoMovimentacao = "Invalido",
            Quantidade = 10
        };

        // Act
        var result = await _estoqueService.Movimetar(request, Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.Contains("Tipo de movimentação", result.Message);
    }
}
