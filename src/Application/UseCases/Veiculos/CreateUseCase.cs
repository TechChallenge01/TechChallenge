using Application.Gateways.Clientes;
using Application.Gateways.Veiculos;
using Domain.Entities;
using Domain.ValueObjects;
using Shared.DTOs.Veiculos.Requests;

namespace Application.UseCases.Veiculos
{
    public class CreateUseCase
    {
        private readonly VeiculoGateway _veiculoGateway;
        private readonly ClienteGateway _clienteGateway;

        private CreateUseCase(VeiculoGateway veiculoGateway, ClienteGateway clienteGateway)
        {
            _veiculoGateway = veiculoGateway;
            _clienteGateway = clienteGateway;
        }

        public static CreateUseCase Create(VeiculoGateway veiculoGateway, ClienteGateway clienteGateway) 
        {
            return new CreateUseCase(veiculoGateway, clienteGateway);
        }

        public async Task<Guid> Run(VeiculoRequestDTO veiculo, Guid usuarioCriacaoId, CancellationToken ct)
        {
            try
            {
                var clienteUseCase = Clientes.GetByIdUseCase.Create(_clienteGateway);
                var cliente = clienteUseCase.Run(veiculo.ClienteId, ct);

                if (cliente is null)
                    throw new KeyNotFoundException("Cliente não encontrado!");

                var veiculoEntity = new Veiculo(veiculo.Modelo, veiculo.MarcaVeiculo, veiculo.ClienteId, veiculo.Ano, new Placa(veiculo.Placa), veiculo.Cor, usuarioCriacaoId);

                await _veiculoGateway.Create(veiculoEntity, ct);

                return veiculoEntity.Id;
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
