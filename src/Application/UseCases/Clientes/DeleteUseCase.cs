using Application.Gateways.Clientes;

namespace Application.UseCases.Clientes
{
    public class DeleteUseCase
    {
        private readonly ClienteGateway _clienteGateway;

        private DeleteUseCase(ClienteGateway clienteGateway)
        {
            _clienteGateway = clienteGateway;
        }

        public static DeleteUseCase Create(ClienteGateway clienteGateway)
        {
            return new DeleteUseCase(clienteGateway);
        }

        public async Task Run(Guid idUsuario, Guid id, CancellationToken ct)
        {
            try
            {
                var cliente = await _clienteGateway.GetById(id, ct);

                if (cliente is null)
                    throw new KeyNotFoundException("Cliente não encontrado!");

                cliente.Inativar();
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
