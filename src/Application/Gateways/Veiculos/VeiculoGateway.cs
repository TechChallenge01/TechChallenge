using Application.Interfaces;
using Domain.Entities;
using Domain.ValueObjects;
using Shared.DTOs.Veiculos.Input;

namespace Application.Gateways.Veiculos
{
    public class VeiculoGateway
    {
        private readonly IVeiculoDataSource _dataSource;

        private VeiculoGateway(IVeiculoDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public static VeiculoGateway Create(IVeiculoDataSource dataSource)
        {
            return new VeiculoGateway(dataSource);
        }

        public async Task<(List<Veiculo> veiculos, int total)> GetPaginated(int page, int pageSize, CancellationToken ct)
        {
            var veiculos = await _dataSource.GetPaginated(page, pageSize, ct);

            var response = veiculos.veiculos.Select(v => new Veiculo(v.Id, v.Modelo, v.MarcaVeiculo, v.ClienteId, v.Ano, new Placa(v.Placa), v.Cor, v.UsuarioCriacaoId)).ToList();

            return (response, veiculos.total);
        }

        public async Task<Veiculo>? GetById(Guid id, CancellationToken ct)
        {
            var veiculo = await _dataSource.GetById(id, ct);

            if (veiculo is null)
                return null;

            var response = new Veiculo(veiculo.Id, veiculo.Modelo, veiculo.MarcaVeiculo, veiculo.ClienteId, veiculo.Ano, new Placa(veiculo.Placa), veiculo.Cor, veiculo.UsuarioCriacaoId);

            return response;
        }

        public async Task Create(Veiculo veiculo, CancellationToken ct)
        {
            var veiculoInput = new VeiculoInputDTO
            {
                Id = veiculo.Id,
                Ano = veiculo.Ano,
                ClienteId = veiculo.ClienteId,
                Cor = veiculo.Cor,
                DataCriacao = veiculo.DataCriacao,
                UsuarioCriacaoId = veiculo.UsuarioCriacaoId,
                MarcaVeiculo = veiculo.MarcaVeiculo,
                Modelo = veiculo.Modelo,
                Placa = veiculo.Placa
            };

            await _dataSource.Create(veiculoInput, ct);
        }
        public async Task Update(Veiculo veiculo, CancellationToken ct)
        {
            var veiculoInput = new VeiculoInputDTO
            {
                Id = veiculo.Id,
                Ano = veiculo.Ano,
                ClienteId = veiculo.ClienteId,
                Cor = veiculo.Cor,
                DataCriacao = veiculo.DataCriacao,
                UsuarioCriacaoId = veiculo.UsuarioCriacaoId,
                MarcaVeiculo = veiculo.MarcaVeiculo,
                Modelo = veiculo.Modelo,
                Placa = veiculo.Placa
            };

            await _dataSource.Update(veiculoInput, ct);
        }
    }
}
