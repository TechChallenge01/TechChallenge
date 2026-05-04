using Application.Clientes.DTOs.Requests;
using Application.Clientes.Services;
using Domain.Aggregates.ClienteAggregates;
using Domain.Aggregates.ClienteAggregates.Repositories;
using Domain.ValueObjects;
using Moq;
using System.Net;
using Application.UnitOfWork;
using Xunit;
using Application.Clientes.DTOs.Shared;

namespace Application.test.Tests
{
    public class ClienteServiceTests
    {
        private readonly Mock<IClienteRepository> _clienteRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly ClienteService _clienteService;

        public ClienteServiceTests()
        {
            _clienteRepositoryMock = new Mock<IClienteRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _clienteService = new ClienteService(_clienteRepositoryMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Create_ComCpfValido_DeveRetornarCreated()
        {
            // Arrange
            var request = new ClienteRequestDTO
            {
                Nome = "João Silva",
                Cpf = "50872558843",
                Email = "joao@email.com",
                Telefone = new TelefoneDTO { DDD = "11", DDI = "55", Numero = "987654321" },
                Enderecos = new EnderecoDTO { Logradouro = "Rua A", Numero = "123", Complemento = null, Bairro = "Centro",Cidade = "São Paulo",Uf = "SP",Cep = "01310100" }
            };
            var usuarioId = Guid.NewGuid();

            _clienteRepositoryMock.Setup(x => x.GetByCpf(It.IsAny<Cpf>(), It.IsAny<CancellationToken>())).ReturnsAsync((Cliente)null);
            _clienteRepositoryMock.Setup(x => x.Create(It.IsAny<Cliente>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            var result = await _clienteService.Create(request, usuarioId, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
        }

        [Fact]
        public async Task Delete_ComClienteValido_DeveRetornarNoContent()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var usuarioId = Guid.NewGuid();
            var cliente = new Cliente("João", new Cpf("50872558843"), usuarioId,
                new Endereco("Rua", "123", null, "Centro", "São Paulo", "SP", "01310100"),
                new Telefone("11", "55", "987654321"),
                new Email("joao@email.com"));

            _clienteRepositoryMock.Setup(x => x.GetById(clienteId, It.IsAny<CancellationToken>())).ReturnsAsync(cliente);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _clienteService.Delete(clienteId, usuarioId, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
        }

        [Fact]
        public async Task Update_ComClienteValido_DeveRetornarNoContent()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var usuarioId = Guid.NewGuid();
            var request = new ClienteRequestDTO
            {
                Nome = "João Silva Atualizado",
                Cpf = "50872558843",
                Email = "joao.novo@email.com",
                Telefone = new TelefoneDTO { DDD = "11", DDI = "55", Numero = "987654321" },
                Enderecos = new EnderecoDTO { Logradouro = "Rua B", Numero = "456", Complemento = null, Bairro = "Vila", Cidade = "São Paulo", Uf = "SP", Cep = "01310100" }
            };
            var cliente = new Cliente("João", new Cpf("50872558843"), usuarioId,
                new Endereco("Rua", "123", null, "Centro", "São Paulo", "SP", "01310100"),
                new Telefone("11", "55", "987654321"),
                new Email("joao@email.com"));

            _clienteRepositoryMock.Setup(x => x.GetById(clienteId, It.IsAny<CancellationToken>())).ReturnsAsync(cliente);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _clienteService.Update(clienteId, usuarioId, request, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
        }

        [Fact]
        public async Task GetById_ComClienteValido_DeveRetornarOk()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var cliente = new Cliente("João", new Cpf("50872558843"), usuarioId,
                new Endereco("Rua", "123", null, "Centro", "São Paulo", "SP", "01310100"),
                new Telefone("11", "55", "987654321"),
                new Email("joao@email.com"));

            _clienteRepositoryMock.Setup(x => x.GetById(cliente.Id, It.IsAny<CancellationToken>())).ReturnsAsync(cliente);

            // Act
            var result = await _clienteService.GetById(cliente.Id, CancellationToken.None);

            // Assert
            Assert.True(result.StatusCode == System.Net.HttpStatusCode.PartialContent || result.StatusCode == System.Net.HttpStatusCode.OK);
        }
    }
}
