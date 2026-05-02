using Application.EmailServices;
using Application.OrdemServicos.DTOs.Requests;
using Application.OrdemServicos.Services;
using Application.UnitOfWork;
using Domain.Aggregates.ClienteAggregates;
using Domain.Aggregates.ClienteAggregates.Repositories;
using Domain.Aggregates.EstoqueAggregates;
using Domain.Aggregates.EstoqueAggregates.Repositories;
using Domain.Aggregates.OrdemServicoAggregates;
using Domain.Aggregates.OrdemServicoAggregates.Repositories;
using Domain.Entities;
using Domain.Entities.Repositories;
using Domain.ValueObjects;
using System.Net;

namespace Application.test.OrdemServicos;

public class OrdemServicoServiceTests
{
    private readonly Mock<IOrdemServicoRepository> _ordemServicoRepositoryMock;
    private readonly Mock<IClienteRepository> _clienteRepositoryMock;
    private readonly Mock<IPecaRepository> _pecaRepositoryMock;
    private readonly Mock<IServicoRepository> _servicoRepositoryMock;
    private readonly Mock<IInsumoRepository> _insumoRepositoryMock;
    private readonly Mock<IEstoqueRepository> _estoqueRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly OrdemServicoService _ordemServicoService;

    public OrdemServicoServiceTests()
    {
        _ordemServicoRepositoryMock = new Mock<IOrdemServicoRepository>();
        _clienteRepositoryMock = new Mock<IClienteRepository>();
        _pecaRepositoryMock = new Mock<IPecaRepository>();
        _servicoRepositoryMock = new Mock<IServicoRepository>();
        _insumoRepositoryMock = new Mock<IInsumoRepository>();
        _estoqueRepositoryMock = new Mock<IEstoqueRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _emailServiceMock = new Mock<IEmailService>();

        _ordemServicoService = new OrdemServicoService(
            _ordemServicoRepositoryMock.Object,
            _clienteRepositoryMock.Object,
            _pecaRepositoryMock.Object,
            _servicoRepositoryMock.Object,
            _estoqueRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _emailServiceMock.Object,
            _insumoRepositoryMock.Object);
    }

    [Fact]
    public async Task Aprovar_ValidOrdemServico_ReturnsNoContent()
    {
        // Arrange
        var ordemServicoId = Guid.NewGuid();
        var clienteId = Guid.NewGuid();
        var veiculoId = Guid.NewGuid();

        var ordemServico = new OrdemServico(clienteId, veiculoId, Guid.NewGuid());
        ordemServico.IniciarDiagnostico();
        ordemServico.RegistrarDiagnostico("Diagnóstico realizado");

        _ordemServicoRepositoryMock.Setup(x => x.GetById(ordemServicoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordemServico);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _ordemServicoService.Aprovar(ordemServicoId, Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Aprovar_OrdemServicoNotFound_ReturnsNotFound()
    {
        // Arrange
        var ordemServicoId = Guid.NewGuid();

        _ordemServicoRepositoryMock.Setup(x => x.GetById(ordemServicoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrdemServico)null);

        // Act
        var result = await _ordemServicoService.Aprovar(ordemServicoId, Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task Cancelar_ValidOrdemServico_ReturnsNoContent()
    {
        // Arrange
        var ordemServicoId = Guid.NewGuid();
        var clienteId = Guid.NewGuid();
        var veiculoId = Guid.NewGuid();

        var ordemServico = new OrdemServico(clienteId, veiculoId, Guid.NewGuid());
        ordemServico.IniciarDiagnostico();
        ordemServico.RegistrarDiagnostico("Diagnóstico realizado");

        _ordemServicoRepositoryMock.Setup(x => x.GetById(ordemServicoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordemServico);

        _estoqueRepositoryMock.Setup(x => x.GetByPecaIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Estoque>());

        _estoqueRepositoryMock.Setup(x => x.GetByInsumoIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Estoque>());

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _ordemServicoService.Cancelar(ordemServicoId, Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
    }

    [Fact]
    public async Task Cancelar_OrdemServicoNotFound_ReturnsNotFound()
    {
        // Arrange
        var ordemServicoId = Guid.NewGuid();

        _ordemServicoRepositoryMock.Setup(x => x.GetById(ordemServicoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrdemServico)null);

        // Act
        var result = await _ordemServicoService.Cancelar(ordemServicoId, Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task IniciarDiagnostico_ValidOrdemServico_ReturnsNoContent()
    {
        // Arrange
        var ordemServicoId = Guid.NewGuid();
        var clienteId = Guid.NewGuid();
        var veiculoId = Guid.NewGuid();

        var ordemServico = new OrdemServico(clienteId, veiculoId, Guid.NewGuid());

        _ordemServicoRepositoryMock.Setup(x => x.GetById(ordemServicoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordemServico);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _ordemServicoService.IniciarDiagnostico(ordemServicoId, Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
    }

    [Fact]
    public async Task IniciarDiagnostico_OrdemServicoNotFound_ReturnsNotFound()
    {
        // Arrange
        var ordemServicoId = Guid.NewGuid();

        _ordemServicoRepositoryMock.Setup(x => x.GetById(ordemServicoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrdemServico)null);

        // Act
        var result = await _ordemServicoService.IniciarDiagnostico(ordemServicoId, Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task GetById_ValidOrdemServicoId_ReturnsOrdemServicoResponseDTO()
    {
        // Arrange
        var ordemServicoId = Guid.NewGuid();
        var clienteId = Guid.NewGuid();
        var veiculoId = Guid.NewGuid();

        var ordemServico = new OrdemServico(clienteId, veiculoId, Guid.NewGuid());

        _ordemServicoRepositoryMock.Setup(x => x.GetById(ordemServicoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordemServico);

        // Act
        var result = await _ordemServicoService.GetById(ordemServicoId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetById_OrdemServicoNotFound_ReturnsNotFound()
    {
        // Arrange
        var ordemServicoId = Guid.NewGuid();

        _ordemServicoRepositoryMock.Setup(x => x.GetById(ordemServicoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrdemServico)null);

        // Act
        var result = await _ordemServicoService.GetById(ordemServicoId, CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }
}
