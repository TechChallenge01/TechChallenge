using Application.Cliente.DTOs.Requests;
using Application.Cliente.DTOs.Responses;
using Application.Cliente.Presenters;
using Domain.Aggregates.Cliente.Repositories;
using Domain.Aggregates.Cliente.ValueObjects;
using Domain.Agregates.Cliente;
using Domain.Enums;
using Shared.Result;
using Shared.Result.DTO;
using System.Net;
using System.Xml;

namespace Application.Cliente.Services
{
    public class ClienteService(IClienteRepository _clienteRepository) : IClienteService
    {
        public async Task<ICommandResult<PagedResultDTO<ClienteResponseDTO>>> GetPaginated(int page, int pageSize, CancellationToken ct)
        {
            try
            {
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

                return new CommandResult<PagedResultDTO<ClienteResponseDTO>> { StatusCode = HttpStatusCode.OK, Message = "Pesquisa de clientes paginadas retornada com sucesso!", Data = pagedResult };
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
                var entity = new ClienteEntity(request.Nome, request.CpfCnpj, Guid.Empty);

                if (request.Emails.Count > 0)
                    entity.AdicionarEmail(request.Emails.Select(e => new Email(e)).ToList());


                if (request.Telefones.Count > 0)
                {
                    foreach (var telefone in request.Telefones)
                    {
                        if (!Enum.TryParse<ETipoTelefone>(telefone.Tipo, true, out var TipoEnum))
                            return new CommandResult<Guid> { StatusCode = HttpStatusCode.BadRequest, Message = "Tipo de telefone inválido!" };

                        telefone.Tipo = TipoEnum.ToString();
                    }
                    entity.AdicionarTelefone(request.Telefones.Select(t => new Telefone(t.DDD, t.DDI, t.Numero, (ETipoTelefone)Enum.Parse(typeof(ETipoTelefone), t.Tipo))).ToList());
                }

                if (request.Enderecos.Count > 0)
                {
                    entity.AdicionarEndereco(request.Enderecos.Select(e => new Endereco(e.Logradouro, e.Numero, e.Complemento, e.Bairro, e.Cidade, e.Uf, e.Cep)).ToList());
                }

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

                await _clienteRepository.Delete(cliente, ct);

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

                cliente.AdicionarEmail(request.Emails.Select(e => new Email(e)).ToList());

                foreach (var telefone in request.Telefones)
                {
                    if (!Enum.TryParse<ETipoTelefone>(telefone.Tipo, true, out var TipoEnum))
                        return new CommandResult<Guid> { StatusCode = HttpStatusCode.BadRequest, Message = "Tipo de telefone inválido!" };

                    telefone.Tipo = TipoEnum.ToString();
                }
                cliente.AdicionarTelefone(request.Telefones.Select(t => new Telefone(t.DDD, t.DDI, t.Numero, (ETipoTelefone)Enum.Parse(typeof(ETipoTelefone), t.Tipo))).ToList());

                cliente.AdicionarEndereco(request.Enderecos.Select(e => new Endereco(e.Logradouro, e.Numero, e.Complemento, e.Bairro, e.Cidade, e.Uf, e.Cep)).ToList());

                cliente.AlterarCpfCnpj(request.CpfCnpj);

                cliente.AlterarNome(request.Nome);

                await _clienteRepository.Update(cliente, ct);

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
