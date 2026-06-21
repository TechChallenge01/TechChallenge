using Application.Gateways.Pecas;
using Application.Interfaces;
using Application.Presenters.Pecas;
using Application.UseCases.Pecas;
using Shared.DTOs;
using Shared.DTOs.Pecas.Output;
using Shared.Result;

namespace Application.Controllers.Pecas;

public class PecaController
{
    private readonly IPecaDataSource _dataSource;

    public PecaController(IPecaDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<ICommandResult<PagedResultDTO<PecaOutputDTO>>> GetPaginated(int page, int pageSize, CancellationToken ct)
    {
        var presenter = new PecaPresenter("Pesquisa de peças retornada com sucesso!");
        try
        {
            var pecaGateway = PecaGateway.Create(_dataSource);
            var useCase = GetPaginatedUseCase.Create(pecaGateway);

            var pecas = await useCase.Run(page, pageSize, ct);

            return presenter.TransformPaged(pecas.Pecas, page, pecas.total);
        }
        catch (ArgumentException ex)
        {
            return presenter.BadRequest<PagedResultDTO<PecaOutputDTO>>(ex.Message);
        }
        catch (Exception ex)
        {
            return presenter.InternalError<PagedResultDTO<PecaOutputDTO>>(ex.Message);
        }
    }

    public async Task<ICommandResult<PecaOutputDTO>> GetById(Guid id, CancellationToken ct)
    {
        var presenter = new PecaPresenter("Peça retornada com sucesso!");
        try
        {
            var pecaGateway = PecaGateway.Create(_dataSource);
            var useCase = GetByIdUseCase.Create(pecaGateway);

            var peca = await useCase.Run(id, ct);

            if (peca is null)
                return presenter.NotFound<PecaOutputDTO>("Peça não encontrada!");

            return presenter.TransformObject(peca);
        }
        catch (ArgumentException ex)
        {
            return presenter.BadRequest<PecaOutputDTO>(ex.Message);
        }
        catch (Exception ex)
        {
            return presenter.InternalError<PecaOutputDTO>(ex.Message);
        }
    }

    public async Task<ICommandResult<Guid>> Create(PecaRequestDTO request, Guid idUsuario, CancellationToken ct)
    {
        var presenter = new PecaPresenter("Peça criada com sucesso!");
        try
        {
            var pecaGateway = PecaGateway.Create(_dataSource);
            var useCase = CreateUseCase.Create(pecaGateway);
            var idPeca = await useCase.Run(request, idUsuario, ct);

            return presenter.Created(idPeca);
        }
        catch (ArgumentException ex)
        {
            return presenter.BadRequest<Guid>(ex.Message);
        }
        catch (Exception ex)
        {
            return presenter.InternalError<Guid>(ex.Message);
        }
    }

    public async Task<ICommandResult> Update(Guid id, PecaRequestDTO request, Guid idUsuario, CancellationToken ct)
    {
        var presenter = new PecaPresenter("Peça atualizada com sucesso!");
        try
        {
            var pecaGateway = PecaGateway.Create(_dataSource);
            var useCase = UpdateUseCase.Create(pecaGateway);
            await useCase.Run(idUsuario, id, request, ct);

            return presenter.NoContent("Peça atualizada com sucesso!");
        }
        catch (KeyNotFoundException ex)
        {
            return presenter.NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return presenter.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return presenter.InternalError(ex.Message);
        }
    }

    public async Task<ICommandResult> Delete(Guid id, Guid idUsuario, CancellationToken ct)
    {
        var presenter = new PecaPresenter("Peça deletada com sucesso!");
        try
        {
            var gateway = PecaGateway.Create(_dataSource);
            var useCase = DeleteUseCase.Create(gateway);
            await useCase.Run(idUsuario, id, ct);

            return presenter.NoContent("Peça deletada com sucesso!");
        }
        catch (KeyNotFoundException ex)
        {
            return presenter.NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return presenter.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return presenter.InternalError(ex.Message);
        }
    }
}