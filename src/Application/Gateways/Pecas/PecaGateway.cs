using Application.Interfaces;
using Domain.Entities;
using Shared.DTOs.Pecas.Input;

namespace Application.Gateways.Pecas;

public class PecaGateway
{
    private readonly IPecaDataSource _dataSource;

    private PecaGateway(IPecaDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public static PecaGateway Create(IPecaDataSource dataSource)
    {
        return new PecaGateway(dataSource);
    }

    public async Task Create(Peca peca, CancellationToken ct)
    {
        var pecaDTO = new PecaInputDTO
        {
            Id = peca.Id,
            Nome = peca.Nome,
            Descricao = peca.Descricao,
            MarcaPeca = peca.MarcaPeca,
            ValorUnitario = peca.ValorUnitario,
            IdUsuarioCriacao = peca.UsuarioCriacaoId,
            DataCriacao = peca.DataCriacao
        };

        await _dataSource.Create(pecaDTO, ct);
    }

    public async Task Update(Peca peca, CancellationToken ct)
    {
        var pecaDTO = new PecaInputDTO
        {
            Id = peca.Id,
            Nome = peca.Nome,
            Descricao = peca.Descricao,
            MarcaPeca = peca.MarcaPeca,
            ValorUnitario = peca.ValorUnitario,
            IdUsuarioAtualizacao = peca.IdUsuarioAtualizacao,
            DataAtualizacao = peca.DataAtualizacao,
            Ativo = peca.Ativo
        };

        await _dataSource.Update(pecaDTO, ct);
    }

    public async Task<Peca?> GetById(Guid id, CancellationToken ct)
    {
        var response = await _dataSource.GetById(id, ct);

        if (response == null)
            return null;

        return new Peca(response.Id, response.Nome, response.Descricao, response.MarcaPeca, response.ValorUnitario);
    }

    public async Task<(List<Peca> Pecas, int total)> GetPaginated(int page, int pageSize, CancellationToken ct)
    {
        var response = await _dataSource.GetPaginated(page, pageSize, ct);

        var pecas = response.pecas
            .Select(p => new Peca(p.Id, p.Nome, p.Descricao, p.MarcaPeca, p.ValorUnitario))
            .ToList();

        return (pecas, response.total);
    }

    public async Task Delete(Guid id, CancellationToken ct)
    {
        await _dataSource.Delete(id, ct);
    }
}
