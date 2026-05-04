using Application.UnitOfWork;
using Application.Veiculos.DTOs.Requests;
using Application.Veiculos.DTOs.Response;
using Application.Veiculos.Services;
using Domain.Aggregates.ClienteAggregates;
using Domain.Aggregates.ClienteAggregates.Repositories;
using Domain.Entities;
using Domain.Entities.Repositories;
using Domain.ValueObjects;
using System.Net;

namespace Application.test.Veiculos;

public class VeiculoServiceTests
{
    private readonly Mock<IVeiculoRepository> _veiculoRepositoryMock;
    private readonly Mock<IClienteRepository> _clienteRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitofWork;
    private readonly VeiculoService _veiculoService;

    public VeiculoServiceTests()
    {
        _veiculoRepositoryMock = new Mock<IVeiculoRepository>();
        _clienteRepositoryMock = new Mock<IClienteRepository>();
        _unitofWork = new Mock<IUnitOfWork>();
        _veiculoService = new VeiculoService(_veiculoRepositoryMock.Object, _clienteRepositoryMock.Object, _unitofWork.Object);
    }

    [Fact]
    public async Task Create_ValidVeiculo_ReturnsCreatedResult()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        var cliente = new Cliente("João Silva", new Cpf("11144477735"), Guid.NewGuid());

        var request = new VeiculoRequestDTO
        {
            Modelo = "Civic",
            MarcaVeiculo = "Honda",
            ClienteId = clienteId,
            Ano = 2023,
            Placa = "ABC1234",
            Cor = "Branco"
        };

        _clienteRepositoryMock.Setup(x => x.GetById(clienteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cliente);

        _veiculoRepositoryMock.Setup(x => x.Add(It.IsAny<Veiculo>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _veiculoService.Create(request, Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(HttpStatusCode.Created, result.StatusCode);
        Assert.NotEqual(Guid.Empty, result.Data);
        _veiculoRepositoryMock.Verify(x => x.Add(It.IsAny<Veiculo>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Create_ClienteNotFound_ReturnsNotFound()
    {
        // Arrange
        var clienteId = Guid.NewGuid();

        var request = new VeiculoRequestDTO
        {
            Modelo = "Civic",
            MarcaVeiculo = "Honda",
            ClienteId = clienteId,
            Ano = 2023,
            Placa = "ABC1234",
            Cor = "Branco"
        };

        _clienteRepositoryMock.Setup(x => x.GetById(clienteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cliente)null);

        // Act
        var result = await _veiculoService.Create(request, Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        Assert.Contains("Cliente não encontrado", result.Message);
    }

    [Fact]
    public async Task Create_InvalidPlaca_ReturnsBadRequest()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        var cliente = new Cliente("João Silva", new Cpf("11144477735"), Guid.NewGuid());

        var request = new VeiculoRequestDTO
        {
            Modelo = "Civic",
            MarcaVeiculo = "Honda",
            ClienteId = clienteId,
            Ano = 2023,
            Placa = "INVALIDA",
            Cor = "Branco"
        };

        _clienteRepositoryMock.Setup(x => x.GetById(clienteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cliente);

        // Act
        var result = await _veiculoService.Create(request, Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    //[Fact]
    //public async Task Delete_ValidVeiculoId_ReturnsNoContent()
    //{
    //    // Arrange
    //    var veiculoId = Guid.NewGuid();
    //    var clienteId = Guid.NewGuid();
    //    var cliente = new Cliente("João Silva", new Cpf("11144477735"), Guid.NewGuid());
    //    var veiculo = new Veiculo("Civic", "Honda", clienteId, 2023, new Placa("ABC1234"), "Branco", Guid.NewGuid());

    //    _veiculoRepositoryMock.Setup(x => x.GetById(veiculoId, It.IsAny<CancellationToken>()))
    //        .ReturnsAsync(veiculo);

    //    _unitofWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
    //        .Returns(Task.FromResult(1));

    //    // Act
    //    var result = await _veiculoService.Delete(veiculoId, Guid.NewGuid(), CancellationToken.None);

    //    // Assert
    //    Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
    //    _veiculoRepositoryMock.Verify(x => x.Update(It.IsAny<Veiculo>(), It.IsAny<CancellationToken>()), Times.Once);
    //}

    [Fact]
    public async Task Delete_VeiculoNotFound_ReturnsNotFound()
    {
        // Arrange
        var veiculoId = Guid.NewGuid();

        _veiculoRepositoryMock.Setup(x => x.GetById(veiculoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Veiculo)null);

        // Act
        var result = await _veiculoService.Delete(veiculoId, Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task GetById_ValidVeiculoId_ReturnsVeiculoResponseDTO()
    {
        // Arrange
        var veiculoId = Guid.NewGuid();
        var clienteId = Guid.NewGuid();
        var veiculo = new Veiculo("Civic", "Honda", clienteId, 2023, new Placa("ABC1234"), "Branco", Guid.NewGuid());

        _veiculoRepositoryMock.Setup(x => x.GetById(veiculoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(veiculo);

        // Act
        var result = await _veiculoService.GetById(veiculoId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetById_VeiculoNotFound_ReturnsNotFound()
    {
        // Arrange
        var veiculoId = Guid.NewGuid();

        _veiculoRepositoryMock.Setup(x => x.GetById(veiculoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Veiculo)null);

        // Act
        var result = await _veiculoService.GetById(veiculoId, CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }
}
