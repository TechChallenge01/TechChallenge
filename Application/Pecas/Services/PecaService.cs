using Application.Pecas.DTOs.Requests;
using Application.Pecas.DTOs.Responses;
using Application.Pecas.Presenters;
using Domain.Entities;
using Domain.Entities.Repositories;
using Shared.Result;
using Shared.Result.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Net;
using System.Text;

namespace Application.Pecas.Services
{
    public class PecaService : IPecaService
    {
        private readonly IPecaRepository _pecaRepository;

        public PecaService(IPecaRepository pecaRepository)
        {
            _pecaRepository = pecaRepository;
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

                return new CommandResult<PagedResultDTO<PecaResponseDTO>> { StatusCode = HttpStatusCode.OK, Data = pagedResult, Message = "Pecas retornadas com sucesso!" };
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

        public async Task<ICommandResult<Guid>> Create(PecaRequestDTO request, CancellationToken ct)
        {
            try
            {
                var peca = new Peca(request.Nome, request.Descricao, request.MarcaPeca, request.PrecoVenda, Guid.Empty, DateTime.UtcNow);

                await _pecaRepository.Add(peca, ct);

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

        public async Task<ICommandResult> Delete(Guid id, CancellationToken ct)
        {
            try
            {
                var peca = await _pecaRepository.GetById(id, ct);

                if(peca is null)
                    return new CommandResult { StatusCode = HttpStatusCode.NotFound, Message = "Peça não encontrada." };
                peca.Inativar(Guid.Empty, DateTime.UtcNow);

                await _pecaRepository.Update(peca, ct);

                return new CommandResult { StatusCode = HttpStatusCode.OK, Message = "Peça excluída com sucesso!" };
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

        public async Task<ICommandResult> Update(Guid id, PecaRequestDTO request, CancellationToken ct)
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

                peca.RastrearAlteracao(Guid.Empty, DateTime.UtcNow);

                await _pecaRepository.Update(peca, ct);

                return new CommandResult { StatusCode = HttpStatusCode.OK, Message = "Peça atualizada com sucesso!" };
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
