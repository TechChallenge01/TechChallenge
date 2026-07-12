using Application.Gateways.Estoques;
using Application.Interfaces;
using Application.Presenters.Estoques;
using Application.UseCases.Estoques;
using Shared.DTOs;
using Shared.DTOs.Estoques.Output;
using Shared.DTOs.Estoques.Request;
using Shared.Result;

namespace Application.Controllers.Estoques;

public class EstoqueController
{
    private readonly IEstoqueDataSource _dataSource;

    public EstoqueController(IEstoqueDataSource dataSource)
    { _dataSource = dataSource; }

    public async Task<ICommandResult<PagedResultDTO<EstoqueOutputDTO>>> GetPaginated(int page, int pageSize, CancellationToken ct)
    {
        var presenter = new EstoquePresenter("Estoques retornada com sucesso.");

        try
        {
            var estoqueGateway = EstoqueGateway.Create(_dataSource);
            var useCase = GetPaginatedUseCase.Create(estoqueGateway);

            var estoque = await useCase.Run(page, pageSize, ct);

            return presenter.TransformPaged(estoque.Estoques, page, estoque.total);
        }
        catch (ArgumentException ex)
        {
            return presenter.BadRequest<PagedResultDTO<EstoqueOutputDTO>>(ex.Message);
        }
        catch (Exception ex)
        {
            return presenter.InternalError<PagedResultDTO<EstoqueOutputDTO>>(ex.Message);
        }
    }

    public async Task<ICommandResult<EstoqueOutputDTO>> GetById(Guid id, CancellationToken ct)
    {
        var presenter = new EstoquePresenter();

        try 
        {
            var estoqueGataway = EstoqueGateway.Create(_dataSource);
            var useCase = GetByIdUseCase.Create(estoqueGataway);

            var estoque = await useCase.Run(id, ct);

            if (estoque is null)
                return presenter.NotFound<EstoqueOutputDTO>("Estoque não encontrado");

            return presenter.TransformObject(estoque);
        }
        catch (ArgumentException ex)
        {
            return presenter.BadRequest<EstoqueOutputDTO>(ex.Message);
        }
        catch (Exception ex)
        {
            return presenter.InternalError<EstoqueOutputDTO>(ex.Message);
        }
    }

    public async Task<ICommandResult<Guid>> Movimentar(EstoqueRequestDTO request, Guid idUsuario, CancellationToken ct)
    {
        var presenter = new EstoquePresenter("Movimentação realizada com sucesso");
        try
        {
            var estoqueGataway = EstoqueGateway.Create(_dataSource);
            var useCase = MovimentarUseCase.Create(estoqueGataway);

            var idEstoque = await useCase.Run(request, idUsuario, ct);
            return presenter.Created(idEstoque);
        }
        catch (KeyNotFoundException ex)
        {
            return presenter.NotFound<Guid>(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return presenter.BadRequest<Guid>(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return presenter.BadRequest<Guid>(ex.Message);
        }
        catch (Exception ex)
        {
            return presenter.InternalError<Guid>(ex.Message);
        }
    }
}
