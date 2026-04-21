using Application.Veiculos.DTOs.Requests;
using Application.Veiculos.DTOs.Response;
using Application.Veiculos.Presenters;
using Domain.Aggregates.ClienteAggregates.Repositories;
using Domain.Entities;
using Domain.Entities.Repositories;
using Shared.Result;
using Shared.Result.DTO;
using System.Net;

namespace Application.Veiculos.Services
{
    public class VeiculoService : IVeiculoService
    {
        private readonly IVeiculoRepository _veiculoRepository;
        private readonly IClienteRepository _clienteRepository;

        public VeiculoService(IVeiculoRepository veiculoRepository, IClienteRepository clienteRepository)
        {
            _veiculoRepository = veiculoRepository;
            _clienteRepository = clienteRepository;
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

        public async Task<ICommandResult<Guid>> Create(VeiculoRequestDTO request, CancellationToken ct)
        {
            try
            {
                var cliente = await _clienteRepository.GetById(request.ClienteId, ct);

                if(cliente is null)
                    return new CommandResult<Guid> { StatusCode = HttpStatusCode.NotFound, Message = "Cliente não encontrado." };

                var entity = new Veiculo(request.Modelo, request.MarcaVeiculo, request.ClienteId, request.Ano, request.Placa, request.Cor, Guid.Empty);

                await _veiculoRepository.Add(entity, ct);

                return new CommandResult<Guid> { StatusCode = HttpStatusCode.Created, Message = "Veículo criado com sucesso.", Data = entity.Id };
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
            try
            {
                var veiculo = await _veiculoRepository.GetById(Id, ct);

                if(veiculo is null)
                    return new CommandResult { StatusCode = HttpStatusCode.NotFound, Message = "Veículo não encontrado." };

                //Exclusão lógica
                veiculo.Inativar(Guid.Empty, DateTime.UtcNow);

                await _veiculoRepository.Update(veiculo, ct);

                return new CommandResult { StatusCode = HttpStatusCode.NoContent, Message = "Veículo deletado com sucesso." };

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

        public async Task<ICommandResult> Update(Guid Id, VeiculoRequestDTO request, CancellationToken ct)
        {
            try
            {
                var veiculo = await _veiculoRepository.GetById(Id, ct);
                var cliente = await _clienteRepository.GetById(request.ClienteId, ct);

                if (veiculo is null)
                    return new CommandResult { StatusCode = HttpStatusCode.NotFound, Message = "Veículo não encontrado." };

                if(cliente is null)
                    return new CommandResult { StatusCode = HttpStatusCode.NotFound, Message = "Cliente não encontrado." };

                veiculo.AlterarModelo(request.Modelo);
                veiculo.AlterarMarcaVeiculo(request.MarcaVeiculo);
                veiculo.AlterarAno(request.Ano);
                veiculo.AlterarCor(request.Cor);
                veiculo.AlterarCliente(request.ClienteId);

                veiculo.RastrearAlteracao(Guid.Empty, DateTime.UtcNow);

                await _veiculoRepository.Update(veiculo, ct);

                return new CommandResult { StatusCode = HttpStatusCode.OK, Message = "Veículo atualizado com sucesso." };
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
