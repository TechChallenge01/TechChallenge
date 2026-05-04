using Application.Servicos.DTOs.Requests;
using Application.Servicos.Services;
using Domain.Entities;
using Domain.Entities.Repositories;
using Moq;
using System.Net;
using Application.UnitOfWork;

namespace Application.test.Tests
{
    public class ServicoServiceTests
    {
        private readonly Mock<IServicoRepository> _servicoRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly ServicoService _servicoService;

        public ServicoServiceTests()
        {
            _servicoRepositoryMock = new Mock<IServicoRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _servicoService = new ServicoService(_servicoRepositoryMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Create_ComDadosValidos_DeveRetornarCreated()
        {
            // Arrange
            var request = new ServicoRequestDTO
            {
                Nome = "Troca de Óleo",
                Descricao = "Troca de óleo e filtro",
                PrecoVenda = 120.00m
            };
            var usuarioId = Guid.NewGuid();

            _servicoRepositoryMock.Setup(x => x.Create(It.IsAny<Servico>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            var result = await _servicoService.Create(request, usuarioId, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
        }

        [Fact]
        public async Task Update_ComDadosValidos_DeveRetornarNoContent()
        {
            // Arrange
            var servicoId = Guid.NewGuid();
            var usuarioId = Guid.NewGuid();
            var request = new ServicoRequestDTO
            {
                Nome = "Troca de Óleo",
                Descricao = "Troca de óleo novo",
                PrecoVenda = 150.00m
            };
            var servico = new Servico("Troca de Óleo", "Descrição", 120.00m, usuarioId, DateTime.UtcNow);

            _servicoRepositoryMock.Setup(x => x.GetById(servicoId, It.IsAny<CancellationToken>())).ReturnsAsync(servico);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));

            // Act
            var result = await _servicoService.Update(servicoId, usuarioId, request, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
        }

        [Fact]
        public async Task Delete_ComServicoValido_DeveRetornarNoContent()
        {
            // Arrange
            var servicoId = Guid.NewGuid();
            var usuarioId = Guid.NewGuid();
            var servico = new Servico("Troca de Óleo", "Descrição", 120.00m, usuarioId, DateTime.UtcNow);

            _servicoRepositoryMock.Setup(x => x.GetById(servicoId, It.IsAny<CancellationToken>())).ReturnsAsync(servico);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _servicoService.Delete(servicoId, usuarioId, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
        }

        [Fact]
        public async Task GetById_ComServicoValido_DeveRetornarOk()
        {
            // Arrange
            var servicoId = Guid.NewGuid();
            var usuarioId = Guid.NewGuid();
            var servico = new Servico("Troca de Óleo", "Descrição", 120.00m, usuarioId, DateTime.UtcNow);

            _servicoRepositoryMock.Setup(x => x.GetById(servicoId, It.IsAny<CancellationToken>())).ReturnsAsync(servico);

            // Act
            var result = await _servicoService.GetById(servicoId, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.NotNull(result.Data);
        }
    }
}
