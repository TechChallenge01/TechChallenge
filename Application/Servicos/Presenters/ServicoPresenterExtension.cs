using Application.Servicos.DTOs.Response;
using Domain.Entities;

namespace Application.Servicos.Presenters;

public static class ServicoPresenterExtension
{
    public static ServicoResponseDTO ToDto(this Servico servicos)
    {
        return new ServicoResponseDTO
        {
            Id = servicos.Id,
            Nome = servicos.Nome,
            Descricao = servicos.Descricao,
            PrecoVenda = servicos.ValorUnitario,
            TempoMedioExecucao = servicos.TempoMedioExecucao
        };
    }

    public static List<ServicoResponseDTO> ToDtoList(this IEnumerable<Servico> servicos)
    {
        return servicos.Select(s => ToDto(s)).ToList();
    }
}
