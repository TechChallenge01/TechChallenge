using Domain.Entities;
using Shared.DTOs;
using Shared.DTOs.Pecas.Output;
using Shared.Result;
using System.Net;

namespace Application.Presenters.Pecas;

public class PecaPresenter
{
    private readonly string _message;

    public PecaPresenter(string? message = null) { _message = message ?? string.Empty; }

    public PecaOutputDTO Transform(Peca peca)
    {
        return new PecaOutputDTO
        {
            Id = peca.Id,
            Nome = peca.Nome,
            Descricao = peca.Descricao,
            MarcaPeca = peca.MarcaPeca,
            ValorUnitario = peca.ValorUnitario
        };
    }

    public ICommandResult<PecaOutputDTO> TransformObject(Peca peca)
    {
        return new CommandResult<PecaOutputDTO>
        {
            Data = Transform(peca),
            Message = _message,
            StatusCode = HttpStatusCode.OK
        };
    }

    public ICommandResult<List<PecaOutputDTO>> TransformList(List<Peca> pecas)
    {
        return new CommandResult<List<PecaOutputDTO>> { Data = pecas.Select(Transform).ToList(), Message = _message, StatusCode = HttpStatusCode.OK };
    }

    public ICommandResult<PagedResultDTO<PecaOutputDTO>> TransformPaged(List<Peca> pecas, int pageNumber, int total)
    {
        return new CommandResult<PagedResultDTO<PecaOutputDTO>>
        {
            Data = new PagedResultDTO<PecaOutputDTO>
            {
                Items = pecas.Select(Transform).ToList(),
                TotalItems = total,
                Page = pageNumber,
                PageSize = pecas.Count,
                TotalPages = (int)Math.Ceiling((double)total / pecas.Count)
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