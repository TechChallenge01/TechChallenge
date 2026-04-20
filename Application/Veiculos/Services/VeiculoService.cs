using Application.Veiculos.DTOs.Response;
using Application.Veiculos.Presenters;
using Domain.Entities.Repositories;
using Shared.Result;
using Shared.Result.DTO;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Application.Veiculos.Services
{
    public class VeiculoService : IVeiculoService
    {
        private readonly IVeiculoRepository _veiculoRepository;

        public VeiculoService(IVeiculoRepository veiculoRepository)
        {
            _veiculoRepository = veiculoRepository;
        }

        public async Task<ICommandResult<PagedResultDTO<VeiculoResponseDTO>>> GetPaginated(int page, int pageSize, CancellationToken ct)
        {
            try
            {
                var veiculos = await _veiculoRepository.GetPaginated(page, pageSize, ct);

                if(veiculos.veiculos.Count == 0)
                    return new CommandResult<PagedResultDTO<VeiculoResponseDTO>> { StatusCode = HttpStatusCode.NotFound, Message = "Nenhum veículo encontrado." };

                var response = veiculos.veiculos.ToDtoList();

                var pagedResult = new PagedResultDTO<VeiculoResponseDTO>
                {
                    Items = response,
                    Page = page,
                    PageSize = pageSize,
                    TotalItems = veiculos.total,
                    TotalPages = (int)Math.Ceiling((double)veiculos.total / pageSize)
                };

                return new CommandResult<PagedResultDTO<VeiculoResponseDTO>> { StatusCode = HttpStatusCode.OK, Message = "Pesquisa de veiculos retornada com sucesso! ", Data = pagedResult };

            }
            catch (ArgumentException ex)
            {
                return new CommandResult<PagedResultDTO<VeiculoResponseDTO>> { StatusCode = HttpStatusCode.BadRequest, Message = ex.Message };
            }
            catch (Exception ex)
            {
                return new CommandResult<PagedResultDTO<VeiculoResponseDTO>> { StatusCode = HttpStatusCode.InternalServerError, Message = $"Erro interno no servidor. Detalhes: {ex.Message}" };
            }
        }
    }
}
