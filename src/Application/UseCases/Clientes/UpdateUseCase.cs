using Application.Gateways.Clientes;
using Domain.ValueObjects;
using Shared.DTOs.Clientes.Request;

namespace Application.UseCases.Clientes
{
    public class UpdateUseCase
    {
        private readonly ClienteGateway _clienteGateway;

        private UpdateUseCase(ClienteGateway clienteGateway)
        {
            _clienteGateway = clienteGateway;
        }

        public static UpdateUseCase Create(ClienteGateway clienteGateway)
        {
            return new UpdateUseCase(clienteGateway);
        }

        public async Task Run(Guid idUsuario, Guid id, ClienteRequestDTO clienteRequest, CancellationToken ct)
        {
            try
            {
                var cliente = await _clienteGateway.GetById(id, ct);

                if (cliente is null)
                    throw new KeyNotFoundException("Cliente não encontrado!");

                cliente.AlterarEmail(new Email(clienteRequest.Email));
                cliente.AlterarEndereco(new Endereco(clienteRequest.Endereco.Logradouro, clienteRequest.Endereco.Numero, clienteRequest.Endereco.Complemento, clienteRequest.Endereco.Bairro, clienteRequest.Endereco.Cidade, clienteRequest.Endereco.Uf, clienteRequest.Endereco.Uf));
                cliente.AlterarNome(clienteRequest.Nome);
                cliente.AlterarTelefone(new Telefone(clienteRequest.Telefone.DDD, clienteRequest.Telefone.DDI, clienteRequest.Telefone.Numero));
                cliente.RastrearAlteracao(idUsuario, DateTime.UtcNow);

                await _clienteGateway.Update(cliente, ct);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                throw new KeyNotFoundException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
