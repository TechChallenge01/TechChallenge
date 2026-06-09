using Domain.Entities;
using Shared.DTOs;
using Shared.DTOs.Servicos.Output;
using Shared.Result;
using System.Net;

namespace Application.Presenters.Servicos
{
    public class ServicoPresenter
    {
        private string _message;
        public ServicoPresenter(string? message = null) { _message = message ?? string.Empty; }
        public ICommandResult<ServicoOutputDTO> TransformObject(Servico servico)
        {
            return new CommandResult<ServicoOutputDTO> { Data = Transform(servico), Message = _message, StatusCode = HttpStatusCode.OK };
        }
        public ICommandResult<List<ServicoOutputDTO>> TransformList(List<Servico> servicos)
        {
            return new CommandResult<List<ServicoOutputDTO>> { Data = servicos.Select(x => Transform(x)).ToList(), Message = _message, StatusCode = HttpStatusCode.OK };
        }
        public ICommandResult<PagedResultDTO<ServicoOutputDTO>> TransformPaged(List<Servico> servicos, int pageNumber, int total)
        {
            return new CommandResult<PagedResultDTO<ServicoOutputDTO>>
            {
                Data = new PagedResultDTO<ServicoOutputDTO>
                {
                    Items = servicos.Select(Transform).ToList(),
                    TotalItems = total,
                    Page = pageNumber,
                    PageSize = servicos.Count,
                    TotalPages = (int)Math.Ceiling((double)total / servicos.Count)
                },
                Message = _message,
                StatusCode = HttpStatusCode.PartialContent
            };
        }
        public ServicoOutputDTO Transform(Servico servico)
        {
            return new ServicoOutputDTO
            {
                Id = servico.Id,
                Nome = servico.Nome,
               Descricao = servico.Descricao,
               PrecoVenda = servico.ValorUnitario,
               TempoMedioExecucao = servico.TempoMedioExecucao
            };
        }
        public ICommandResult<T> Created<T>(T data)
        {
            return new CommandResult<T> { Message = _message, StatusCode = HttpStatusCode.Created, Data = data };
        }
        public ICommandResult NoContent()
        {
            return new CommandResult { Message = _message, StatusCode = HttpStatusCode.NoContent };
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
}
