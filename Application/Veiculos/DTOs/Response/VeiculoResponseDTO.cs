using Domain.Aggregates.ClienteAggregates;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Veiculos.DTOs.Response
{
    public record VeiculoResponseDTO
    {
        public Guid Id { get; init; }
        public string Marca { get; init; }
        public string Modelo { get; init; }
        public int Ano { get; init; }
        public string Placa { get; init; }
        public string Cor { get; init; }
        public string NomeCliente { get; init; }        
    }
}
