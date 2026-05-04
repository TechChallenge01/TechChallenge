using Application.Pecas.DTOs.Requests;
using Application.Pecas.Services;
using Domain.Aggregates.EstoqueAggregates;
using Domain.Aggregates.EstoqueAggregates.Repositories;
using Domain.Entities;
using Domain.Entities.Repositories;
using Moq;
using Shared.Result;
using System.Net;
using Application.UnitOfWork;
using Xunit;

namespace Application.test.Tests
{
    public class PecaServiceTests
    {
        private readonly Mock<IPecaRepository> _pecaRepositoryMock;
        private readonly Mock<IEstoqueRepository> _estoqueRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly PecaService _pecaService;

        public PecaServiceTests()
        {
            _pecaRepositoryMock = new Mock<IPecaRepository>();
            _estoqueRepositoryMock = new Mock<IEstoqueRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _pecaService = new PecaService(_pecaRepositoryMock.Object, _estoqueRepositoryMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Create_ComDadosValidos_DeveRetornarCreated()
        {
            // Arrange
            var request = new PecaRequestDTO
            {
                Nome = "Filtro de Ar",
                Descricao = "Filtro premium",
                MarcaPeca = "Bosch",
                PrecoVenda = 85.50m
            };
            var usuarioId = Guid.NewGuid();

            _pecaRepositoryMock.Setup(x => x.Create(It.IsAny<Peca>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _estoqueRepositoryMock.Setup(x => x.Create(It.IsAny<Estoque>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            var result = await _pecaService.Create(request, usuarioId, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task Update_ComDadosValidos_DeveRetornarNoContent()
        {
            // Arrange
            var pecaId = Guid.NewGuid();
            var usuarioId = Guid.NewGuid();
            var request = new PecaRequestDTO
            {
                Nome = "Filtro Novo",
                Descricao = "Descrição nova",
                MarcaPeca = "Bosch",
                PrecoVenda = 100.00m
            };

            var peca = new Peca("Filtro", "Descrição", "Bosch", 85.50m, usuarioId, DateTime.UtcNow);
            _pecaRepositoryMock.Setup(x => x.GetById(pecaId, It.IsAny<CancellationToken>())).ReturnsAsync(peca);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _pecaService.Update(pecaId, usuarioId, request, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
        }

        [Fact]
        public async Task Delete_ComPecaValida_DeveRetornarNoContent()
        {
            // Arrange
            var pecaId = Guid.NewGuid();
            var usuarioId = Guid.NewGuid();
            var peca = new Peca("Filtro", "Descrição", "Bosch", 85.50m, usuarioId, DateTime.UtcNow);
            var estoque = new Estoque(null, pecaId, 0, usuarioId, DateTime.UtcNow);

            _pecaRepositoryMock.Setup(x => x.GetById(pecaId, It.IsAny<CancellationToken>())).ReturnsAsync(peca);
            _estoqueRepositoryMock.Setup(x => x.GetByPecaId(pecaId, It.IsAny<CancellationToken>())).ReturnsAsync(estoque);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _pecaService.Delete(pecaId, usuarioId, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
        }

        [Fact]
        public async Task Delete_ComPecaNaoEncontrada_DeveRetornarNotFound()
        {
            // Arrange
            var pecaId = Guid.NewGuid();
            var usuarioId = Guid.NewGuid();

            _pecaRepositoryMock.Setup(x => x.GetById(pecaId, It.IsAny<CancellationToken>())).ReturnsAsync((Peca)null);

            // Act
            var result = await _pecaService.Delete(pecaId, usuarioId, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task GetById_ComPecaValida_DeveRetornarOk()
        {
            // Arrange
            var pecaId = Guid.NewGuid();
            var usuarioId = Guid.NewGuid();
            var peca = new Peca("Filtro", "Descrição", "Bosch", 85.50m, usuarioId, DateTime.UtcNow);

            _pecaRepositoryMock.Setup(x => x.GetById(pecaId, It.IsAny<CancellationToken>())).ReturnsAsync(peca);

            // Act
            var result = await _pecaService.GetById(pecaId, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.NotNull(result.Data);
        }
    }
}
