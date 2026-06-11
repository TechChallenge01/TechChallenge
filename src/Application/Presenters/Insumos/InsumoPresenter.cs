using Shared.DTOs.Insumos.Output;
using Shared.Result;
using System.Net;
using Shared.DTOs;
using Domain.Entities;

namespace Application.Presenters.Insumos;

public class InsumoPresenter
{
    private string _message;

    public InsumoPresenter(string? message = null) { _message = message ?? string.Empty; }
    public InsumoOutputDTO Transform(Insumo insumo)
    {
        return new InsumoOutputDTO
        {
            Id = insumo.Id,
            Nome = insumo.Nome,
            Descricao = insumo.Descricao,
            CustoUnitario = insumo.CustoUnitario
        };
    }

    public ICommandResult<InsumoOutputDTO> TransformObject(Insumo insumo)
    {
        return new CommandResult<InsumoOutputDTO>
        {
            Data = Transform(insumo),
            Message = _message,
            StatusCode = HttpStatusCode.OK
        };
    }

    public ICommandResult<List<InsumoOutputDTO>> TransformList(List<Insumo> insumos)
    {
        return new CommandResult<List<InsumoOutputDTO>> { Data = insumos.Select(Transform).ToList(), Message = _message, StatusCode = HttpStatusCode.OK };
    }

    public ICommandResult<PagedResultDTO<InsumoOutputDTO>> TransformPaged(List<Insumo> insumos, int pageNumber, int total)
    {
        return new CommandResult<PagedResultDTO<InsumoOutputDTO>>
        {
            Data = new PagedResultDTO<InsumoOutputDTO>
            {
                Items = insumos.Select(Transform).ToList(),
                TotalItems = total,
                Page = pageNumber,
                PageSize = insumos.Count,
                TotalPages = (int)Math.Ceiling((double)total / insumos.Count)
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
