using Application.OrdemServicos.DTOs.Responses;
using Domain.Aggregates.OrdemServicoAggregates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.OrdemServicos.Presenters;

public static class OrdemServicoPresenterExtension
{
    public static OrdemServicoResponseDTO ToDTO(this OrdemServico ordemServico) 
    {
        return new OrdemServicoResponseDTO
        {
            NomeCliente = ordemServico.NomeCliente,
            ModeloVeiculo = ordemServico.ModeloVeiculo,
            PlacaVeiculo = ordemServico.PlacaVeiculo,
            MarcaVeiculo = ordemServico.MarcaVeiculo,
            StatusOS = ordemServico.StatusOS.ToString(),
            Observacao = ordemServico.Observacao,
            ValorTotal = ordemServico.ValorTotal,
            ValorDesconto = ordemServico.ValorDesconto,

            Pecas = ordemServico.Pecas != null ? null : ordemServico.Pecas.Select(p => new OrdemServicoPecaResponseDTO
            {
                PecaId = p.PecaId,
                NomePeca = p.NomePeca,
                DescricaoPeca = p.DescricaoPeca,
                Quantidade = p.Quantidade,
                ValorUnitario = p.ValorUnitario,
                ValorTotal = p.ValorTotal
            }).ToList(),

            Servicos = ordemServico.Servicos != null ? null : ordemServico.Servicos.Select(s => new OrdemServicoServicoResponseDTO
            {
                ServicoId = s.ServicoId,
                NomeServico = s.NomeServico,
                DescricaoServico = s.DescricaoServico,
                Quantidade = s.Quantidade,
                ValorUnitario = s.ValorUnitario,
                ValorTotal = s.ValorTotal
            }).ToList()
        };
    }

    public static ICollection<OrdemServicoResponseDTO> ToListDTO(this ICollection<OrdemServico> ordemServicos)
    {
        return ordemServicos.Select(os => os.ToDTO()).ToList();
    }
}
