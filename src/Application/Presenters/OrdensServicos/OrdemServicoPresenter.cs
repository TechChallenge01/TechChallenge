using Domain.Aggregates.OrdemServicoAggregates;
using Shared.DTOs;
using Shared.DTOs.OrdemServicos.Output;
using Shared.DTOs.OrdemServicos.Shared;
using Shared.Result;
using System.Net;

namespace Application.Presenters.OrdensServicos
{
    public class OrdemServicoPresenter
    {
        private string _message;

        public OrdemServicoPresenter(string? message = null) { _message = message ?? string.Empty; }
        public OrdemServicoOutputDTO Transform(OrdemServico ordemServico)
        {
            return new OrdemServicoOutputDTO
            {
                Id = ordemServico.Id,
                ClienteId = ordemServico.ClienteId,
                Observacao = ordemServico.Observacao,
                StatusOS = ordemServico.StatusOS,
                TempoExecucao = ordemServico.TempoExecucao,
                ValorDesconto = ordemServico.ValorDesconto,
                ValorTotal = ordemServico.ValorTotal,
                VeiculoId = ordemServico.VeiculoId,
                Insumos = ordemServico.Insumos.Select(i => new OrdemServicoInsumoDTO
                {
                    CustoUnitario = i.CustoUnitario,
                    InsumoId = i.InsumoId,
                    Quantidade = i.Quantidade
                }).ToList(),
                Pecas = ordemServico.Pecas.Select(p => new OrdemServicoPecaDTO
                {
                    PecaId = p.PecaId,
                    Quantidade = p.Quantidade,
                    ValorUnitario = p.ValorUnitario
                }).ToList(),
                Servicos = ordemServico.Servicos.Select(s => new OrdemServicoServicoDTO
                {
                    DataInicioExecucao = s.DataInicioExecucao,
                    DataTerminoExecucao = s.DataTerminoExecucao,
                    Quantidade = s.Quantidade,
                    ServicoId = s.ServicoId,
                    Status = s.Status,
                    ValorUnitario = s.ValorUnitario
                }).ToList()
            };
        }

        public ICommandResult<OrdemServicoOutputDTO> TransformObject(OrdemServico ordemServico)
        {
            return new CommandResult<OrdemServicoOutputDTO>
            {
                Data = Transform(ordemServico),
                Message = _message,
                StatusCode = HttpStatusCode.OK
            };
        }

        public ICommandResult<List<OrdemServicoOutputDTO>> TransformList(List<OrdemServico> ordemServico)
        {
            return new CommandResult<List<OrdemServicoOutputDTO>> { Data = ordemServico.Select(Transform).ToList(), Message = _message, StatusCode = HttpStatusCode.OK };
        }

        public ICommandResult<PagedResultDTO<OrdemServicoOutputDTO>> TransformPaged(List<OrdemServico> ordemServico, int pageNumber, int total)
        {
            return new CommandResult<PagedResultDTO<OrdemServicoOutputDTO>>
            {
                Data = new PagedResultDTO<OrdemServicoOutputDTO>
                {
                    Items = ordemServico.Select(Transform).ToList(),
                    TotalItems = total,
                    Page = pageNumber,
                    PageSize = ordemServico.Count,
                    TotalPages = (int)Math.Ceiling((double)total / ordemServico.Count)
                },
                Message = _message,
                StatusCode = HttpStatusCode.PartialContent
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
        public ICommandResult Forbidden(string message)
        {
            return new CommandResult { Message = message, StatusCode = HttpStatusCode.Forbidden };
        }
        public ICommandResult<T> Forbidden<T>(string message)
        {
            return new CommandResult<T> { Message = message, StatusCode = HttpStatusCode.Forbidden };
        }
        public ICommandResult<T> InternalError<T>(string message)
        {
            return new CommandResult<T> { Message = _message, StatusCode = HttpStatusCode.InternalServerError };
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
