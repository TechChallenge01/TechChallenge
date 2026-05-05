using Application.Veiculos.DTOs.Requests;
using Application.Veiculos.Services;
using Domain.Aggregates.ClienteAggregates;
using Domain.Aggregates.ClienteAggregates.Repositories;
using Domain.Entities;
using Domain.Entities.Repositories;
using Domain.ValueObjects;
using Moq;
using System.Net;
using Application.UnitOfWork;

namespace Application.test.Tests
{
    public class VeiculoServiceTests
    {
        private readonly Mock<IVeiculoRepository> _veiculoRepositoryMock;
        private readonly Mock<IClienteRepository> _clienteRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly VeiculoService _veiculoService;

        public VeiculoServiceTests()
        {
            _veiculoRepositoryMock = new Mock<IVeiculoRepository>();
            _clienteRepositoryMock = new Mock<IClienteRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _veiculoService = new VeiculoService(_veiculoRepositoryMock.Object, _clienteRepositoryMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Create_ComDadosValidos_DeveRetornarCreated()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var request = new VeiculoRequestDTO
            {
                ClienteId = clienteId,
                Modelo = "Civic",
                MarcaVeiculo = "Honda",
                Ano = 2020,
                Placa = "ABC1234",
                Cor = "Preto"
            };
            var usuarioId = Guid.NewGuid();
            var cliente = new Cliente("João", new Cpf("50872558843"), usuarioId,
                new Endereco("Rua", "123", null, "Centro", "São Paulo", "SP", "01310100"),
                new Telefone("11", "55", "987654321"),
                new Email("joao@email.com"));

            _clienteRepositoryMock.Setup(x => x.GetById(clienteId, It.IsAny<CancellationToken>())).ReturnsAsync(cliente);
            _veiculoRepositoryMock.Setup(x => x.Create(It.IsAny<Veiculo>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            var result = await _veiculoService.Create(request, usuarioId, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
        }

        [Fact]
        public async Task Delete_ComVeiculoValido_DeveRetornarNoContent()
        {
            // Arrange
            var veiculoId = Guid.NewGuid();
            var clienteId = Guid.NewGuid();
            var usuarioId = Guid.NewGuid();
            var veiculo = new Veiculo("Civic", "Honda", clienteId, 2020, new Placa("ABC1234"), "Preto", usuarioId);

            _veiculoRepositoryMock.Setup(x => x.GetById(veiculoId, It.IsAny<CancellationToken>())).ReturnsAsync(veiculo);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _veiculoService.Delete(veiculoId, usuarioId, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
        }

        [Fact]
        public async Task Update_ComVeiculoValido_DeveRetornarNoContent()
        {
            // Arrange
            var veiculoId = Guid.NewGuid();
            var clienteId = Guid.NewGuid();
            var usuarioId = Guid.NewGuid();
            var request = new VeiculoRequestDTO
            {
                ClienteId = clienteId,
                Modelo = "Civic Atualizado",
                MarcaVeiculo = "Honda",
                Ano = 2021,
                Placa = "ABC1234",
                Cor = "Branco"
            };
            var veiculo = new Veiculo("Civic", "Honda", clienteId, 2020, new Placa("ABC1234"), "Preto", usuarioId);
            var cliente = new Cliente("João", new Cpf("50872558843"), usuarioId,
                new Endereco("Rua", "123", null, "Centro", "São Paulo", "SP", "01310100"),
                new Telefone("11", "55", "987654321"),
                new Email("joao@email.com"));

            _veiculoRepositoryMock.Setup(x => x.GetById(veiculoId, It.IsAny<CancellationToken>())).ReturnsAsync(veiculo);
            _clienteRepositoryMock.Setup(x => x.GetById(clienteId, It.IsAny<CancellationToken>())).ReturnsAsync(cliente);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _veiculoService.Update(veiculoId, usuarioId, request, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
        }

        [Fact]
        public async Task GetById_ComVeiculoValido_DeveRetornarOk()
        {
            // Arrange
            var veiculoId = Guid.NewGuid();
            var clienteId = Guid.NewGuid();
            var usuarioId = Guid.NewGuid();
            var veiculo = new Veiculo("Civic", "Honda", clienteId, 2020, new Placa("ABC1234"), "Preto", usuarioId);

            _veiculoRepositoryMock.Setup(x => x.GetById(veiculoId, It.IsAny<CancellationToken>())).ReturnsAsync(veiculo);

            // Act
            var result = await _veiculoService.GetById(veiculoId, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.NotNull(result.Data);
        }
    }
}
