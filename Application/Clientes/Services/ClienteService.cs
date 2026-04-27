using Application.Clientes.DTOs.Requests;
using Application.Clientes.DTOs.Responses;
using Application.Clientes.Presenters;
using Domain.Aggregates.ClienteAggregates;
using Domain.Aggregates.ClienteAggregates.Repositories;
using Domain.Enums;
using Domain.UnitOfWork;
using Domain.ValueObjects;
using Shared.Result;
using Shared.Result.DTO;
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
                var clientes = await _clienteRepository.GetPaginated(page, pageSize, ct);

                if (clientes.Clientes.Count == 0)
                    return new CommandResult<PagedResultDTO<ClienteResponseDTO>> { StatusCode = HttpStatusCode.NotFound, Message = "Nenhum cliente encontrado." };

                var response = clientes.Clientes.ToListDTO();

                var pagedResult = new PagedResultDTO<ClienteResponseDTO>
                {
                    Items = response,
                    Page = page,
                    PageSize = pageSize,
                    TotalItems = clientes.Total,
                    TotalPages = (int)Math.Ceiling(clientes.Total / (double)pageSize)
                };

                return new CommandResult<PagedResultDTO<ClienteResponseDTO>> { StatusCode = HttpStatusCode.OK, Message = "Pesquisa de clientes retornada com sucesso!", Data = pagedResult };
            }
            catch(ArgumentException ex)
            {
                return new CommandResult<PagedResultDTO<ClienteResponseDTO>>{ StatusCode = HttpStatusCode.BadRequest, Message = ex.Message};
            }
            catch(Exception ex)
            {
                return new CommandResult<PagedResultDTO<ClienteResponseDTO>> { StatusCode = HttpStatusCode.InternalServerError, Message = $"Erro interno no servidor. Detalhes: {ex.Message}"};
            }
        }

        public async Task<ICommandResult<Guid>> Create(ClienteRequestDTO request, CancellationToken ct)
        {
            try
            {
                if(request.Cpf is null && request.Cnpj is null)
                    return new CommandResult<Guid> { StatusCode = HttpStatusCode.BadRequest, Message = "É necessário informar ou o CPF ou o CNPJ do cliente!" };
                else if(request.Cpf is not null && request.Cnpj is not null)
                    return new CommandResult<Guid> { StatusCode = HttpStatusCode.BadRequest, Message = "Não é possível informar ambos CPF e CNPJ do cliente!" };

                Cliente entity;
                var isCpf = request.Cpf is not null;


                if (request.Telefones.Any(t => !Enum.TryParse<ETipoTelefone>(t.Tipo, true, out _)))
                {
                    return new CommandResult<Guid> { StatusCode = HttpStatusCode.BadRequest, Message = "Tipo de telefone inválido!" };
                }

                if (isCpf)
                    entity = new Cliente(request.Nome, new Cpf(request.Cpf), Guid.Empty);
                else
                    entity = new Cliente(request.Nome, new Cnpj(request.Cnpj), Guid.Empty);

                var telefones = request.Telefones.Select(t => new Telefone(t.DDD, t.DDI, t.Numero, (ETipoTelefone)Enum.Parse(typeof(ETipoTelefone), t.Tipo), entity.Id)).ToList();
                var enderecos = request.Enderecos.Select(e => new Endereco(e.Logradouro, e.Numero, e.Complemento, e.Bairro, e.Cidade, e.Uf, e.Cep, entity.Id)).ToList();
                var emails = request.Emails.Select(e => new Email(e, entity.Id)).ToList();

                entity.AlterarTelefones(telefones);
                entity.AlterarEnderecos(enderecos);
                entity.AlterarEmails(emails);

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

        public async Task<ICommandResult> Delete(Guid id, CancellationToken ct)
        {
            try
            {
                var cliente = await _clienteRepository.GetById(id, ct);

                if(cliente is null)
                    return new CommandResult { StatusCode = HttpStatusCode.NotFound, Message = "Cliente não encontrado!" };

                //exclusão lógica
                cliente.Inativar();

                cliente.RastrearAlteracao(Guid.Empty, DateTime.UtcNow);

                await _unitOfWork.SaveChangesAsync(ct);
                
                return new CommandResult { StatusCode = HttpStatusCode.NoContent, Message = "Cliente excluído com sucesso!" };
            }
            catch(ArgumentException ex)
            {
                return new CommandResult { StatusCode = HttpStatusCode.BadRequest, Message = ex.Message };
            }
            catch(Exception ex)
            {
                return new CommandResult { StatusCode = HttpStatusCode.InternalServerError, Message = $"Erro interno no servidor. Detalhes: {ex.Message}" };
            }
        }

        public async Task<ICommandResult> Update(Guid id, ClienteRequestDTO request, CancellationToken ct)
        {
            try
            {
                var cliente = await _clienteRepository.GetById(id, ct);

                if(cliente is null)
                    return new CommandResult { StatusCode = HttpStatusCode.NotFound, Message = "Cliente não encontrado!" };

                cliente.AlterarEmails(request.Emails.Select(e => new Email(e, cliente.Id)).ToList());

                if (request.Telefones.Any(t => !Enum.TryParse<ETipoTelefone>(t.Tipo, true, out _)))
                {
                    return new CommandResult<Guid> { StatusCode = HttpStatusCode.BadRequest, Message = "Tipo de telefone inválido!" };
                }

                cliente.AlterarTelefones(request.Telefones.Select(t => new Telefone(t.DDD, t.DDI, t.Numero, (ETipoTelefone)Enum.Parse(typeof(ETipoTelefone), t.Tipo), cliente.Id)).ToList());

                if(request.Enderecos.Count > 0)
                    cliente.AlterarEnderecos(request.Enderecos.Select(e => new Endereco(e.Logradouro, e.Numero, e.Complemento, e.Bairro, e.Cidade, e.Uf, e.Cep, cliente.Id)).ToList());

                cliente.AlterarNome(request.Nome);

                cliente.RastrearAlteracao(Guid.Empty, DateTime.UtcNow);

                await _unitOfWork.SaveChangesAsync(ct);

                return new CommandResult { StatusCode = HttpStatusCode.NoContent, Message = "Cliente atualizado com sucesso!" };
            }
            catch(ArgumentException ex)
            {
                return new CommandResult { StatusCode = HttpStatusCode.BadRequest, Message = ex.Message };
            }
            catch(Exception ex)
            {
                return new CommandResult { StatusCode = HttpStatusCode.InternalServerError, Message = $"Erro interno no servidor. Detalhes: {ex.Message}" };
            }
        }
    }
}
