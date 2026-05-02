using Domain.Aggregates.ClienteAggregates;
using Domain.Aggregates.EstoqueAggregates;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using Infra.Context;

namespace Infra.Seeds
{
    public static class DbSeeds
    {
        public static async Task Seed(AppDbContext _appDbContext)
        {
            bool atualizou = false;
            if(!_appDbContext.Usuarios.Any())
            {
                atualizou = true;
                var senha = "12345678";

                _appDbContext.Usuarios.Add(new Usuario("Padrao", "padrao@email.com", senha.ToArgon2Hash(), EPerfilUsuario.Administrador, Guid.Empty));
            }

            if(!_appDbContext.Clientes.Any())
            {
                atualizou = true;

                var clientes = new List<Cliente>
                {
                    new Cliente("Retifica Dinamica", new Cnpj("68.380.757.0001/91"), Guid.Empty),
                    new Cliente("Leonardo Pagni", new Cpf("508.725.588-43"), Guid.Empty),
                    new Cliente("José Manuel Lopez", new Cpf("795.663.230-77"), Guid.Empty)
                };

                var enderecos = new List<Endereco>
                {
                    new Endereco("Rua 7", "5", "casa verde", "jabaquara", "São Paulo", "SP", "04347-060"),
                    new Endereco("Rua 8", "6", "casa azul", "jabaquara", "São Paulo", "SP", "04347-060"),
                    new Endereco("Rua 9", "7", "casa amarela", "jabaquara", "São Paulo", "SP", "04347-060"),
                };

                var emails = new List<Email>
                {
                    new Email("leo_pagni@hotmail.com"),
                    new Email("JoseLopez@hotmail.com"),
                    new Email("ManuelSilva@hotmail.com"),
                };
                var telefones = new List<Telefone>
                {
                    new Telefone("11", "55", "95997-2016", ETipoTelefone.Celular),
                    new Telefone("11", "55", "95997-4017", ETipoTelefone.Celular),
                    new Telefone("11", "55", "95997-3018", ETipoTelefone.Celular),
                };

                foreach(var cliente in clientes)
                {
                    cliente.AlterarEnderecos(enderecos);
                    cliente.AlterarEmails(emails);
                    cliente.AlterarTelefones(telefones);
                }

                var veiculos = new List<Veiculo>
                {
                    new Veiculo("Civic", "Honda", clientes[0].Id, 2026, new Placa("KEG-6322"), "Preto", Guid.Empty),
                    new Veiculo("Corolla", "Toyota", clientes[1].Id, 2015, new Placa("MZP-6968"), "Prata", Guid.Empty),
                    new Veiculo("Clio", "Renault", clientes[2].Id, 2008, new Placa("KBI-6915"), "Azul", Guid.Empty),
                };

                _appDbContext.Clientes.AddRange(clientes);
                _appDbContext.Veiculos.AddRange(veiculos);
                await _appDbContext.SaveChangesAsync();
            }

            if(_appDbContext.Pecas.Any())
            {
                atualizou = true;

                var pecas = new List<Peca>
                {
                    new Peca("Pneu", "Penu 215/55", "Michellin", 80, Guid.Empty, DateTime.UtcNow),
                    new Peca("Retrovisor", "Retrovisor renault clio", "original", 160, Guid.Empty, DateTime.UtcNow)
                };

                var estoques = new List<Estoque>
                {
                    new Estoque(null, pecas[0].Id, 0, Guid.Empty, DateTime.UtcNow),
                    new Estoque(null, pecas[1].Id, 0, Guid.Empty, DateTime.UtcNow)
                };

                _appDbContext.Pecas.AddRange(pecas);
                _appDbContext.Estoques.AddRange(estoques);                
            }

            if(_appDbContext.Insumos.Any())
            {
                await _appDbContext.SaveChangesAsync();
                var insumos = new List<Insumo>
                {
                    new Insumo("Oleo do motor", "16/40", 15, Guid.Empty, DateTime.UtcNow),
                    new Insumo("Fluído de freio", "", 30, Guid.Empty, DateTime.UtcNow),
                };

                var estoques = new List<Estoque>
                {
                    new Estoque(insumos[0].Id, null, 0, Guid.Empty, DateTime.UtcNow),
                    new Estoque(insumos[1].Id, null, 0, Guid.Empty, DateTime.UtcNow)
                };

                _appDbContext.Insumos.AddRange(insumos);
                _appDbContext.Estoques.AddRange(estoques);
            }

            if (atualizou)
                await _appDbContext.SaveChangesAsync();
        }
    }
}
