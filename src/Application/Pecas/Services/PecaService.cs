using Application.Pecas.Presenters;
using Application.Pecas.DTOs.Requests;
using Application.Pecas.DTOs.Responses;
using Domain.Aggregates.EstoqueAggregates;
using Domain.Aggregates.EstoqueAggregates.Repositories;
using Domain.Entities;
using Domain.Entities.Repositories;
using Shared.Result;
using System.Net;
using Application.UnitOfWork;
using Shared.DTOs;

namespace Application.Pecas.Services
{
    public class PecaService : IPecaService
    {
        private readonly IPecaRepository _pecaRepository;
        private readonly IEstoqueRepository _estoqueRepository;
        private readonly IUnitOfWork _unitOfWork;
        public PecaService(IPecaRepository pecaRepository, IEstoqueRepository estoqueRepository, IUnitOfWork unitOfWork)
        {
            _pecaRepository = pecaRepository;
            _estoqueRepository = estoqueRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<ICommandResult<PagedResultDTO<PecaResponseDTO>>> GetPaginated(int page, int pageSize, CancellationToken ct)
        {
            try
            {
                var pecas = await _pecaRepository.GetPaginated(page, pageSize, ct);

                var response = pecas.pecas.ToDtoList();

                var pagedResult = new PagedResultDTO<PecaResponseDTO>
                {
                    Items = response,
                    Page = page,
                    PageSize = pageSize,
                    TotalItems = pecas.total,
                    TotalPages = (int)Math.Ceiling((double)pecas.total / pageSize)
                };

                return new CommandResult<PagedResultDTO<PecaResponseDTO>> { StatusCode = HttpStatusCode.PartialContent, Data = pagedResult, Message = "Pecas retornadas com sucesso!" };
            }
            catch (ArgumentException ex)
            {
                return new CommandResult<PagedResultDTO<PecaResponseDTO>> { StatusCode = HttpStatusCode.BadRequest, Message = ex.Message };
            }
            catch (Exception ex)
            {
                return new CommandResult<PagedResultDTO<PecaResponseDTO>>    { StatusCode = HttpStatusCode.InternalServerError, Message = $"Erro interno no servidor. Detalhes: {ex.Message}" };
            }
        }
        public async Task<ICommandResult<Guid>> Create(PecaRequestDTO request, Guid idUsuario, CancellationToken ct)
        {
            try
            {
                var peca = new Peca(request.Nome, request.Descricao, request.MarcaPeca, request.PrecoVenda, idUsuario, DateTime.UtcNow);
                var estoque = new Estoque(null, peca.Id, 0, idUsuario, DateTime.UtcNow);

                await _pecaRepository.Create(peca, ct);
                await _estoqueRepository.Create(estoque, ct);

                return new CommandResult<Guid> { StatusCode = HttpStatusCode.Created, Data = peca.Id, Message = "Peça criada com sucesso!" };
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
        public async Task<ICommandResult> Delete(Guid id, Guid idUsuario, CancellationToken ct)
        {
            try
            {
                var peca = await _pecaRepository.GetById(id, ct);

                if(peca is null)
                    return new CommandResult { StatusCode = HttpStatusCode.NotFound, Message = "Peça não encontrada." };

                var estoque = await _estoqueRepository.GetByPecaId(id, ct);

                if (estoque.QuantidadeTotal > 0)
                    return new CommandResult { StatusCode = HttpStatusCode.BadRequest, Message = "Peça não pode ser excluida, pois contém quantidade em estoque!" };

                peca.Inativar();
                estoque.Inativar();
                peca.RastrearAlteracao(idUsuario, DateTime.UtcNow);

                await _unitOfWork.SaveChangesAsync(ct);

                return new CommandResult { StatusCode = HttpStatusCode.NoContent, Message = "Peça excluída com sucesso!" };
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
        public async Task<ICommandResult> Update(Guid id, Guid idUsuario, PecaRequestDTO request, CancellationToken ct)
        {
            try
            {
                var peca = await _pecaRepository.GetById(id, ct);

                if(peca is null)
                    return new CommandResult { StatusCode = HttpStatusCode.NotFound, Message = "Peça não encontrada." };

                peca.AlterarNome(request.Nome);
                peca.AlterarDescricao(request.Descricao);
                peca.AlterarPrecoVenda(request.PrecoVenda);
                peca.AlterarMarcaPeca(request.MarcaPeca);

                peca.RastrearAlteracao(idUsuario, DateTime.UtcNow);

                await _unitOfWork.SaveChangesAsync(ct);

                return new CommandResult { StatusCode = HttpStatusCode.NoContent, Message = "Peça atualizada com sucesso!" };
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
        public async Task<ICommandResult<PecaResponseDTO>> GetById(Guid id, CancellationToken ct)
        {
            try
            {
                var peca = await _pecaRepository.GetById(id, ct);

                if (peca is null)
                    return new CommandResult<PecaResponseDTO>{ StatusCode = HttpStatusCode.NotFound, Message = "Peça não encontrada." };

                var response = peca.ToDto();

                return new CommandResult<PecaResponseDTO> { StatusCode = HttpStatusCode.OK, Data = response, Message = "Peça retornada com sucesso!" };
            }
            catch (ArgumentException ex)
            {
                return new CommandResult<PecaResponseDTO> { StatusCode = HttpStatusCode.BadRequest, Message = ex.Message };
            }
            catch (Exception ex)
            {
                return new CommandResult<PecaResponseDTO> { StatusCode = HttpStatusCode.InternalServerError, Message = $"Erro interno no servidor. Detalhes: {ex.Message}" };
            }
        }
    }
}
