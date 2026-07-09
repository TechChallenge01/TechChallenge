using Application.Gateways.Clientes;
using Domain.Aggregates.ClienteAggregates;
using Domain.ValueObjects;
using Shared.DTOs.Clientes.Request;

namespace Application.UseCases.Clientes
{
    public class CreateUseCase
    {
        private readonly ClienteGateway _clienteGateway;

        private CreateUseCase(ClienteGateway clienteGateway)
        {
            _clienteGateway = clienteGateway;
        }

        public static CreateUseCase Create(ClienteGateway clienteGateway)
        {
            return new CreateUseCase(clienteGateway);
        }

        public async Task<Guid> Run(ClienteRequestDTO request, Guid idUsuario, CancellationToken ct)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Cpf) && string.IsNullOrEmpty(request.Cnpj))
                    throw new ArgumentException("É necessário informar ou o CPF ou o CNPJ do cliente!");
                else if (!string.IsNullOrEmpty(request.Cpf) && !String.IsNullOrEmpty(request.Cnpj))
                    throw new ArgumentException("Não é possível informar ambos CPF e CNPJ do cliente!");

                var isCpf = !string.IsNullOrEmpty(request.Cpf);

                if (isCpf)
                {
                    var clienteValidation = await _clienteGateway.GetByCpf(new Cpf(request.Cpf), ct);
                    if (clienteValidation is not null)
                        throw new InvalidOperationException("CPF já cadastrado em outro cliente");
                }
                else
                {
                    var clienteValidation = await _clienteGateway.GetByCnpj(new Cnpj(request.Cnpj), ct);
                    if (clienteValidation is not null)
                        throw new InvalidOperationException("Cnpj já cadastrado em outro cliente");
                }
                Cliente cliente;
                var endereco = new Endereco(request.Endereco.Logradouro, request.Endereco.Numero, request.Endereco.Complemento, request.Endereco.Bairro, request.Endereco.Cidade, request.Endereco.Uf, request.Endereco.Cep);
                var telefone = new Telefone(request.Telefone.DDD, request.Telefone.DDI, request.Telefone.Numero);
                var email = new Email(request.Email);

                if (isCpf)
                {
                    cliente = new Cliente(request.Nome, new Cpf(request.Cpf), idUsuario, endereco, telefone, email);
                }
                else
                {
                    cliente = new Cliente(request.Nome, new Cnpj(request.Cnpj), idUsuario, endereco, telefone, email);
                }

                await _clienteGateway.Create(cliente, ct);
                return cliente.Id;
            }
            catch(InvalidOperationException)
            {
                throw;
            }
            catch(ArgumentException ex)
            {
                throw new ArgumentException(ex.Message);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
