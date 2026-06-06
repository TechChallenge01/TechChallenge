using Application.Gateways.Veiculos;

namespace Application.UseCases.Veiculos
{
    public class DeleteUseCase
    {
        private readonly VeiculoGateway _veiculoGateway;

        private DeleteUseCase(VeiculoGateway veiculoGateway)
        {
            _veiculoGateway = veiculoGateway;
        }

        public static DeleteUseCase Create(VeiculoGateway veiculoGateway)
        {
            return new DeleteUseCase(veiculoGateway);
        }

        public async Task Run(Guid id, Guid usuarioId, CancellationToken ct)
        {
            try
            {
                var veiculo = await _veiculoGateway.GetById(id, ct);

                if (veiculo is null)
                    throw new KeyNotFoundException("Veiculo não encontrado!");

                veiculo.Inativar();
                veiculo.RastrearAlteracao(usuarioId, DateTime.UtcNow);

                await _veiculoGateway.Update(veiculo, ct);
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
