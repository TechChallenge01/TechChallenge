using Application.Insumos.DTOs.Requests;
using Application.Insumos.Services;
using Domain.Aggregates.EstoqueAggregates;
using Domain.Aggregates.EstoqueAggregates.Repositories;
using Domain.Entities.Repositories;
using Moq;
using System.Net;
using Application.UnitOfWork;

namespace Application.test.Tests
{
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
        public async Task Create_ComDadosValidos_DeveRetornarCreated()
        {
            // Arrange
            var request = new InsumoRequestDTO
            {
                Nome = "Óleo",
                Descricao = "Óleo sintético",
                CustoUnitario = 75.00m
            };
            var usuarioId = Guid.NewGuid();

            _insumoRepositoryMock.Setup(x => x.Create(It.IsAny<Insumo>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _estoqueRepositoryMock.Setup(x => x.Create(It.IsAny<Estoque>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            var result = await _insumoService.Create(request, usuarioId, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task Delete_ComInsumoValido_DeveRetornarNoContent()
        {
            // Arrange
            var insumoId = Guid.NewGuid();
            var usuarioId = Guid.NewGuid();
            var insumo = new Insumo("Óleo", "Óleo sintético", 75.00m, usuarioId, DateTime.UtcNow);
            var estoque = new Estoque(insumoId, null, 0, usuarioId, DateTime.UtcNow);

            _insumoRepositoryMock.Setup(x => x.GetById(insumoId, It.IsAny<CancellationToken>())).ReturnsAsync(insumo);
            _estoqueRepositoryMock.Setup(x => x.GetByInsumoId(insumoId, It.IsAny<CancellationToken>())).ReturnsAsync(estoque);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _insumoService.Delete(insumoId, usuarioId, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
        }

        [Fact]
        public async Task GetById_ComInsumoValido_DeveRetornarOk()
        {
            // Arrange
            var insumoId = Guid.NewGuid();
            var usuarioId = Guid.NewGuid();
            var insumo = new Insumo("Óleo", "Óleo sintético", 75.00m, usuarioId, DateTime.UtcNow);

            _insumoRepositoryMock.Setup(x => x.GetById(insumoId, It.IsAny<CancellationToken>())).ReturnsAsync(insumo);

            // Act
            var result = await _insumoService.GetById(insumoId, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.NotNull(result.Data);
        }
    }
}
