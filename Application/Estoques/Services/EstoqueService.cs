using Application.Estoques.DTOs.Requests;
using Application.Estoques.DTOs.Responses;
using Application.Estoques.Presenters;
using Application.UnitOfWork;
using Domain.Aggregates.EstoqueAggregates;
using Domain.Aggregates.EstoqueAggregates.Repositories;
using Domain.Entities.Repositories;
using Domain.Enums;
using Shared.DTOs;
using Shared.Result;
using System.Net;

namespace Application.Estoques.Services
{
    public class EstoqueService : IEstoqueService
    {
        private readonly IEstoqueRepository _estoqueRepository;
        private readonly IPecaRepository _pecaRepository;
        private readonly IInsumoRepository _insumoRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EstoqueService(IEstoqueRepository estoqueRepository, IPecaRepository pecaRepository, IUnitOfWork unitOfWork, IInsumoRepository insumoRepository)
        {
            _estoqueRepository = estoqueRepository;
            _pecaRepository = pecaRepository;
            _unitOfWork = unitOfWork;
            _insumoRepository = insumoRepository;
        }

        public async Task<ICommandResult<PagedResultDTO<EstoqueResponseDTO>>> GetPaginated(int page, int pageSize, CancellationToken ct)
        {
            try
            {
                var estoque = await _estoqueRepository.GetPaginated(page, pageSize, ct);

                var response = estoque.estoques.ToDTOList();

                var pagedResult = new PagedResultDTO<EstoqueResponseDTO>
                {
                    Items = response,
                    Page = page,
                    PageSize = pageSize,
                    TotalItems = estoque.total,
                    TotalPages = (int)Math.Ceiling((double)estoque.total / pageSize)
                };

                return new CommandResult<PagedResultDTO<EstoqueResponseDTO>> { StatusCode = HttpStatusCode.OK, Data = pagedResult , Message = "Estoques retornados com sucesso!"};
            }
            catch (ArgumentException ex)
            {
                return new CommandResult<PagedResultDTO<EstoqueResponseDTO>> { StatusCode = HttpStatusCode.BadRequest, Message = ex.Message };
            }
            catch (Exception ex)
            {
                return new CommandResult<PagedResultDTO<EstoqueResponseDTO>> { StatusCode = HttpStatusCode.InternalServerError, Message = $"Erro interno no servidor. Detalhes: {ex.Message}" };
            }
        }

        public async Task<ICommandResult<EstoqueResponseDTO>> GetById(Guid id, CancellationToken ct)
        {
            try
            {
                var estoque = await _estoqueRepository.GetById(id, ct);

                if (estoque is null)
                    return new CommandResult<EstoqueResponseDTO> { StatusCode = HttpStatusCode.NotFound, Message = "Estoque não encontrado" };

                var response = estoque.ToDTO();

                return new CommandResult<EstoqueResponseDTO> { StatusCode = HttpStatusCode.OK, Data = response, Message = "Estoque retornado com sucesso!" };
            }
            catch (ArgumentException ex)
            {
                return new CommandResult<EstoqueResponseDTO> { StatusCode = HttpStatusCode.BadRequest, Message = ex.Message };
            }
            catch (Exception ex)
            {
                return new CommandResult<EstoqueResponseDTO> { StatusCode = HttpStatusCode.InternalServerError, Message = $"Erro interno no servidor. Detalhes: {ex.Message}" };
            }
        }

        public async Task<ICommandResult<Guid>> Movimetar(EstoqueRequestDTO request, CancellationToken ct)
        {
            try
            {
                bool entrada = false;
                bool isInsumo = false;

                if (request.InsumoId is null && request.PecaId is null)
                    return new CommandResult<Guid> { StatusCode = HttpStatusCode.BadRequest, Message = "É obrigatório ter a PecaId ou o InsumoId preenchidos" };

                if (request.InsumoId is not null && request.PecaId is not null)
                    return new CommandResult<Guid> { StatusCode = HttpStatusCode.BadRequest, Message = "Apenas uma opção é para ser preenchida, ou PecaId ou InsumoId!" };

                if (!Enum.TryParse<ETipoMovimentacao>(request.TipoMovimentacao, true, out var tipoMovimentacao))
                    return new CommandResult<Guid> { StatusCode = HttpStatusCode.BadRequest, Message = "Tipo de movimentação inválido!" };

                if (request.InsumoId is not null)
                    isInsumo = true;

                Estoque estoque;

                if (isInsumo)
                {
                    var insumo = await _insumoRepository.GetById((Guid)request.InsumoId, ct);

                    if (insumo is null)
                        return new CommandResult<Guid> { StatusCode = HttpStatusCode.NotFound, Message = "Insumo não encontrada" };

                    estoque = await _estoqueRepository.GetByInsumoId((Guid)request.InsumoId, ct);
                }
                else
                {
                    var peca = await _pecaRepository.GetById((Guid)request.PecaId, ct);

                    if (peca is null)
                        return new CommandResult<Guid> { StatusCode = HttpStatusCode.NotFound, Message = "Peça não encontrada" };

                    estoque = await _estoqueRepository.GetByPecaId((Guid)request.PecaId, ct);
                }

                if(tipoMovimentacao == ETipoMovimentacao.Entrada)
                    entrada = true;

                if(estoque is null)
                    return new CommandResult<Guid> { StatusCode = HttpStatusCode.NotFound, Message = "Estoque para a peça ou insumo informado não encontrado!" };


                if (entrada)
                    estoque.AdicionarEstoque(request.Quantidade, Guid.Empty);
                else
                    estoque.RetirarEstoque(request.Quantidade, Guid.Empty);


                estoque.RastrearAlteracao(Guid.Empty, DateTime.UtcNow);

                await _unitOfWork.SaveChangesAsync(ct);

                return new CommandResult<Guid> { StatusCode = HttpStatusCode.Created, Data = estoque.Id, Message = "Movimentação realizada com sucesso!" };
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
    }
}
