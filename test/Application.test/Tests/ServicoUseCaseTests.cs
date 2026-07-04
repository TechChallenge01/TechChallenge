using Application.Gateways.Servicos;
using Application.Interfaces;
using Application.UseCases.Servicos;
using Moq;
using Shared.DTOs.Servicos.Input;
using Shared.DTOs.Servicos.Requests;

namespace Application.test.Tests;

public class ServicoUseCaseTests
{
    private static Mock<IServicoDataSource> CriarMockDataSource(Guid? idRetorno = null)
    {
        var id = idRetorno ?? Guid.NewGuid();
        var mock = new Mock<IServicoDataSource>();

        mock.Setup(m => m.Create(It.IsAny<ServicoInputDTO>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        mock.Setup(m => m.Update(It.IsAny<ServicoInputDTO>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        mock.Setup(m => m.UpdateServicos(It.IsAny<ICollection<ServicoInputDTO>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        mock.Setup(m => m.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(new ServicoInputDTO { Id = id, Nome = "Troca de Óleo", Descricao = "Troca completa", ValorUnitario = 120.00m }));
        mock.Setup(m => m.GetPaginated(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<ServicoInputDTO> { new ServicoInputDTO { Id = id, Nome = "Troca de Óleo", Descricao = "Troca completa", ValorUnitario = 120.00m } } as ICollection<ServicoInputDTO>, 1));

        return mock;
    }

    [Fact]
    public async Task CriarServico_DeveRetornarGuidValido()
    {
        var mock = CriarMockDataSource();
        var gateway = ServicoGateway.Create(mock.Object);
        var useCase = CreateUseCase.Create(gateway);
        var request = new ServicoRequestDTO { Nome = "Troca de Óleo", Descricao = "Troca completa", PrecoVenda = 120.00m };

        var id = await useCase.Run(request, Guid.NewGuid(), CancellationToken.None);

        Assert.NotEqual(Guid.Empty, id);
    }

    [Fact]
    public async Task CriarServico_DeveCallCreateNoDataSource()
    {
        var mock = CriarMockDataSource();
        var gateway = ServicoGateway.Create(mock.Object);
        var useCase = CreateUseCase.Create(gateway);
        var request = new ServicoRequestDTO { Nome = "Troca de Óleo", Descricao = "Troca completa", PrecoVenda = 120.00m };

        await useCase.Run(request, Guid.NewGuid(), CancellationToken.None);

        mock.Verify(m => m.Create(It.IsAny<ServicoInputDTO>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AtualizarServico_ComDadosValidos_DeveCallUpdateNoDataSource()
    {
        var id = Guid.NewGuid();
        var mock = CriarMockDataSource(id);
        var gateway = ServicoGateway.Create(mock.Object);
        var useCase = UpdateUseCase.Create(gateway);
        var request = new ServicoRequestDTO { Nome = "Alinhamento", Descricao = "Alinhamento completo", PrecoVenda = 150.00m };

        await useCase.Run(Guid.NewGuid(), id, request, CancellationToken.None);

        mock.Verify(m => m.Update(It.IsAny<ServicoInputDTO>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AtualizarServico_ServicoNaoEncontrado_DeveThrowKeyNotFoundException()
    {
        var mock = new Mock<IServicoDataSource>();
        mock.Setup(m => m.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<ServicoInputDTO>(null));
        var gateway = ServicoGateway.Create(mock.Object);
        var useCase = UpdateUseCase.Create(gateway);
        var request = new ServicoRequestDTO { Nome = "X", Descricao = "Y", PrecoVenda = 10m };

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            useCase.Run(Guid.NewGuid(), Guid.NewGuid(), request, CancellationToken.None));
    }

    [Fact]
    public async Task DeletarServico_DeveInativarECallUpdate()
    {
        var id = Guid.NewGuid();
        var mock = CriarMockDataSource(id);
        var gateway = ServicoGateway.Create(mock.Object);
        var useCase = DeleteUseCase.Create(gateway);

        await useCase.Run(Guid.NewGuid(), id, CancellationToken.None);

        mock.Verify(m => m.Update(It.Is<ServicoInputDTO>(dto => dto.Ativo == false), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeletarServico_ServicoNaoEncontrado_DeveThrowKeyNotFoundException()
    {
        var mock = new Mock<IServicoDataSource>();
        mock.Setup(m => m.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<ServicoInputDTO>(null));
        var gateway = ServicoGateway.Create(mock.Object);
        var useCase = DeleteUseCase.Create(gateway);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            useCase.Run(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));
    }

    [Fact]
    public async Task ObterServicoPorId_DeveRetornarServicoCorreto()
    {
        var id = Guid.NewGuid();
        var mock = CriarMockDataSource(id);
        var gateway = ServicoGateway.Create(mock.Object);
        var useCase = GetByIdUseCase.Create(gateway);

        var servico = await useCase.Run(id, CancellationToken.None);

        Assert.NotNull(servico);
        Assert.Equal(id, servico.Id);
        Assert.Equal("Troca de Óleo", servico.Nome);
    }

    [Fact]
    public async Task ObterServicoPorId_ServicoNaoEncontrado_DeveRetornarNull()
    {
        var mock = new Mock<IServicoDataSource>();
        mock.Setup(m => m.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<ServicoInputDTO>(null));
        var gateway = ServicoGateway.Create(mock.Object);
        var useCase = GetByIdUseCase.Create(gateway);

        var servico = await useCase.Run(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(servico);
    }

    [Fact]
    public async Task ObterServicosPaginados_DeveRetornarListaETotal()
    {
        var mock = CriarMockDataSource();
        var gateway = ServicoGateway.Create(mock.Object);
        var useCase = GetPaginatedUseCase.Create(gateway);

        var (servicos, total) = await useCase.Run(1, 10, CancellationToken.None);

        Assert.NotEmpty(servicos);
        Assert.Equal(1, total);
    }
}
