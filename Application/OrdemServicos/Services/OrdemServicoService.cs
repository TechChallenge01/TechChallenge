using Application.OrdemServicos.DTOs.Requests;
using Application.OrdemServicos.DTOs.Responses;
using Application.OrdemServicos.Presenters;
using Domain.Aggregates.ClienteAggregates.Repositories;
using Domain.Aggregates.EstoqueAggregates.Repositories;
using Domain.Aggregates.OrdemServicoAggregates;
using Domain.Aggregates.OrdemServicoAggregates.Repositories;
using Domain.Entities.Repositories;
using Domain.ValueObjects;
using Shared.Result;
using Shared.Result.DTO;
using System.Net;

namespace Application.OrdemServicos.Services;

public class OrdemServicoService : IOrdemServicoService
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IVeiculoRepository _veiculoRepository;
    private readonly IPecaRepository _pecaRepository;
    private readonly IServicoRepository _servicoRepository;
    private readonly IEstoqueRepository _estoqueRepository;
    public OrdemServicoService(IOrdemServicoRepository ordemServicoRepository, IClienteRepository clienteRepository, IVeiculoRepository veiculoRepository, IPecaRepository pecaRepository, IServicoRepository servicoRepository, IEstoqueRepository estoqueRepository)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _clienteRepository = clienteRepository;
        _veiculoRepository = veiculoRepository;
        _pecaRepository = pecaRepository;
        _servicoRepository = servicoRepository;
        _estoqueRepository = estoqueRepository;
    }

    public async Task<ICommandResult> Aprovar(int id, CancellationToken ct)
    {
        try
        {
            var ordemServico = await _ordemServicoRepository.GetById(id, ct);

            if (ordemServico is null)
                return new CommandResult { StatusCode = HttpStatusCode.NotFound, Message = "Ordem de serviço não encontrada." };

            ordemServico.AprovarOrdemServico();

            ordemServico.RastrearAlteracao(Guid.Empty, DateTime.UtcNow);

            await _ordemServicoRepository.Update(ordemServico, ct);

            return new CommandResult { StatusCode = HttpStatusCode.NoContent, Message = "Ordem de serviço aprovada com sucesso." };
        }
        catch (ArgumentException ex)
        {
            return new CommandResult { StatusCode = HttpStatusCode.BadRequest, Message = ex.Message };
        }
        catch (Exception ex)
        {
            return new CommandResult { StatusCode = HttpStatusCode.InternalServerError, Message = $"Erro interno no servidor. Detalhes: {ex.Message}" };
        }
    }
    public async Task<ICommandResult> Cancelar(int id, CancellationToken ct)
    {
        try
        {
            var ordemServico = await _ordemServicoRepository.GetById(id, ct);

            if(ordemServico is null)
                return new CommandResult { StatusCode = HttpStatusCode.NotFound, Message = "Ordem de serviço não encontrada." };

            if(ordemServico.Pecas.Any())
            {
                var estoques = await _estoqueRepository.GetByPecaIds(ordemServico.Pecas.Select(x => x.PecaId).ToList(), ct);

                foreach (var peca in ordemServico.Pecas)
                {
                    var estoque = estoques.FirstOrDefault(e => e.PecaId == peca.PecaId);

                    if (estoque is not null)
                    {
                        estoque.LiberarReserva(peca.Quantidade);
                    }
                }
            }

            ordemServico.CancelarOrdemServico();

            ordemServico.RastrearAlteracao(Guid.Empty, DateTime.UtcNow);

            await _ordemServicoRepository.Update(ordemServico, ct);

            return new CommandResult { StatusCode = HttpStatusCode.NoContent, Message = "Ordem de serviço cancelada com sucesso." };
        }
        catch (ArgumentException ex)
        {
            return new CommandResult { StatusCode = HttpStatusCode.BadRequest, Message = ex.Message };
        }
        catch (Exception ex)
        {
            return new CommandResult { StatusCode = HttpStatusCode.InternalServerError, Message = $"Erro interno no servidor. Detalhes: {ex.Message}" };
        }
    }

    public async Task<ICommandResult> FinalizarServico(int id, FinalizarServicoDTO dto, CancellationToken ct)
    {
        try
        {
            var ordemServico = await _ordemServicoRepository.GetById(id, ct);

            if (ordemServico is null)
                return new CommandResult { StatusCode = HttpStatusCode.NotFound, Message = "Ordem de serviço não encontrada." };

            var servicosEntities = await _servicoRepository.GetByIds(dto.ServicosId, ct);

            if (servicosEntities.Count() != dto.ServicosId.Count())
                return new CommandResult<Guid> { StatusCode = HttpStatusCode.NotFound, Message = "Um ou mais serviços não foram encontrados." };

            ordemServico.FinalizarOrdemServico(dto.ServicosId);

            var tempos = await _ordemServicoRepository.GetByIdsSTimeSpanDataExecucao(dto.ServicosId, ct);

            var dataAlteracao = DateTime.UtcNow;
            var usuarioAuditoria = Guid.Empty;

            foreach (var servico in servicosEntities)
            {
                servico.AtualizarTempoMedio(tempos);
                servico.RastrearAlteracao(usuarioAuditoria, dataAlteracao);
            }

            ordemServico.RastrearAlteracao(usuarioAuditoria, dataAlteracao);

            await _ordemServicoRepository.Update(ordemServico, ct);

            return new CommandResult<Guid> { StatusCode = HttpStatusCode.NoContent, Message = "Serviço finalizado com sucesso." };
        }
        catch (ArgumentException ex)
        {
            return new CommandResult<Guid> { StatusCode = HttpStatusCode.BadRequest, Message = ex.Message };
        }
        catch (Exception ex)
        {
            return new CommandResult<Guid> { StatusCode = HttpStatusCode.InternalServerError, Message = $"Erro interno no servidor. Detalhes: {ex.Message}" };
        }
    }

    public async Task<ICommandResult<Guid>> Create(OrdemServicoRequestDTO request, CancellationToken ct)
    {
        try
        {
            var cliente = await _clienteRepository.GetById(request.ClienteId, ct);
            if (cliente is null)
                return new CommandResult<Guid> { StatusCode = HttpStatusCode.NotFound, Message = "Cliente não encontrado." };

            var veiculo = await _veiculoRepository.GetById(request.VeiculoId, ct);
            if (veiculo is null)
                return new CommandResult<Guid> { StatusCode = HttpStatusCode.NotFound, Message = "Veículo não encontrado." };

            var entity = new OrdemServico(cliente.Id, veiculo.Id, Guid.Empty);

            if (request.Pecas is not null && request.Pecas.Any())
            {
                var pecasAgrupadas = request.Pecas
                    .GroupBy(p => p.PecaId)
                    .Select(g => new { PecaId = g.Key, QuantidadeTotal = g.Sum(x => x.Quantidade) })
                    .ToList();

                var idsPecas = pecasAgrupadas.Select(p => p.PecaId).ToList();
                var pecasEntities = await _pecaRepository.GetByIds(idsPecas, ct);

                if (pecasEntities.Count() != idsPecas.Count)
                    return new CommandResult<Guid> { StatusCode = HttpStatusCode.NotFound, Message = "Uma ou mais peças não foram encontradas." };

                var ordemPecas = pecasAgrupadas.Select(p => {
                    var valorUnitario = pecasEntities.First(e => e.Id == p.PecaId).ValorUnitario;
                    return new OrdemServicoPeca(entity.Id, p.PecaId, p.QuantidadeTotal, valorUnitario, Guid.Empty);
                }).ToList();

                entity.AlterarPeca(ordemPecas);

                var estoques = await _estoqueRepository.GetByPecaIds(idsPecas, ct);

                foreach(var peca in ordemPecas)
                {
                    var estoque = estoques.FirstOrDefault(e => e.PecaId == peca.PecaId);

                    if (estoque is not null)
                    {
                        estoque.ReservarEstoque(peca.Quantidade);
                    }
                }
            }
            
            if (request.Servicos is not null && request.Servicos.Any())
            {
                var servicosAgrupados = request.Servicos
                    .GroupBy(s => s.ServicoId)
                    .Select(g => new { ServicoId = g.Key, QuantidadeTotal = g.Sum(x => x.Quantidade) })
                    .ToList();

                var idsServicos = servicosAgrupados.Select(s => s.ServicoId).ToList();
                var servicosEntities = await _servicoRepository.GetByIds(idsServicos, ct);

                if (servicosEntities.Count() != idsServicos.Count)
                    return new CommandResult<Guid> { StatusCode = HttpStatusCode.NotFound, Message = "Um ou mais serviços não foram encontrados." };

                var ordemServicos = servicosAgrupados.Select(s => {
                    var valorUnitario = servicosEntities.First(e => e.Id == s.ServicoId).ValorUnitario;
                    return new OrdemServicoServico(entity.Id, s.ServicoId, s.QuantidadeTotal, valorUnitario, Guid.Empty);
                }).ToList();

                entity.AlterarServico(ordemServicos);
            }

            var ordemServicoId = await _ordemServicoRepository.Create(entity, ct);

            return new CommandResult<Guid> { StatusCode = HttpStatusCode.Created, Message = "Ordem de serviço criada com sucesso.", Data = ordemServicoId };
        }
        catch (ArgumentException ex)
        {
            return new CommandResult<Guid> { StatusCode = HttpStatusCode.BadRequest, Message = ex.Message };
        }
        catch (Exception ex)
        {
            return new CommandResult<Guid> { StatusCode = HttpStatusCode.InternalServerError, Message = $"Erro interno no servidor. Detalhes: {ex.Message}" };
        }
    }

    public async Task<ICommandResult<PagedResultDTO<OrdemServicoResponseDTO>>> GetPaginated(int page, int pageSize, CancellationToken ct)
    {
        try 
        {
            var ordemServico = await _ordemServicoRepository.GetPaginated(page, pageSize, ct);

            if(ordemServico.OrdemServicos is null || ordemServico.OrdemServicos.Count == 0)
                return new CommandResult<PagedResultDTO<OrdemServicoResponseDTO>> { StatusCode = HttpStatusCode.NotFound, Message = "Nenhuma ordem de serviço encontrada." };

            var response = ordemServico.OrdemServicos.ToListDTO();

            var pagedResult = new PagedResultDTO<OrdemServicoResponseDTO>
            {
                Items = response,
                Page = page,
                PageSize = pageSize,
                TotalItems = ordemServico.Total,
                TotalPages = (int)Math.Ceiling((double)ordemServico.Total / pageSize)
            };

            return new CommandResult<PagedResultDTO<OrdemServicoResponseDTO>> { StatusCode = HttpStatusCode.OK, Message = "Pesquisa de Ordens de Serviços retornada com sucesso.", Data = pagedResult };
        }
        catch (ArgumentException ex)
        {
            return new CommandResult<PagedResultDTO<OrdemServicoResponseDTO>> { StatusCode = HttpStatusCode.BadRequest, Message = ex.Message };
        }
        catch (Exception ex)
        {
            return new CommandResult<PagedResultDTO<OrdemServicoResponseDTO>> { StatusCode = HttpStatusCode.InternalServerError, Message = $"Erro interno no servidor. Detalhes: {ex.Message}" };
        }
    }
}
