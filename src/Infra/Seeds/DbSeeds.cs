using Domain.Aggregates.ClienteAggregates;
using Domain.Aggregates.EstoqueAggregates;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using Infra.Context;
using System.Diagnostics.CodeAnalysis;

namespace Infra.Seeds
{
    [ExcludeFromCodeCoverage]
    public static class DbSeeds
    {
        public static async Task Seed(AppDbContext _appDbContext)
        {
            // Seed Usuários
            if(!_appDbContext.Usuarios.Any())
            {
                SeedUsuarios(_appDbContext);
            }

            // Seed Serviços (não precisa de estoque)
            if (!_appDbContext.Servicos.Any())
            {
                SeedServicos(_appDbContext);
            }

            // Seed Peças + Estoques (juntos para garantir integridade)
            if (!_appDbContext.Pecas.Any())
            {
                SeedPecasComEstoque(_appDbContext);
            }

            // Seed Insumos + Estoques (juntos para garantir integridade)
            if (!_appDbContext.Insumos.Any())
            {
                SeedInsumosComEstoque(_appDbContext);
            }

            // Seed Clientes
            if (!_appDbContext.Clientes.Any())
            {
                SeedClientes(_appDbContext);
            }

            // Seed Veículos
            if (!_appDbContext.Veiculos.Any())
            {
                SeedVeiculos(_appDbContext);
            }
        }

        private static void SeedUsuarios(AppDbContext context)
        {
            var usuarioAdmin = new Usuario("Admin", "Admin@email.com", BCrypt.Net.BCrypt.HashPassword("12345678"), EPerfilUsuario.Administrador, Guid.Empty);
            context.Usuarios.Add(usuarioAdmin);
            context.SaveChanges();
        }

        private static void SeedPecasComEstoque(AppDbContext context)
        {
            var usuarioId = context.Usuarios.First().Id;
            var agora = DateTime.UtcNow;

            var pecas = new List<Peca>
            {
                (new Peca("Filtro de Ar", "Filtro de ar premium para motores", "Bosch", 85.50m, usuarioId, agora)),
                (new Peca("Pastilha de Freio", "Pastilha de freio cerâmica de alta performance", "Frenmax", 120.00m, usuarioId, agora)),
                (new Peca("Vela de Ignição", "Vela de ignição de platina", "NGK", 45.00m, usuarioId, agora)),
                (new Peca("Bateria Automotiva", "Bateria 12V 60Ah", "Moura", 350.00m, usuarioId, agora)),
                (new Peca("Amortecedor Dianteiro", "Amortecedor dianteiro com mola", "Monroe", 280.00m, usuarioId, agora)),
                (new Peca("Correia Dentada", "Correia de distribuição", "Contitech", 150.00m, usuarioId, agora)),
                (new Peca("Pneu aro 14", "Pneu 195/65 R14", "Bridgestone", 320.00m, usuarioId, agora)),
                (new Peca("Óleo Lubrificante", "Óleo sintético 5W-40", "Shell", 75.00m, usuarioId, agora)),
                (new Peca("Disco de Freio", "Disco de freio ventilado", "Brembo", 180.00m, usuarioId, agora)),
                (new Peca("Radiador", "Radiador de alumínio", "Valeo", 420.00m, usuarioId, agora)),
            };

            foreach (var peca in pecas)
            {
                // Adicionar peça
                context.Pecas.Add(peca);
                context.SaveChanges();

                // Criar estoque imediatamente após a peça
                var estoque = new Estoque(null, peca.Id, 0, usuarioId, agora);
                context.Estoques.Add(estoque);
                context.SaveChanges();
            }
        }

        private static void SeedInsumosComEstoque(AppDbContext context)
        {
            var usuarioId = context.Usuarios.First().Id;
            var agora = DateTime.UtcNow;

            var insumos = new List<Insumo>
            {
                (new Insumo("Limpador Desengordurante", "Limpador desengraxante para peças automotivas", 25.50m, usuarioId, agora)),
                (new Insumo("Graxa Multiuso", "Graxa NLGI grade 2", 15.00m, usuarioId, agora)),
                (new Insumo("Fluido de Freio", "Fluido de freio DOT 4", 35.00m, usuarioId, agora)),
                (new Insumo("Refrigerante", "Refrigerante rosa concentrado", 22.00m, usuarioId, agora)),
                (new Insumo("Combustível Aditivo", "Aditivo para combustível", 18.00m, usuarioId, agora)),
                (new Insumo("Silicone Automotivo", "Silicone protetor de borracha", 12.50m, usuarioId, agora)),
                (new Insumo("Álcool Isopropílico", "Álcool isopropílico 99%", 8.00m, usuarioId, agora)),
                (new Insumo("Lápis para Retoque", "Lápis para retoque de pintura", 20.00m, usuarioId, agora)),
                (new Insumo("Estopa", "Estopa branca 500g", 5.50m, usuarioId, agora)),
                (new Insumo("Fita de Isolamento", "Fita de isolamento elétrica", 3.50m, usuarioId, agora)),
            };

            foreach (var insumo in insumos)
            {
                // Adicionar insumo
                context.Insumos.Add(insumo);
                context.SaveChanges();

                // Criar estoque imediatamente após o insumo
                var estoque = new Estoque(insumo.Id, null, 0, usuarioId, agora);
                context.Estoques.Add(estoque);
                context.SaveChanges();
            }
        }

        private static void SeedServicos(AppDbContext context)
        {
            var usuarioId = context.Usuarios.First().Id;
            var agora = DateTime.UtcNow;

            var servicos = new List<Servico>
            {
                new Servico("Troca de Óleo", "Troca de óleo e filtro do motor", 120.00m, usuarioId, agora),
                new Servico("Alinhamento", "Alinhamento e balanceamento de rodas", 200.00m, usuarioId, agora),
                new Servico("Limpeza de Injetor", "Limpeza dos injetores de combustível", 180.00m, usuarioId, agora),
                new Servico("Revisão Completa", "Revisão geral do veículo", 350.00m, usuarioId, agora),
                new Servico("Troca de Pastilha de Freio", "Substituição das pastilhas de freio", 250.00m, usuarioId, agora),
                new Servico("Limpeza de Radiador", "Limpeza do sistema de arrefecimento", 150.00m, usuarioId, agora),
                new Servico("Troca de Filtro de Ar", "Substituição do filtro de ar", 80.00m, usuarioId, agora),
                new Servico("Diagnóstico Eletrônico", "Diagnóstico completo do veículo", 200.00m, usuarioId, agora),
                new Servico("Polimento de Pintura", "Polimento e proteção da pintura", 300.00m, usuarioId, agora),
                new Servico("Troca de Bateria", "Substituição da bateria automotiva", 100.00m, usuarioId, agora),
            };

            context.Servicos.AddRange(servicos);
            context.SaveChanges();
        }

        private static void SeedClientes(AppDbContext context)
        {
            var usuarioId = context.Usuarios.First().Id;

            var clientes = new List<Cliente>
            {
                new Cliente(
                    "João Silva",
                    new Cpf("52998224725"),
                    usuarioId,
                    new Endereco("Rua das Flores", "123", "Apto 101", "Centro", "São Paulo", "SP", "01310100"),
                    new Telefone("11", "55", "987654321"),
                    new Email("joao.silva@email.com")
                ),
                new Cliente(
                    "Maria Santos",
                    new Cpf("12345678909"),
                    usuarioId,
                    new Endereco("Av. Paulista", "1000", null, "Bela Vista", "São Paulo", "SP", "01310100"),
                    new Telefone("11", "55", "987654322"),
                    new Email("maria.santos@email.com")
                ),
                new Cliente(
                    "Carlos Oliveira",
                    new Cpf("11144477735"),
                    usuarioId,
                    new Endereco("Rua do Comércio", "456", "Sala 201", "Consolação", "São Paulo", "SP", "01310100"),
                    new Telefone("11", "55", "987654323"),
                    new Email("carlos.oliveira@email.com")
                ),
                new Cliente(
                    "Ana Costa",
                    new Cpf("93541134780"),
                    usuarioId,
                    new Endereco("Rua de Janeiro", "789", null, "Vila Mariana", "São Paulo", "SP", "01310100"),
                    new Telefone("11", "55", "987654324"),
                    new Email("ana.costa@email.com")
                ),
                new Cliente(
                    "Tech Solutions Ltda",
                    new Cnpj("68380757000191"),
                    usuarioId,
                    new Endereco("Av. Tecnológica", "2000", "Bloco A", "Parque Tecnológico", "São Paulo", "SP", "01310100"),
                    new Telefone("11", "55", "33333333"),
                    new Email("contato@techsolutions.com.br")
                ),
                new Cliente(
                    "Serviços Gerais Ltda",
                    new Cnpj("56432696000180"),
                    usuarioId,
                    new Endereco("Rua Industrial", "3000", null, "Vila Prudente", "São Paulo", "SP", "01310100"),
                    new Telefone("11", "55", "44444444"),
                    new Email("servicos@geraisltda.com.br")
                ),
            };

            context.Clientes.AddRange(clientes);
            context.SaveChanges();
        }

        private static void SeedVeiculos(AppDbContext context)
        {
            var usuarioId = context.Usuarios.First().Id;
            var clientes = context.Clientes.ToList();

            var veiculos = new List<Veiculo>
            {
                new Veiculo("Civic", "Honda", clientes[0].Id, 2020, new Placa("ABC1234"), "Preto", usuarioId),
                new Veiculo("Gol", "Volkswagen", clientes[0].Id, 2018, new Placa("DEF5678"), "Branco", usuarioId),
                new Veiculo("Corolla", "Toyota", clientes[1].Id, 2021, new Placa("GHI9012"), "Prata", usuarioId),
                new Veiculo("Onix", "Chevrolet", clientes[1].Id, 2019, new Placa("JKL3456"), "Cinza", usuarioId),
                new Veiculo("Hyundai HB20", "Hyundai", clientes[2].Id, 2022, new Placa("MNO7890"), "Azul", usuarioId),
                new Veiculo("Fiat Uno", "Fiat", clientes[2].Id, 2017, new Placa("PQR1234"), "Vermelho", usuarioId),
                new Veiculo("Jeep Compass", "Jeep", clientes[3].Id, 2020, new Placa("STU5678"), "Branco", usuarioId),
                new Veiculo("Ford Fusion", "Ford", clientes[3].Id, 2019, new Placa("VWX9012"), "Preto", usuarioId),
                new Veiculo("Renault Kwid", "Renault", clientes[4].Id, 2021, new Placa("YZA3456"), "Laranja", usuarioId),
                new Veiculo("Peugeot 208", "Peugeot", clientes[5].Id, 2020, new Placa("BCD7890"), "Verde", usuarioId),
            };

            context.Veiculos.AddRange(veiculos);
            context.SaveChanges();
        }
    }
}
