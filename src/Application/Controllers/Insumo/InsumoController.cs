using Application.Gateways.Insumos;
using Application.Insumos.DTOs.Requests;
using Application.Interfaces;
using Application.Presenters.Insumos;
using Application.UseCases.Insumos;
using Shared.DTOs;
using Shared.DTOs.Insumo.Output;
using Shared.Result;

namespace Application.Controllers.Insumo;

public class InsumoController
{
    private readonly IInsumoDataSource _dataSource;

    public InsumoController(IInsumoDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<ICommandResult<PagedResultDTO<InsumoOutputDTO>>> GetPaginated(int page, int pageSize, CancellationToken ct)
    {
        var presenter = new InsumoPresenter("Pesquisa de insumos retornada com sucesso!");

        try
        {
            var insumoGateway = InsumoGateway.Create(_dataSource);
            var userCase = GetPaginatedUseCase.Create(insumoGateway);

            var insumos = await userCase.Run(page, pageSize, ct);

            return presenter.TransformPaged(insumos.Insumos, page, insumos.total);
        }
        catch (ArgumentException ex)
        {
            return presenter.BadRequest<PagedResultDTO<InsumoOutputDTO>>(ex.Message);
        }
        catch (Exception ex)
        {
            return presenter.InternalError<PagedResultDTO<InsumoOutputDTO>>(ex.Message);
        }
    }

    public async Task<ICommandResult<InsumoOutputDTO>> GetById(Guid id, CancellationToken ct)
    {
        var presenter = new InsumoPresenter("Pesquisa de insumo retornada com sucesso!");
        
        try 
        {
            var insumoGateway = InsumoGateway.Create(_dataSource);
            var userCase = GetByIdUseCase.Create(insumoGateway);

            var insumo = await userCase.Run(id, ct);

            if (insumo is null)
                return presenter.NotFound<InsumoOutputDTO>("Insumo não encontrado!");

            return presenter.TransformObject(insumo);
        }
        catch (ArgumentException ex)
        {
            return presenter.BadRequest<InsumoOutputDTO>(ex.Message);
        }
        catch (Exception ex)
        {
            return presenter.InternalError<InsumoOutputDTO>(ex.Message);
        }
    }

    public async Task<ICommandResult<Guid>> Create(InsumoRequestDTO request, Guid idUsuario, CancellationToken ct)
    {
        var presenter = new InsumoPresenter("Insumo criado com sucesso!");
        try
        {
            var insumoGateway = InsumoGateway.Create(_dataSource);
            var userCase = CreateUseCase.Create(insumoGateway);
            var idInsumo = await userCase.Run(request, idUsuario, ct);
            return presenter.Created(idInsumo);
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

    public async Task<ICommandResult> Update (Guid id, InsumoRequestDTO request, Guid idUsuario, CancellationToken ct)
    {
        var presenter = new InsumoPresenter("Insumo atualizado com sucesso!");
        try
        {
            var insumoGateway = InsumoGateway.Create(_dataSource);
            var userCase = UpdateUseCase.Create(insumoGateway);
            await userCase.Run(idUsuario, id, request, ct);

            return presenter.NoContent("Insumo atualizado com sucesso!");
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
        var presenter = new InsumoPresenter("Insumo deletado com sucesso!");
        try
        {
            var insumoGateway = InsumoGateway.Create(_dataSource);
            var userCase = DeleteUseCase.Create(insumoGateway);
            await userCase.Run(idUsuario, id, ct);

            return presenter.NoContent("Insumo deletado com sucesso!");
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
