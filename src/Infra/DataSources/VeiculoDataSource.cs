using Application.Interfaces;
using Infra.Context;
using Infra.DbModel;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs.Veiculos.Input;

namespace Infra.DataSources
{
    public class VeiculoDataSource : IVeiculoDataSource
    {
        private readonly AppDbContext _appDbContext;

        public VeiculoDataSource(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }


        public async Task<VeiculoInputDTO>? GetById(Guid id, CancellationToken ct)
        {
            IQueryable<VeiculoDbModel> query = _appDbContext.Veiculos.Where(v => v.Ativo);

            var veiculo = await query.FirstOrDefaultAsync(v => v.Id == id);

            if (veiculo is null)
                return null;

            var response = new VeiculoInputDTO
            {
                Id = veiculo.Id,
                Ano = veiculo.Ano,
                ClienteId = veiculo.ClienteId,
                Cor = veiculo.Cor,
                MarcaVeiculo = veiculo.MarcaVeiculo,
                Modelo = veiculo.Modelo,
                Placa = veiculo.Placa,
                Ativo = veiculo.Ativo,
                DataAlteracao = veiculo.DataAtualizacao,
                DataCriacao = veiculo.DataCriacao,
                UsuarioCriacaoId = veiculo.UsuarioCriacaoId,
                UsuarioAlteracaoId = veiculo.IdUsuarioAtualizacao
            };

            return response;
        }

        public async Task<(List<VeiculoInputDTO> veiculos, int total)> GetPaginated(int page, int pageSize, CancellationToken ct)
        {
            IQueryable<VeiculoDbModel> query = _appDbContext.Veiculos.Where(v => v.Ativo);

            var veiculos = await query.Skip((page - 1) * pageSize)
                                      .Take(pageSize)
                                      .AsNoTracking()
                                      .Include(v => v.Cliente)
                                      .ToListAsync(ct);

            var veiculoResponse = veiculos.Select(v => new VeiculoInputDTO
            {
                Id = v.Id,
                Ano = v.Ano,
                ClienteId = v.ClienteId,
                Cor = v.Cor,
                MarcaVeiculo = v.MarcaVeiculo,
                Modelo = v.Modelo,
                Placa = v.Placa
            }).ToList();

            var total = await query.CountAsync(ct);

            return (veiculoResponse, total);
        }

        public async Task Create(VeiculoInputDTO veiculo, CancellationToken ct)
        {
            var veiculoDbModel = new VeiculoDbModel(veiculo.Id, veiculo.Modelo, veiculo.MarcaVeiculo, veiculo.ClienteId, veiculo.Ano, veiculo.Placa, veiculo.Cor, veiculo.UsuarioCriacaoId, veiculo.DataCriacao, veiculo.UsuarioAlteracaoId, veiculo.DataAlteracao, veiculo.Ativo);

            await _appDbContext.Veiculos.AddAsync(veiculoDbModel, ct);
            await _appDbContext.SaveChangesAsync(ct);
        }

        public async Task Update(VeiculoInputDTO veiculo, CancellationToken ct)
        {
            var veiculoDbModel = await _appDbContext.Veiculos.FirstOrDefaultAsync(v => v.Id == veiculo.Id);

            veiculoDbModel.Ativo = veiculo.Ativo;
            veiculoDbModel.MarcaVeiculo = veiculo.MarcaVeiculo;
            veiculoDbModel.DataCriacao = veiculo.DataCriacao;
            veiculoDbModel.Placa = veiculo.Placa;
            veiculoDbModel.Ano = veiculo.Ano;
            veiculoDbModel.ClienteId = veiculo.ClienteId;
            veiculoDbModel.Cor = veiculo.Cor;
            veiculoDbModel.DataAtualizacao = veiculo.DataAlteracao;
            veiculoDbModel.IdUsuarioAtualizacao = veiculo.UsuarioAlteracaoId;

            await _appDbContext.SaveChangesAsync(ct);
        }

        public async Task<VeiculoInputDTO>? GetByPlaca(string placa, CancellationToken ct)
        {
            IQueryable<VeiculoDbModel> query = _appDbContext.Veiculos.Where(v => v.Ativo);

            var veiculo = await query.FirstOrDefaultAsync(v => v.Placa == placa, ct);

            if (veiculo is null)
                return null;


            return new VeiculoInputDTO
            {
                Ano = veiculo.Ano,
                Ativo = veiculo.Ativo,
                ClienteId = veiculo.ClienteId,
                Cor = veiculo.Cor,
                DataAlteracao = veiculo.DataAtualizacao,
                DataCriacao = veiculo.DataCriacao,
                Id = veiculo.Id,
                MarcaVeiculo = veiculo.MarcaVeiculo,
                Modelo = veiculo.Modelo,
                Placa = veiculo.Placa,
                UsuarioAlteracaoId = veiculo.IdUsuarioAtualizacao,
                UsuarioCriacaoId = veiculo.UsuarioCriacaoId
            };
        }
    }
}
