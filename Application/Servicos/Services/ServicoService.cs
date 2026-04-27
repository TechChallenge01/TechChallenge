using Application.Servicos.DTOs.Requests;
using Application.Servicos.DTOs.Response;
using Application.Servicos.Presenters;
using Domain.Entities;
using Domain.Entities.Repositories;
using Domain.UnitOfWork;
using Shared.Result;
using Shared.Result.DTO;
using System.Net;

namespace Application.Servicos.Services;

public class ServicoService : IServicoService
{
    private readonly IServicoRepository _servico;
    private readonly IUnitOfWork _unitOfWork;
    public ServicoService(IServicoRepository servico, IUnitOfWork unitOfWork)
    {
        _servico = servico;
        _unitOfWork = unitOfWork;
    }
    public async Task<ICommandResult<Guid>> Create(ServicoRequestDTO request, CancellationToken ct)
    {
        try
        {
            var entityServico = new Servico(request.Nome, request.Descricao, request.PrecoVenda, Guid.Empty, DateTime.Now);

            await _servico.Create(entityServico, ct);

            return new CommandResult<Guid> { StatusCode = HttpStatusCode.Created, Message = "Serviço criado com sucesso.", Data = entityServico.Id };

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

    public async Task<ICommandResult> Delete(Guid Id, CancellationToken ct)
    {
        var servico = await _servico.GetById(Id, ct);

        if (servico is null)
            return new CommandResult<Guid> { StatusCode = HttpStatusCode.NotFound, Message = "Serviço não encontrado." };

        servico.Inativar();
        servico.RastrearAlteracao(Guid.Empty, DateTime.UtcNow);

        await _unitOfWork.SaveChangesAsync(ct);

        return new CommandResult<Guid> { StatusCode = HttpStatusCode.NoContent, Message = "Serviço deletado com sucesso." };

    }

    public async Task<ICommandResult<ServicoResponseDTO>> GetById(Guid Id, CancellationToken ct)
    {
        try 
        {
            var servico = await _servico.GetById(Id, ct);

            if (servico is null)
                return new CommandResult<ServicoResponseDTO> { StatusCode = HttpStatusCode.NotFound, Message = "Serviço não encontrado." };

            var response = servico.ToDto();

            return new CommandResult<ServicoResponseDTO> { StatusCode = HttpStatusCode.OK, Data = response, Message = "Serviço recuperado com sucesso." };
        }
        catch (ArgumentException ex)
        {
            return new CommandResult<ServicoResponseDTO> { StatusCode = HttpStatusCode.BadRequest, Message = ex.Message };
        }
        catch (Exception ex)
        {
            return new CommandResult<ServicoResponseDTO> { StatusCode = HttpStatusCode.InternalServerError, Message = $"Erro interno no servidor. Detalhes: {ex.Message}" };
        }
    }

    public async Task<ICommandResult<PagedResultDTO<ServicoResponseDTO>>> GetPaginated(int page, int pageSize, CancellationToken ct)
    {
        try
        {
            var servicos = await _servico.GetPaginatedList(page, pageSize, ct);

            if (servicos.servicos.Count == 0)
                return new CommandResult<PagedResultDTO<ServicoResponseDTO>> { StatusCode = HttpStatusCode.NoContent, Message = "Nenhum serviço encontrado." };

            var response = servicos.servicos.ToDtoList();

            var pagedResult = new PagedResultDTO<ServicoResponseDTO>
            {
                Items = response,
                Page = page,
                PageSize = pageSize,
                TotalItems = servicos.total,
                TotalPages = (int)Math.Ceiling(servicos.total / (double)pageSize)
            };

            return new CommandResult<PagedResultDTO<ServicoResponseDTO>> { StatusCode = HttpStatusCode.OK, Message = "Serviços recuperados com sucesso.", Data = pagedResult };
        }
        catch (ArgumentException ex)
        {
            return new CommandResult<PagedResultDTO<ServicoResponseDTO>> { StatusCode = HttpStatusCode.BadRequest, Message = ex.Message };
        }
        catch (Exception ex) 
        {
            return new CommandResult<PagedResultDTO<ServicoResponseDTO>> { StatusCode = HttpStatusCode.InternalServerError, Message = $"Erro interno no servidor. Detalhes: {ex.Message}" };
        }
    }

    public async Task<ICommandResult> Update(Guid Id, ServicoRequestDTO request, CancellationToken ct)
    {
        try
        {
            var servico = await _servico.GetById(Id, ct);

            if (servico is null)
                return new CommandResult<ICommandResult> { StatusCode = HttpStatusCode.NotFound, Message = "Serviço não encontrado." };

            servico.AlterarNome(request.Nome);
            servico.AlterarDescricao(request.Descricao);
            servico.AlterarPrecoVenda(request.PrecoVenda);

            servico.RastrearAlteracao(Guid.Empty, DateTime.UtcNow);

            await _unitOfWork.SaveChangesAsync(ct);

            return new CommandResult<ICommandResult> { StatusCode = HttpStatusCode.OK, Message = "Serviço atualizado com sucesso." };

        }
        catch (ArgumentException ex)
        {
            return new CommandResult<ICommandResult> { StatusCode = HttpStatusCode.BadRequest, Message = ex.Message };
        }
        catch (Exception ex)
        {
            return new CommandResult<ICommandResult> { StatusCode = HttpStatusCode.InternalServerError, Message = $"Erro interno no servidor. Detalhes: {ex.Message}" };
        }
    }
}
