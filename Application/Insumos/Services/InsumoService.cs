using Application.Insumos.DTOs.Requests;
using Application.Insumos.DTOs.Responses;
using Application.Insumos.Presenters;
using Application.UnitOfWork;
using Domain.Entities.Repositories;
using Shared.DTOs;
using Shared.Result;
using System.Net;

namespace Application.Insumos.Services
{
    public class InsumoService : IInsumoService
    {
        private readonly IInsumoRepository _insumoRepository;
        private readonly IUnitOfWork _unitOfWork;

        public InsumoService(IInsumoRepository repository, IUnitOfWork unitOfWork)
        {
            _insumoRepository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ICommandResult<Guid>> Create(InsumoRequestDTO request, CancellationToken cancellationToken)
        {
            try
            {
                var insumo = new Insumo(request.Nome, request.Descricao, request.CustoUnitario, Guid.Empty, DateTime.UtcNow);

                await _insumoRepository.Create(insumo, cancellationToken);

                return new CommandResult<Guid> { StatusCode = HttpStatusCode.Created, Message = "Insumo criado com sucesso!", Data = insumo.Id };
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

        public async Task<ICommandResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var insumo = await _insumoRepository.GetById(id, cancellationToken);

                if (insumo is null)
                    return new CommandResult { StatusCode = HttpStatusCode.NotFound, Message = "Insumo não encontrado!" };

                insumo.Inativar();
                insumo.RastrearAlteracao(Guid.Empty, DateTime.UtcNow);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return new CommandResult { StatusCode = HttpStatusCode.NoContent, Message = "Insumo deletado com sucesso!" };
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

        public async Task<ICommandResult<InsumoResponseDTO>> GetById(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var insumo = await _insumoRepository.GetById(id, cancellationToken);

                if(insumo is null) 
                    return new CommandResult<InsumoResponseDTO> { StatusCode = HttpStatusCode.NotFound, Message = "Insumo não encontrado!" };

                var response = insumo.ToDto();

                return new CommandResult<InsumoResponseDTO> { StatusCode = HttpStatusCode.OK, Message = "Insumo retornado com sucesso!", Data = response };
            }
            catch (ArgumentException ex)
            {
                return new CommandResult<InsumoResponseDTO> { StatusCode = HttpStatusCode.BadRequest, Message = ex.Message };
            }
            catch (Exception ex)
            {
                return new CommandResult<InsumoResponseDTO> { StatusCode = HttpStatusCode.InternalServerError, Message = $"Erro interno no servidor. Detalhes: {ex.Message}" };
            }
        }

        public async Task<ICommandResult<PagedResultDTO<InsumoResponseDTO>>> GetPaginated(int page, int pageSize, CancellationToken cancellationToken)
        {
            try
            {
                var insumos = await _insumoRepository.GetPaginatedAsync(page, pageSize, cancellationToken);

                var response = insumos.insumos.ToDtoList();

                var pagedResult = new PagedResultDTO<InsumoResponseDTO>
                {
                    Items = response,
                    Page = page,
                    PageSize = pageSize,
                    TotalItems = insumos.total,
                    TotalPages = (int)Math.Ceiling((double)insumos.total / pageSize)
                };

                return new CommandResult<PagedResultDTO<InsumoResponseDTO>> { StatusCode = HttpStatusCode.OK, Message = "Insumos retornados com sucesso!", Data = pagedResult };
            }
            catch (ArgumentException ex)
            {
                return new CommandResult<PagedResultDTO<InsumoResponseDTO>> { StatusCode = HttpStatusCode.BadRequest, Message = ex.Message };
            }
            catch (Exception ex)
            {
                return new CommandResult<PagedResultDTO<InsumoResponseDTO>> { StatusCode = HttpStatusCode.InternalServerError, Message = $"Erro interno no servidor. Detalhes: {ex.Message}" };
            }
        }

        public async Task<ICommandResult> Update(Guid id, InsumoRequestDTO request, CancellationToken cancellationToken)
        {
            try
            {
                var insumo = await _insumoRepository.GetById(id, cancellationToken);

                if (insumo is null)
                    return new CommandResult { StatusCode = HttpStatusCode.NotFound, Message = "Insumo não encontrado!" };

                insumo.AtualizarNome(request.Nome);
                insumo.AtualizarCusto(request.CustoUnitario);
                insumo.AtualizarDescricao(request.Descricao);

                insumo.RastrearAlteracao(Guid.Empty, DateTime.UtcNow);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return new CommandResult { StatusCode = HttpStatusCode.NoContent, Message = "Insumo atualizado com sucesso!" };
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
    }
}
