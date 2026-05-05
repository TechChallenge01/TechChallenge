using Application.Clientes.DTOs.Requests;
using Application.Clientes.DTOs.Responses;
using Application.Clientes.Presenters;
using Application.UnitOfWork;
using Domain.Aggregates.ClienteAggregates;
using Domain.Aggregates.ClienteAggregates.Repositories;
using Domain.ValueObjects;
using Shared.DTOs;
using Shared.Result;
using System.Net;

namespace Application.Clientes.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ClienteService(IClienteRepository clienteRepository, IUnitOfWork unitOfWork)
        {
            _clienteRepository = clienteRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ICommandResult<PagedResultDTO<ClienteResponseDTO>>> GetPaginated(int page, int pageSize, CancellationToken ct)
        {
            try
            {
                if (page <= 0 || pageSize <= 0)
                {
                    return new CommandResult<PagedResultDTO<ClienteResponseDTO>>{
                        StatusCode = HttpStatusCode.BadRequest, Message = "A página e o tamanho da página devem ser maiores que zero."};
                }

                var clientes = await _clienteRepository.GetPaginated(page, pageSize, ct);

                var response = clientes.Clientes.ToListDTO();

                var pagedResult = new PagedResultDTO<ClienteResponseDTO>
                {
                    Items = response,
                    Page = page,
                    PageSize = pageSize,
                    TotalItems = clientes.Total,
                    TotalPages = (int)Math.Ceiling(clientes.Total / (double)pageSize)
                };

                return new CommandResult<PagedResultDTO<ClienteResponseDTO>> { StatusCode = HttpStatusCode.PartialContent, Message = "Pesquisa de clientes retornada com sucesso!", Data = pagedResult };
            }
            catch (ArgumentException ex)
            {
                return new CommandResult<PagedResultDTO<ClienteResponseDTO>> { StatusCode = HttpStatusCode.BadRequest, Message = ex.Message };
            }
            catch (Exception ex)
            {
                return new CommandResult<PagedResultDTO<ClienteResponseDTO>> { StatusCode = HttpStatusCode.InternalServerError, Message = $"Erro interno no servidor. Detalhes: {ex.Message}" };
            }
        }

        public async Task<ICommandResult<Guid>> Create(ClienteRequestDTO request, Guid idUsuario, CancellationToken ct)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Cpf) && string.IsNullOrEmpty(request.Cnpj))
                    return new CommandResult<Guid> { StatusCode = HttpStatusCode.BadRequest, Message = "É necessário informar ou o CPF ou o CNPJ do cliente!" };
                else if (!string.IsNullOrEmpty(request.Cpf) && !String.IsNullOrEmpty(request.Cnpj))
                    return new CommandResult<Guid> { StatusCode = HttpStatusCode.BadRequest, Message = "Não é possível informar ambos CPF e CNPJ do cliente!" };

                var isCpf = !string.IsNullOrEmpty(request.Cpf);

                if (isCpf)
                {
                    var cliente = await _clienteRepository.GetByCpf(new Cpf(request.Cpf), ct);
                    if (cliente is not null)
                        return new CommandResult<Guid> { StatusCode = HttpStatusCode.Conflict, Message = "CPF já cadastrado em outro cliente" };
                }
                else
                {
                    var cliente = await _clienteRepository.GetByCnpj(new Cnpj(request.Cnpj), ct);
                    if (cliente is not null)
                        return new CommandResult<Guid> { StatusCode = HttpStatusCode.Conflict, Message = "Cnpj já cadastrado em outro cliente" };
                }

                var Endereco = new Endereco(request.Endereco.Logradouro, request.Endereco.Numero, request.Endereco.Complemento, request.Endereco.Bairro, request.Endereco.Cidade, request.Endereco.Uf, request.Endereco.Cep);
                var Telefone = new Telefone(request.Telefone.DDD, request.Telefone.DDI, request.Telefone.Numero);

                Cliente entity;

                if (isCpf)
                    entity = new Cliente(request.Nome, new Cpf(request.Cpf), idUsuario, Endereco, Telefone, new Email(request.Email));
                else
                    entity = new Cliente(request.Nome, new Cnpj(request.Cnpj), idUsuario, Endereco, Telefone, new Email(request.Email));

                await _clienteRepository.Create(entity, ct);

                return new CommandResult<Guid> { StatusCode = HttpStatusCode.Created, Message = "Cliente criado com sucesso!", Data = entity.Id };
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
                var cliente = await _clienteRepository.GetById(id, ct);

                if (cliente is null)
                    return new CommandResult { StatusCode = HttpStatusCode.NotFound, Message = "Cliente não encontrado!" };

                //exclusão lógica
                cliente.Inativar();

                cliente.RastrearAlteracao(idUsuario, DateTime.UtcNow);

                await _unitOfWork.SaveChangesAsync(ct);

                return new CommandResult { StatusCode = HttpStatusCode.NoContent, Message = "Cliente excluído com sucesso!" };
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

        public async Task<ICommandResult> Update(Guid id, Guid idUsuario, ClienteRequestDTO request, CancellationToken ct)
        {
            try
            {
                var cliente = await _clienteRepository.GetById(id, ct);

                if (cliente is null)
                    return new CommandResult { StatusCode = HttpStatusCode.NotFound, Message = "Cliente não encontrado!" };

                var Endereco = new Endereco(request.Endereco.Logradouro, request.Endereco.Numero, request.Endereco.Complemento, request.Endereco.Bairro, request.Endereco.Cidade, request.Endereco.Uf, request.Endereco.Cep);
                var Telefone = new Telefone(request.Telefone.DDD, request.Telefone.DDI, request.Telefone.Numero);

                cliente.AlterarEmail(new Email(request.Email));

                cliente.AlterarEndereco(Endereco);

                cliente.AlterarTelefone(Telefone);

                cliente.AlterarNome(request.Nome);

                cliente.RastrearAlteracao(idUsuario, DateTime.UtcNow);

                await _unitOfWork.SaveChangesAsync(ct);

                return new CommandResult { StatusCode = HttpStatusCode.NoContent, Message = "Cliente atualizado com sucesso!" };
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

        public async Task<ICommandResult<ClienteResponseDTO>> GetById(Guid id, CancellationToken ct)
        {
            try
            {
                var cliente = await _clienteRepository.GetById(id, ct);

                if (cliente is null)
                    return new CommandResult<ClienteResponseDTO> { StatusCode = HttpStatusCode.NotFound, Message = "Cliente não encontrado!" };

                var response = cliente.ToDto();

                return new CommandResult<ClienteResponseDTO> { StatusCode = HttpStatusCode.OK, Data = response, Message = "Cliente encontrado com sucesso!" };

            }
            catch (ArgumentException ex)
            {
                return new CommandResult<ClienteResponseDTO> { StatusCode = HttpStatusCode.BadRequest, Message = ex.Message };
            }
            catch (Exception ex)
            {
                return new CommandResult<ClienteResponseDTO> { StatusCode = HttpStatusCode.InternalServerError, Message = $"Erro interno no servidor. Detalhes: {ex.Message}" };
            }
        }
    }
}
