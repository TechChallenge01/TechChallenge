using Application.Estoques.DTOs.Requests;
using Application.Estoques.Services;
using Domain.Aggregates.EstoqueAggregates;
using Domain.Aggregates.EstoqueAggregates.Repositories;
using Domain.Entities;
using Domain.Entities.Repositories;
using Moq;
using System.Net;
using Application.UnitOfWork;
using Xunit;

namespace Application.test.Tests
{
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
        public async Task GetById_ComEstoqueValido_DeveRetornarOk()
        {
            // Arrange
            var estoqueId = Guid.NewGuid();
            var usuarioId = Guid.NewGuid();
            var estoque = new Estoque(null, Guid.NewGuid(), 10, usuarioId, DateTime.UtcNow);

            _estoqueRepositoryMock.Setup(x => x.GetById(estoqueId, It.IsAny<CancellationToken>())).ReturnsAsync(estoque);

            // Act
            var result = await _estoqueService.GetById(estoqueId, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task Movimetar_ComEntrada_DeveRetornarCreated()
        {
            // Arrange
            var pecaId = Guid.NewGuid();
            var usuarioId = Guid.NewGuid();
            var request = new EstoqueRequestDTO
            {
                PecaId = pecaId,
                TipoMovimentacao = "Entrada",
                Quantidade = 5
            };
            var peca = new Peca("Filtro", "Descrição", "Bosch", 85.50m, usuarioId, DateTime.UtcNow);
            var estoque = new Estoque(null, pecaId, 10, usuarioId, DateTime.UtcNow);

            _pecaRepositoryMock.Setup(x => x.GetById(pecaId, It.IsAny<CancellationToken>())).ReturnsAsync(peca);
            _estoqueRepositoryMock.Setup(x => x.GetByPecaId(pecaId, It.IsAny<CancellationToken>())).ReturnsAsync(estoque);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _estoqueService.Movimetar(request, usuarioId, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
        }

        [Fact]
        public async Task Movimetar_ComSaida_DeveRetornarCreated()
        {
            // Arrange
            var pecaId = Guid.NewGuid();
            var usuarioId = Guid.NewGuid();
            var request = new EstoqueRequestDTO
            {
                PecaId = pecaId,
                TipoMovimentacao = "Saida",
                Quantidade = 3
            };
            var peca = new Peca("Filtro", "Descrição", "Bosch", 85.50m, usuarioId, DateTime.UtcNow);
            var estoque = new Estoque(null, pecaId, 10, usuarioId, DateTime.UtcNow);

            _pecaRepositoryMock.Setup(x => x.GetById(pecaId, It.IsAny<CancellationToken>())).ReturnsAsync(peca);
            _estoqueRepositoryMock.Setup(x => x.GetByPecaId(pecaId, It.IsAny<CancellationToken>())).ReturnsAsync(estoque);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _estoqueService.Movimetar(request, usuarioId, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
        }

        [Fact]
        public async Task GetById_ComEstoqueNaoEncontrado_DeveRetornarNotFound()
        {
            // Arrange
            var estoqueId = Guid.NewGuid();

            _estoqueRepositoryMock.Setup(x => x.GetById(estoqueId, It.IsAny<CancellationToken>())).ReturnsAsync((Estoque)null);

            // Act
            var result = await _estoqueService.GetById(estoqueId, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }
    }
}
