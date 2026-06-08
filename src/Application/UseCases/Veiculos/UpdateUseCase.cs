using Application.Gateways.Veiculos;
using Shared.DTOs.Veiculos.Requests;

namespace Application.UseCases.Veiculos
{
    public class UpdateUseCase
    {
        private readonly VeiculoGateway _veiculoGateway;

        private UpdateUseCase(VeiculoGateway veiculoGateway)
        {
            _veiculoGateway = veiculoGateway;
        }

        public static UpdateUseCase Create(VeiculoGateway veiculoGateway)
        {
            return new UpdateUseCase(veiculoGateway);
        }

        public async Task Run(Guid id, Guid usuarioId, VeiculoRequestDTO veiculo, CancellationToken ct)
        {
            try
            {
                var veiculoEntity = await _veiculoGateway.GetById(id, ct);

                if (veiculoEntity is null)
                    throw new KeyNotFoundException("Veiculo não encontrado!");

                veiculoEntity.AlterarMarcaVeiculo(veiculo.MarcaVeiculo);
                veiculoEntity.AlterarCliente(veiculo.ClienteId);
                veiculoEntity.AlterarAno(veiculo.Ano);
                veiculoEntity.AlterarCor(veiculo.Cor);
                veiculoEntity.AlterarModelo(veiculo.Modelo);

                veiculoEntity.RastrearAlteracao(usuarioId, DateTime.UtcNow);

                await _veiculoGateway.Update(veiculoEntity, ct);
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
