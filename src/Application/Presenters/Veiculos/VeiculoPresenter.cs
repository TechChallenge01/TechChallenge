using Domain.Entities;
using Shared.DTOs;
using Shared.DTOs.Veiculos.Output;
using Shared.Result;
using System.Net;

namespace Application.Presenters.Veiculos
{
    public class VeiculoPresenter
    {
        private string _message;
        public VeiculoPresenter(string? message = null) { _message = message ?? string.Empty; }
        public ICommandResult<VeiculoOutputDTO> TransformObject(Veiculo veiculo)
        {
            return new CommandResult<VeiculoOutputDTO> { Data = Transform(veiculo), Message = _message, StatusCode = HttpStatusCode.OK };
        }
        public ICommandResult<List<VeiculoOutputDTO>> TransformList(List<Veiculo> veiculos)
        {
            return new CommandResult<List<VeiculoOutputDTO>> { Data = veiculos.Select(x => Transform(x)).ToList(), Message = _message, StatusCode = HttpStatusCode.OK };
        }
        public ICommandResult<PagedResultDTO<VeiculoOutputDTO>> TransformPaged(List<Veiculo> veiculos, int pageNumber, int total)
        {
            return new CommandResult<PagedResultDTO<VeiculoOutputDTO>>
            {
                Data = new PagedResultDTO<VeiculoOutputDTO>
                {
                    Items = veiculos.Select(Transform).ToList(),
                    TotalItems = total,
                    Page = pageNumber,
                    PageSize = veiculos.Count,
                    TotalPages = (int)Math.Ceiling((double)total / veiculos.Count)
                },
                Message = _message,
                StatusCode = HttpStatusCode.PartialContent
            };
        }
        public VeiculoOutputDTO Transform(Veiculo veiculo)
        {
            return new VeiculoOutputDTO
            {
                Id = veiculo.Id,
                Ano = veiculo.Ano,
                ClienteId = veiculo.ClienteId,
                Cor = veiculo.Cor,
                MarcaVeiculo = veiculo.MarcaVeiculo,
                Modelo = veiculo.Modelo,
                Placa = veiculo.Placa
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
