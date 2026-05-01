using Application.OrdemServicos.DTOs.Requests;
using Application.OrdemServicos.DTOs.Responses;
using Application.OrdemServicos.Presenters;
using Application.UnitOfWork;
using Domain.Aggregates.ClienteAggregates;
using Domain.Aggregates.ClienteAggregates.Repositories;
using Domain.Aggregates.EstoqueAggregates;
using Domain.Aggregates.EstoqueAggregates.Repositories;
using Domain.Aggregates.OrdemServicoAggregates;
using Domain.Aggregates.OrdemServicoAggregates.Repositories;
using Domain.Entities.Repositories;
using Domain.Services;
using Domain.ValueObjects;
using Shared.DTOs;
using Shared.Result;
using System.Net;

namespace Application.OrdemServicos.Services;

public class OrdemServicoService : IOrdemServicoService
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IPecaRepository _pecaRepository;
    private readonly IServicoRepository _servicoRepository;
    private readonly IEstoqueRepository _estoqueRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    public OrdemServicoService(IOrdemServicoRepository ordemServicoRepository, IClienteRepository clienteRepository, IPecaRepository pecaRepository, IServicoRepository servicoRepository, IEstoqueRepository estoqueRepository, IUnitOfWork unitOfWork, IEmailService emailService)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _clienteRepository = clienteRepository;
        _pecaRepository = pecaRepository;
        _servicoRepository = servicoRepository;
        _estoqueRepository = estoqueRepository;
        _unitOfWork = unitOfWork;
        _emailService = emailService;
    }

    public async Task<ICommandResult> Aprovar(Guid id, CancellationToken ct)
    {
        try
        {
            var ordemServico = await _ordemServicoRepository.GetById(id, ct);

            if (ordemServico is null)
                return new CommandResult { StatusCode = HttpStatusCode.NotFound, Message = "Ordem de serviço não encontrada." };

            ordemServico.AprovarOrdemServico();

            ordemServico.RastrearAlteracao(Guid.Empty, DateTime.UtcNow);

            await _unitOfWork.SaveChangesAsync(ct);

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
    public async Task<ICommandResult> Cancelar(Guid id, CancellationToken ct)
    {
        try
        {
            var ordemServico = await _ordemServicoRepository.GetById(id, ct);

            if (ordemServico is null)
                return new CommandResult { StatusCode = HttpStatusCode.NotFound, Message = "Ordem de serviço não encontrada." };

            if (ordemServico.Pecas.Any())
            {
                var estoques = await _estoqueRepository.GetByPecaIds(ordemServico.Pecas.Select(x => x.PecaId).ToList(), ct);

                foreach (var peca in ordemServico.Pecas)
                {
                    var estoque = estoques.FirstOrDefault(e => e.PecaId == peca.PecaId);

                    if (estoque is not null)
                    {
                        estoque.LiberarReserva(peca.Quantidade, Guid.NewGuid());
                    }
                }
            }

            ordemServico.CancelarOrdemServico();

            ordemServico.RastrearAlteracao(Guid.Empty, DateTime.UtcNow);

            await _unitOfWork.SaveChangesAsync(ct);

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
    public async Task<ICommandResult> FinalizarServico(Guid id, FinalizarServicoDTO dto, CancellationToken ct)
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

            await _unitOfWork.SaveChangesAsync(ct);

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
            if (request.Cpf is null && request.Cnpj is null)
                return new CommandResult<Guid> { StatusCode = HttpStatusCode.BadRequest, Message = "CPF ou CNPJ deve ser informado." };

            if (request.Cpf is not null && request.Cnpj is not null)
                return new CommandResult<Guid> { StatusCode = HttpStatusCode.BadRequest, Message = "Não é possível informar ambos CPF e CNPJ do cliente!" };

            var isCpf = request.Cpf is not null;

            Cliente? cliente;

            if (isCpf)
            {
                var cpf = new Cpf(request.Cpf!);
                cliente = await _clienteRepository.GetByCpf(cpf, ct);
            }
            else
            {
                var cnpj = new Cnpj(request.Cnpj!);
                cliente = await _clienteRepository.GetByCnpj(cnpj, ct);
            }

            if (cliente is null)
                return new CommandResult<Guid> { StatusCode = HttpStatusCode.NotFound, Message = "Cliente não encontrado. Realize o cadastro antes de abrir uma OS." };

            if (!cliente.Veiculos.Any(v => v.Id == request.VeiculoId))
                return new CommandResult<Guid> { StatusCode = HttpStatusCode.NotFound, Message = "Veículo não encontrado." };

            var entity = new OrdemServico(cliente.Id, request.VeiculoId, Guid.Empty);

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

                var ordemPecas = pecasAgrupadas.Select(p =>
                {
                    var valorUnitario = pecasEntities.First(e => e.Id == p.PecaId).ValorUnitario;
                    return new OrdemServicoPeca(p.PecaId, p.QuantidadeTotal, valorUnitario, Guid.Empty);
                }).ToList();

                entity.AlterarPeca(ordemPecas);

                var estoques = await _estoqueRepository.GetByPecaIds(idsPecas, ct);

                foreach (var peca in ordemPecas)
                {
                    var estoque = estoques.FirstOrDefault(e => e.PecaId == peca.PecaId);

                    if (estoque is not null)
                    {
                        estoque.ReservarEstoque(peca.Quantidade, Guid.NewGuid());
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

                var ordemServicos = servicosAgrupados.Select(s =>
                {
                    var valorUnitario = servicosEntities.First(e => e.Id == s.ServicoId).ValorUnitario;
                    return new OrdemServicoServico(s.ServicoId, s.QuantidadeTotal, valorUnitario, Guid.Empty);
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

            if (ordemServico.OrdemServicos is null || ordemServico.OrdemServicos.Count == 0)
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
    public async Task<ICommandResult<OrdemServicoResponseDTO>> GetById(Guid id, CancellationToken ct)
    {
        try
        {
            var ordemServico = await _ordemServicoRepository.GetById(id, ct);

            if (ordemServico is null)
                return new CommandResult<OrdemServicoResponseDTO> { StatusCode = HttpStatusCode.NotFound, Message = "Ordem de serviço não encontrada." };

            var response = ordemServico.ToDTO();

            return new CommandResult<OrdemServicoResponseDTO> { StatusCode = HttpStatusCode.OK, Message = "Ordem de serviço retornada com sucesso.", Data = response };
        }
        catch (ArgumentException ex)
        {
            return new CommandResult<OrdemServicoResponseDTO> { StatusCode = HttpStatusCode.BadRequest, Message = ex.Message };
        }
        catch (Exception ex)
        {
            return new CommandResult<OrdemServicoResponseDTO> { StatusCode = HttpStatusCode.InternalServerError, Message = $"Erro interno no servidor. Detalhes: {ex.Message}" };
        }
    }
    public async Task<ICommandResult> RealizarDiagnostico(Guid id, DiagnosticoRequestDTO request, CancellationToken ct)
    {
        try
        {
            var ordemServico = await _ordemServicoRepository.GetById(id, ct);

            if (ordemServico is null)
                return new CommandResult { StatusCode = HttpStatusCode.NotFound, Message = "Ordem de serviço não encontrada." };

            var possuiServico = request.Servicos is not null && request.Servicos.Any();
            var possuiPeca = request.Pecas is not null && request.Pecas.Any();

            if (!possuiServico && !possuiPeca)
                return new CommandResult { StatusCode = HttpStatusCode.BadRequest, Message = "O diagnóstico deve conter ao menos um serviço ou uma peça."};

            var ordemPecas = new List<OrdemServicoPeca>();
            var estoques = new List<Estoque>();

            if (possuiPeca)
            {
                var pecasAgrupadas = request.Pecas
                    .GroupBy(p => p.PecaId)
                    .Select(g => new { PecaId = g.Key, QuantidadeTotal = g.Sum(x => x.Quantidade) })
                    .ToList();

                var idsPecas = pecasAgrupadas.Select(p => p.PecaId).ToList();
                var pecasEntities = await _pecaRepository.GetByIds(idsPecas, ct);

                if (pecasEntities.Count() != idsPecas.Count)
                    return new CommandResult<Guid> { StatusCode = HttpStatusCode.NotFound, Message = "Uma ou mais peças não foram encontradas." };

                estoques = (await _estoqueRepository.GetByPecaIds(idsPecas, ct)).ToList();

                ordemPecas = pecasAgrupadas.Select(p =>
                {
                    var valorUnitario = pecasEntities.First(e => e.Id == p.PecaId).ValorUnitario;
                    return new OrdemServicoPeca(p.PecaId, p.QuantidadeTotal, valorUnitario, Guid.Empty);
                }).ToList();
            }

            var ordemServicos = new List<OrdemServicoServico>();

            if (possuiServico)
            {
                var servicosAgrupados = request.Servicos
                    .GroupBy(s => s.ServicoId)
                    .Select(g => new { ServicoId = g.Key, QuantidadeTotal = g.Sum(x => x.Quantidade) })
                    .ToList();

                var idsServicos = servicosAgrupados.Select(s => s.ServicoId).ToList();
                var servicosEntities = await _servicoRepository.GetByIds(idsServicos, ct);

                if (servicosEntities.Count() != idsServicos.Count)
                    return new CommandResult<Guid> { StatusCode = HttpStatusCode.NotFound, Message = "Um ou mais serviços não foram encontrados." };

                ordemServicos = servicosAgrupados.Select(s =>
                {
                    var valorUnitario = servicosEntities.First(e => e.Id == s.ServicoId).ValorUnitario;
                    return new OrdemServicoServico(s.ServicoId, s.QuantidadeTotal, valorUnitario, Guid.Empty);
                }).ToList();
            }

            if (ordemServicos.Any())
                ordemServico.AlterarServico(ordemServicos);

            if (ordemPecas.Any())
                ordemServico.AlterarPeca(ordemPecas);

            ordemServico.RegistrarDiagnostico(request.Observacao ?? string.Empty);

            foreach (var peca in ordemPecas)
            {
                var estoque = estoques.First(e => e.PecaId == peca.PecaId);
                estoque.ReservarEstoque(peca.Quantidade, Guid.Empty);
            }

            ordemServico.RastrearAlteracao(Guid.Empty, DateTime.UtcNow);

            await _unitOfWork.SaveChangesAsync(ct);

            await EnviarEmail(ordemServico, ct);

            return new CommandResult {  StatusCode = HttpStatusCode.NoContent, Message = "Diagnóstico realizado. Status atualizado para Em Diagnóstico."};
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
    public async Task<ICommandResult> IniciarDiagnostico(Guid id, CancellationToken ct)
    {
        try
        { 
            var ordemServico = await _ordemServicoRepository.GetById(id, ct);

            if (ordemServico is null)
                return new CommandResult { StatusCode = HttpStatusCode.NotFound, Message = "Ordem de serviço não encontrada." };

            ordemServico.IniciarDiagnostico();
            ordemServico.RastrearAlteracao(Guid.Empty, DateTime.UtcNow);

            await _unitOfWork.SaveChangesAsync(ct);

            return new CommandResult { StatusCode = HttpStatusCode.NoContent, Message = "Diagnóstico iniciado com sucesso." };
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
    private async Task EnviarEmail(OrdemServico ordemServico, CancellationToken ct)
    {
        try
        {
            var orcamento = $"""
                            ORÇAMENTO

                            PEÇAS:
                            {string.Join("\n", ordemServico.Pecas.Select(p =>
                                    $"  {p.Quantidade}x {p.NomePeca} | Unit: {p.ValorUnitario:C} | Total: {p.ValorTotal:C}"))}

                            SERVIÇOS:
                            {string.Join("\n", ordemServico.Servicos.Select(s =>
                                    $"  {s.Quantidade}x {s.NomeServico} | Unit: {s.ValorUnitario:C} | Total: {s.ValorTotal:C}"))}

                            Desconto:    {ordemServico.ValorDesconto:C}
                            Valor Total: {ordemServico.ValorTotal:C}
                            """;

            var payloadEmail = new EmailPayloadDTO
            {
                To = ordemServico.Cliente.Emails.Select(e => e.ToString()).ToList(),
                Body = $"Olá {ordemServico.Cliente.Nome}, o diagnóstico da sua ordem de serviço (ID: {ordemServico.Id}) foi realizado. segue o orçamento:\n{orcamento}",
                Subject = "Orçamento da Ordem de Serviço"
            };

            await _emailService.Send(payloadEmail, ct);
        }
        catch (Exception ex)
        {
            // Log do erro de envio de email, mas não interrompe o fluxo principal
            Console.WriteLine($"Erro ao enviar email: {ex.Message}");
        }
    }
}
