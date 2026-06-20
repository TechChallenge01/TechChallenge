using Domain.Aggregates.EstoqueAggregates;
using Shared.DTOs;
using Shared.DTOs.Estoques.Output;
using Shared.Result;
using System.Net;

namespace Application.Presenters.Estoques;
public class EstoquePresenter
{
    private string _message;

    public EstoquePresenter(string? message = null)
    {
        _message = message ?? string.Empty;
    }

    public EstoqueOutputDTO Transform(Estoque estoque)
    {
        return new EstoqueOutputDTO
        {
            Id = estoque.Id,
            PecaId = estoque.PecaId,
            InsumoId = estoque.InsumoId,
            QuantidadeDisponivel = estoque.QuantidadeDisponivel,
            QuantidadeReservada = estoque.QuantidadeReservada,
            QuantidadeTotal = estoque.QuantidadeTotal
        };
    }

    public ICommandResult<EstoqueOutputDTO> TransformObject(Estoque estoque)
    {
        return new CommandResult<EstoqueOutputDTO>
        {
            Data = Transform(estoque),
            Message = _message,
            StatusCode = HttpStatusCode.OK
        };
    }

    public ICommandResult<PagedResultDTO<EstoqueOutputDTO>> TransformPaged(List<Estoque> estoques, int pageNumber, int total)
    {
        return new CommandResult<PagedResultDTO<EstoqueOutputDTO>>
        {
            Data = new PagedResultDTO<EstoqueOutputDTO>
            { 
                Items = estoques.Select(Transform).ToList(),
                TotalItems = total,
                Page = pageNumber,
                PageSize = estoques.Count,
                TotalPages = (int)Math.Ceiling((double)total / estoques.Count)
            },
            Message = _message,
            StatusCode = HttpStatusCode.PartialContent
        };
    }

    public ICommandResult<T> Created<T>(T data)
    {
        return new CommandResult<T> { Message = _message, StatusCode = HttpStatusCode.Created, Data = data };
    }
    public ICommandResult NoContent(string message)
    {
        return new CommandResult { Message = message, StatusCode = HttpStatusCode.NoContent };
    }
    public ICommandResult InternalError(string message)
    {
        return new CommandResult { Message = message, StatusCode = HttpStatusCode.InternalServerError };
    }
    public ICommandResult BadRequest(string message)
    {
        return new CommandResult { Message = message, StatusCode = HttpStatusCode.BadRequest };
    }
    public ICommandResult NotFound(string message)
    {
        return new CommandResult { Message = message, StatusCode = HttpStatusCode.NotFound };
    }
    public ICommandResult<T> InternalError<T>(string message)
    {
        return new CommandResult<T> { Message = message, StatusCode = HttpStatusCode.InternalServerError };
    }
    public ICommandResult<T> BadRequest<T>(string message)
    {
        return new CommandResult<T> { Message = message, StatusCode = HttpStatusCode.BadRequest };
    }
    public ICommandResult<T> NotFound<T>(string message)
    {
        return new CommandResult<T> { Message = message, StatusCode = HttpStatusCode.NotFound };
    }
}
