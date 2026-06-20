using Application.Gateways.Veiculos;
using Domain.Aggregates.ClienteAggregates;
using Domain.Entities;
using Domain.ValueObjects;
using Shared.DTOs.Veiculos.Requests;

namespace Application.UseCases.OrdensServicos
{
    public class ValidarVeiculoOrdemServicoUseCase
    {
        private readonly VeiculoGateway _veiculoGateway;

        private ValidarVeiculoOrdemServicoUseCase(VeiculoGateway veiculoGateway)
        {
            _veiculoGateway = veiculoGateway;
        }

        public static ValidarVeiculoOrdemServicoUseCase Create(VeiculoGateway veiculoGateway)
        {
            return new ValidarVeiculoOrdemServicoUseCase(veiculoGateway);
        }


        public async Task<Veiculo> Run(Cliente cliente, VeiculoRequestDTO veiculoRequest, Guid idUsuario, CancellationToken ct)
        {
            try
            {
                var placa = new Placa(veiculoRequest.Placa);
                var veiculo = await _veiculoGateway.GetByPlaca(placa, ct);

                if (veiculo is null)
                {
                    veiculo = new Veiculo(veiculoRequest.Modelo, veiculoRequest.MarcaVeiculo, cliente.Id, veiculoRequest.Ano, placa, veiculoRequest.Cor, idUsuario);

                    await _veiculoGateway.Create(veiculo, ct);
                    return veiculo;
                }

                if (veiculo.ClienteId == cliente.Id)
                    throw new ArgumentException("Numeração da placa existente em outro veiculo pertencente à outro cliente, realize ajustes no cadastro, antes de criar a Ordem de Serviço");

                return veiculo;

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
