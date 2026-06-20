using Application.Gateways.Clientes;
using Domain.Aggregates.ClienteAggregates;
using Domain.ValueObjects;
using Shared.DTOs.Clientes.Request;

namespace Application.UseCases.OrdensServicos
{
    public class ValidarClienteOrdemServicoUseCase
    {
        private readonly ClienteGateway _clienteGateway;

        private ValidarClienteOrdemServicoUseCase(ClienteGateway clienteGateway)
        {
            _clienteGateway = clienteGateway;
        }

        public static ValidarClienteOrdemServicoUseCase Create(ClienteGateway clienteGateway)
        {
            return new ValidarClienteOrdemServicoUseCase(clienteGateway);
        }

        public async Task<Cliente> Run(ClienteRequestDTO clienteRequest, Guid idUsuario, CancellationToken ct)
        {
            try
            {
                bool isCpf = false;
                Cliente cliente;

                if (string.IsNullOrEmpty(clienteRequest.Cpf) && string.IsNullOrEmpty(clienteRequest.Cnpj))
                    throw new ArgumentException("CPF e CNPJ não poodem ser ambos nulos!");

                if (!string.IsNullOrEmpty(clienteRequest.Cpf) && !string.IsNullOrEmpty(clienteRequest.Cnpj))
                    throw new ArgumentException("CPF e CNPJ não poodem ser ambos preenchidos!");

                if (!string.IsNullOrEmpty(clienteRequest.Cpf)) isCpf = true;

                if (isCpf)
                    cliente = await _clienteGateway.GetByCpf(new Cpf(clienteRequest.Cpf), ct);
                else
                    cliente = await _clienteGateway.GetByCnpj(new Cnpj(clienteRequest.Cnpj), ct);

                if (cliente is not null)
                {
                    if (clienteRequest.Nome.ToLower().Trim() != cliente.Nome.ToLower().Trim())
                        throw new ArgumentException("CPF ou CNPJ já cadastrados, porém com nome diferente");

                    return cliente;
                }
                else
                {
                    if (isCpf)
                        cliente = new Cliente(clienteRequest.Nome, new Cpf(clienteRequest.Cpf), idUsuario,
                                    new Endereco(clienteRequest.Endereco.Logradouro, clienteRequest.Endereco.Numero, cliente.Endereco.Complemento, cliente.Endereco.Bairro, cliente.Endereco.Cidade, cliente.Endereco.Uf, cliente.Endereco.Cep),
                                    new Telefone(clienteRequest.Telefone.DDD, clienteRequest.Telefone.DDI, clienteRequest.Telefone.Numero),
                                    new Email(clienteRequest.Email));

                    cliente = new Cliente(clienteRequest.Nome, new Cnpj(clienteRequest.Cnpj), idUsuario,
                                    new Endereco(clienteRequest.Endereco.Logradouro, clienteRequest.Endereco.Numero, cliente.Endereco.Complemento, cliente.Endereco.Bairro, cliente.Endereco.Cidade, cliente.Endereco.Uf, cliente.Endereco.Cep),
                                    new Telefone(clienteRequest.Telefone.DDD, clienteRequest.Telefone.DDI, clienteRequest.Telefone.Numero),
                                    new Email(clienteRequest.Email));

                    await _clienteGateway.Create(cliente, ct);
                    return cliente;
                }

            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
