using Application.Interfaces;
using Shared.DTOs.Insumo.Input;

namespace Application.Gateways.Insumos;

public class InsumoGateway
{
    private readonly IInsumoDataSource _dataSource;

    private InsumoGateway(IInsumoDataSource dataSource)
    {
        _dataSource = dataSource;
    }
    public static InsumoGateway Create(IInsumoDataSource dataSource) 
    { 
        return new InsumoGateway(dataSource);
    }

    public async Task Create(Insumo insumo, CancellationToken ct) 
    {
        var insumoDTO = new InsumoInputDTO
        {
            Id = insumo.Id,
            Nome = insumo.Nome,
            Descricao = insumo.Descricao,
            CustoUnitario = insumo.CustoUnitario,
            IdUsuarioCriacao = insumo.UsuarioCriacaoId,
            DataCriacao = insumo.DataCriacao
        };

        await _dataSource.Create(insumoDTO, ct);
    }

    public async Task Update(Insumo insumo, CancellationToken ct) 
    {
        var insumoDTO = new InsumoInputDTO
        {
            Id = insumo.Id,
            Nome = insumo.Nome,
            Descricao = insumo.Descricao,
            CustoUnitario = insumo.CustoUnitario,
            IdUsuarioAtualizacao = insumo.IdUsuarioAtualizacao,
            DataAtualizacao = insumo.DataAtualizacao,
            Ativo = insumo.Ativo
        };

        await _dataSource.Update(insumoDTO, ct);
    }

    public async Task<Insumo?> GetById(Guid id, CancellationToken ct)
    {
        var response = await _dataSource.GetById(id, ct);

        if(response == null)
        {
            return null;
        }

        var insumo = new Insumo(response.Id, response.Nome, response.Descricao, response.CustoUnitario);

        return insumo;
    }

    public async Task<(List<Insumo> Insumos, int total)> GetPaginated(int page, int pageSize, CancellationToken ct)
    {
        var response = await _dataSource.GetPaginated(page, pageSize, ct);

        var insumos = response.insumos.Select(i => new Insumo(i.Id, i.Nome, i.Descricao, i.CustoUnitario)).ToList();

        return (insumos, response.total);
    }

    public async Task Delete(Guid id, CancellationToken ct)
    {
        await _dataSource.Delete(id, ct);
    }
}
