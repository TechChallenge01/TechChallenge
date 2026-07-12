using Domain.Aggregates.ClienteAggregates;
using Shared.DTOs;
using Shared.DTOs.Clientes.Output;
using Shared.DTOs.Clientes.Shared;
using Shared.Result;
using System.Net;

namespace Application.Presenters.Clientes
{
    public class ClientePresenter
    {
        private string _message;
        public ClientePresenter(string? message = null) { _message = message ?? string.Empty; }
        public ICommandResult<ClienteOutputDTO> TransformObject(Cliente cliente)
        {
            return new CommandResult<ClienteOutputDTO>{ Data = Transform(cliente), Message = _message, StatusCode = HttpStatusCode.OK };
        }
        public ICommandResult<List<ClienteOutputDTO>> TransformList(List<Cliente> clientes)
        {
            return new CommandResult<List<ClienteOutputDTO>>{ Data = clientes.Select(x => Transform(x)).ToList(), Message = _message, StatusCode = HttpStatusCode.OK };
        }
        public ICommandResult<PagedResultDTO<ClienteOutputDTO>> TransformPaged(List<Cliente> clientes, int pageNumber, int total)
        {
            return new CommandResult<PagedResultDTO<ClienteOutputDTO>>
            {
                Data = new PagedResultDTO<ClienteOutputDTO>
                {
                    Items = clientes.Select(Transform).ToList(),
                    TotalItems = total,
                    Page = pageNumber,
                    PageSize = clientes.Count,
                    TotalPages = (int)Math.Ceiling((double)total / clientes.Count)
                },
                Message = _message,
                StatusCode = HttpStatusCode.PartialContent
            };
        }
        public ClienteOutputDTO Transform(Cliente cliente)
        {
            return new ClienteOutputDTO
            {
                Id = cliente.Id,
                Nome = cliente.Nome,
                Email = cliente.Email.EnderecoEmail,
                Cnpj = cliente.Cnpj == null ? null : cliente.Cnpj.Valor,
                Cpf = cliente.Cpf == null ? null : cliente.Cpf.Valor,
                Endereco = new EnderecoDTO
                {
                    Bairro = cliente.Endereco.Bairro,
                    Cep = cliente.Endereco.Cep,
                    Cidade = cliente.Endereco.Cidade,
                    Complemento = cliente.Endereco.Complemento,
                    Logradouro = cliente.Endereco.Logradouro,
                    Numero = cliente.Endereco.Numero,
                    Uf = cliente.Endereco.Uf
                },
                Telefone = new TelefoneDTO
                {
                    DDD = cliente.Telefone.DDD,
                    DDI = cliente.Telefone.DDI,
                    Numero = cliente.Telefone.Numero
                },
                Veiculos = cliente.Veiculos?.Select(x => x.Id).ToList() ?? new List<Guid>()
            };
        }
        public ICommandResult<T> Created<T>(T data)
        {
            return new CommandResult<T> { Message = _message, StatusCode = HttpStatusCode.Created, Data =  data};
        }
        public ICommandResult NoContent()
        {
            return new CommandResult { Message = _message, StatusCode = HttpStatusCode.NoContent };
        }
        public ICommandResult InternalError(string message)
        {
            return new CommandResult{ Message = message, StatusCode = HttpStatusCode.InternalServerError };
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
            return new CommandResult<T> { Message = message , StatusCode = HttpStatusCode.InternalServerError };
        }
        public ICommandResult<T> BadRequest<T>(string message)
        {
            return new CommandResult<T> { Message = message , StatusCode = HttpStatusCode.BadRequest };
        }
        public ICommandResult<T> NotFound<T>(string message)
        {
            return new CommandResult<T> { Message = message , StatusCode = HttpStatusCode.NotFound};
        }
        public ICommandResult Conflict(string message)
        {
            return new CommandResult { Message = message, StatusCode = HttpStatusCode.Conflict };
        }
        public ICommandResult<T> Conflict<T>(string message)
        {
            return new CommandResult<T> { Message = message, StatusCode = HttpStatusCode.Conflict };
        }
    }
}
