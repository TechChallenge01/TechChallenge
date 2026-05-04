using Application.Insumos.DTOs.Requests;
using Application.Insumos.DTOs.Responses;
using Application.Insumos.Services;
using Application.UnitOfWork;
using Domain.Aggregates.EstoqueAggregates.Repositories;
using Domain.Entities;
using Domain.Entities.Repositories;
using System.Net;

namespace Application.test.Insumos;

public class InsumoServiceTests
{
    private readonly Mock<IInsumoRepository> _insumoRepositoryMock;
    private readonly Mock<IEstoqueRepository> _estoqueRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly InsumoService _insumoService;

    public InsumoServiceTests()
    {
        _insumoRepositoryMock = new Mock<IInsumoRepository>();
        _estoqueRepositoryMock = new Mock<IEstoqueRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _insumoService = new InsumoService(_insumoRepositoryMock.Object, _unitOfWorkMock.Object, _estoqueRepositoryMock.Object);
    }

    [Fact]
    public async Task Create_ValidInsumo_ReturnsCreatedResult()
    {
        // Arrange
        var request = new InsumoRequestDTO
        {
            Nome = "Óleo Motor",
            Descricao = "Óleo Sintético 5W30",
            CustoUnitario = 150.00m
        };

        Guid? capturedInsumoId = null;
        _insumoRepositoryMock.Setup(x => x.Create(It.IsAny<Insumo>(), It.IsAny<CancellationToken>()))
            .Callback<Insumo, CancellationToken>((insumo, _) => capturedInsumoId = insumo.Id)
            .Returns(Task.CompletedTask);

        _estoqueRepositoryMock.Setup(x => x.Create(It.IsAny<Domain.Aggregates.EstoqueAggregates.Estoque>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _insumoService.Create(request, Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(HttpStatusCode.Created, result.StatusCode);
        Assert.NotEqual(Guid.Empty, result.Data);
        _insumoRepositoryMock.Verify(x => x.Create(It.IsAny<Insumo>(), It.IsAny<CancellationToken>()), Times.Once);
        _estoqueRepositoryMock.Verify(x => x.Create(It.IsAny<Domain.Aggregates.EstoqueAggregates.Estoque>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Create_InsumoWithEmptyNome_ReturnsBadRequest()
    {
        // Arrange
        var request = new InsumoRequestDTO
        {
            Nome = "",
            Descricao = "Óleo Sintético 5W30",
            CustoUnitario = 150.00m
        };

        // Act
        var result = await _insumoService.Create(request, Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task Create_InsumoWithNegativeCusto_ReturnsBadRequest()
    {
        // Arrange
        var request = new InsumoRequestDTO
        {
            Nome = "Óleo Motor",
            Descricao = "Óleo Sintético 5W30",
            CustoUnitario = -50.00m
        };

        // Act
        var result = await _insumoService.Create(request, Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task Create_RepositoryThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var request = new InsumoRequestDTO
        {
            Nome = "Óleo Motor",
            Descricao = "Óleo Sintético 5W30",
            CustoUnitario = 150.00m
        };

        _insumoRepositoryMock.Setup(x => x.Create(It.IsAny<Insumo>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _insumoService.Create(request, Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
    }

    [Fact]
    public async Task Delete_ValidInsumoId_ReturnsNoContent()
    {
        // Arrange
        var insumoId = Guid.NewGuid();
        var insumo = new Insumo("Óleo Motor", "Óleo Sintético 5W30", 150.00m, Guid.NewGuid(), DateTime.UtcNow);

        _insumoRepositoryMock.Setup(x => x.GetById(insumoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(insumo);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(1));

        // Act
        var result = await _insumoService.Delete(insumoId, Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Delete_InsumoNotFound_ReturnsNotFound()
    {
        // Arrange
        var insumoId = Guid.NewGuid();

        _insumoRepositoryMock.Setup(x => x.GetById(insumoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Insumo)null);

        // Act
        var result = await _insumoService.Delete(insumoId, Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task GetById_ValidInsumoId_ReturnsInsumoResponseDTO()
    {
        // Arrange
        var insumoId = Guid.NewGuid();
        var insumo = new Insumo("Óleo Motor", "Óleo Sintético 5W30", 150.00m, Guid.NewGuid(), DateTime.UtcNow);

        _insumoRepositoryMock.Setup(x => x.GetById(insumoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(insumo);

        // Act
        var result = await _insumoService.GetById(insumoId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetById_InsumoNotFound_ReturnsNotFound()
    {
        // Arrange
        var insumoId = Guid.NewGuid();

        _insumoRepositoryMock.Setup(x => x.GetById(insumoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Insumo)null);

        // Act
        var result = await _insumoService.GetById(insumoId, CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }
}
