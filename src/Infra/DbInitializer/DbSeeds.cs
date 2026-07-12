using Infra.Context;
using Infra.DbModel;
using Microsoft.EntityFrameworkCore;

namespace Infra.DbInitializer
{
    public static class DbSeeds
    {
        // ── GUIDs fixos ────────────────────────────────────────────────────────

        // Usuários
        private static readonly Guid UsuarioAdminId    = Guid.Parse("11111112-0000-0000-0000-000000000001");
        private static readonly Guid UsuarioMecanicoId = Guid.Parse("11111111-0000-0000-0000-000000000002");
        private static readonly Guid UsuarioFuncionarioId  = Guid.Parse("11111111-0000-0000-0000-000000000003");

        // Clientes
        private static readonly Guid ClienteJoaoId    = Guid.Parse("22222222-0000-0000-0000-000000000001");
        private static readonly Guid ClienteMariaId   = Guid.Parse("22222222-0000-0000-0000-000000000002");
        private static readonly Guid ClienteEmpresaId = Guid.Parse("22222222-0000-0000-0000-000000000003");

        // Veículos
        private static readonly Guid VeiculoUnoId      = Guid.Parse("33333333-0000-0000-0000-000000000001");
        private static readonly Guid VeiculoHRVId      = Guid.Parse("33333333-0000-0000-0000-000000000002");
        private static readonly Guid VeiculoSprinterId = Guid.Parse("33333333-0000-0000-0000-000000000003");

        // Serviços
        private static readonly Guid ServicoTrocaOleoId = Guid.Parse("44444444-0000-0000-0000-000000000001");
        private static readonly Guid ServicoAlinhamId   = Guid.Parse("44444444-0000-0000-0000-000000000002");
        private static readonly Guid ServicoRevisaoId   = Guid.Parse("44444444-0000-0000-0000-000000000003");

        // Peças
        private static readonly Guid PecaFiltroOleoId    = Guid.Parse("55555555-0000-0000-0000-000000000001");
        private static readonly Guid PecaPastilhaFreioId = Guid.Parse("55555555-0000-0000-0000-000000000002");
        private static readonly Guid PecaVelaIgnicaoId   = Guid.Parse("55555555-0000-0000-0000-000000000003");

        // Insumos
        private static readonly Guid InsumoOleoMotorId    = Guid.Parse("66666666-0000-0000-0000-000000000001");
        private static readonly Guid InsumoLiquidoFreioId = Guid.Parse("66666666-0000-0000-0000-000000000002");

        // Estoques de peças (1-para-1, criados com 0 unidades)
        private static readonly Guid EstoqueFiltroId        = Guid.Parse("77777777-0000-0000-0000-000000000001");
        private static readonly Guid EstoquePastilhaId      = Guid.Parse("77777777-0000-0000-0000-000000000002");
        private static readonly Guid EstoqueVelaId          = Guid.Parse("77777777-0000-0000-0000-000000000003");

        // Estoques de insumos
        private static readonly Guid EstoqueOleoId          = Guid.Parse("77777777-0000-0000-0000-000000000004");
        private static readonly Guid EstoqueLiquidoFreioId  = Guid.Parse("77777777-0000-0000-0000-000000000005");

        // ── Data base de referência ────────────────────────────────────────────
        private static readonly DateTime DataBase = new DateTime(2025, 1, 10, 8, 0, 0, DateTimeKind.Utc);

        // ══════════════════════════════════════════════════════════════════════
        // ENTRY POINT — chamado no startup da aplicação
        // ══════════════════════════════════════════════════════════════════════
        public static async Task Seed(AppDbContext context)
        {
            await SeedUsuarios(context);
            await SeedClientes(context);
            await SeedVeiculos(context);
            await SeedServicos(context);
            await SeedPecas(context);
            await SeedInsumos(context);
            await SeedEstoques(context);
        }

        // ══════════════════════════════════════════════════════════════════════
        // USUÁRIOS
        // ══════════════════════════════════════════════════════════════════════
        private static async Task SeedUsuarios(AppDbContext context)
        {
            var usuarios = new List<UsuarioDbModel>
            {
                new UsuarioDbModel(
                    id: UsuarioAdminId,
                    nome: "Administrador",
                    email: "admin@oficina.com.br",
                    senhaHash: BCrypt.Net.BCrypt.HashPassword("senha123"),
                    perfil: "Administrador",
                    idUsuarioCriacao: UsuarioAdminId,
                    dataCriacao: DataBase,
                    idUsuarioAtualizacao: null,
                    dataAtualizacao: null,
                    ativo: true),

                new UsuarioDbModel(
                    id: UsuarioMecanicoId,
                    nome: "Carlos Silva",
                    email: "carlos.mecanico@oficina.com.br",
                    senhaHash: BCrypt.Net.BCrypt.HashPassword("senha123"),
                    perfil: "Mecanico",
                    idUsuarioCriacao: UsuarioAdminId,
                    dataCriacao: DataBase,
                    idUsuarioAtualizacao: null,
                    dataAtualizacao: null,
                    ativo: true),

                new UsuarioDbModel(
                    id: UsuarioFuncionarioId,
                    nome: "Fernanda Souza",
                    email: "fernanda.Funcionario@oficina.com.br",
                    senhaHash: BCrypt.Net.BCrypt.HashPassword("senha123"),
                    perfil: "Funcionario",
                    idUsuarioCriacao: UsuarioAdminId,
                    dataCriacao: DataBase,
                    idUsuarioAtualizacao: null,
                    dataAtualizacao: null,
                    ativo: true),
            };

            foreach (var usuario in usuarios)
            {
                if (!await context.Usuarios.AnyAsync(u => u.Id == usuario.Id))
                    await context.Usuarios.AddAsync(usuario);
            }

            await context.SaveChangesAsync();
        }

        // ══════════════════════════════════════════════════════════════════════
        // CLIENTES — CPFs e CNPJ matematicamente válidos
        // ══════════════════════════════════════════════════════════════════════
        private static async Task SeedClientes(AppDbContext context)
        {
            var clientes = new List<ClienteDbModel>
            {
                new ClienteDbModel(
                    id: ClienteJoaoId,
                    nome: "João Pereira",
                    cpf: "52998224725",       // CPF válido
                    cnpj: null,
                    email: "joao.pereira@email.com",
                    dDD: "11",
                    dDI: "55",
                    numeroTelefone: "912345678",
                    logradouro: "Rua das Flores",
                    numero: "123",
                    complemento: "Apto 4B",
                    bairro: "Jardim Primavera",
                    cep: "01310100",
                    cidade: "São Paulo",
                    uf: "SP",
                    idUsuarioCriacao: UsuarioFuncionarioId,
                    dataCriacao: DataBase,
                    idUsuarioAtualizacao: null,
                    dataAtualizacao: null),

                new ClienteDbModel(
                    id: ClienteMariaId,
                    nome: "Maria Oliveira",
                    cpf: "11144477735",       // CPF válido
                    cnpj: null,
                    email: "maria.oliveira@email.com",
                    dDD: "21",
                    dDI: "55",
                    numeroTelefone: "987654321",
                    logradouro: "Av. Brasil",
                    numero: "456",
                    complemento: null,
                    bairro: "Centro",
                    cep: "20040020",
                    cidade: "Rio de Janeiro",
                    uf: "RJ",
                    idUsuarioCriacao: UsuarioFuncionarioId,
                    dataCriacao: DataBase.AddDays(2),
                    idUsuarioAtualizacao: null,
                    dataAtualizacao: null),

                new ClienteDbModel(
                    id: ClienteEmpresaId,
                    nome: "Transportes Veloz Ltda.",
                    cpf: null,
                    cnpj: "62173620000180",  // CNPJ válido
                    email: "contato@transportesveloz.com.br",
                    dDD: "31",
                    dDI: "55",
                    numeroTelefone: "33221100",
                    logradouro: "Rodovia BR-040",
                    numero: "1000",
                    complemento: "Galpão 5",
                    bairro: "Distrito Industrial",
                    cep: "30640901",
                    cidade: "Belo Horizonte",
                    uf: "MG",
                    idUsuarioCriacao: UsuarioFuncionarioId,
                    dataCriacao: DataBase.AddDays(5),
                    idUsuarioAtualizacao: null,
                    dataAtualizacao: null),
            };

            foreach (var cliente in clientes)
            {
                if (!await context.Clientes.AnyAsync(c => c.Id == cliente.Id))
                    await context.Clientes.AddAsync(cliente);
            }

            await context.SaveChangesAsync();
        }

        // ══════════════════════════════════════════════════════════════════════
        // VEÍCULOS
        // ══════════════════════════════════════════════════════════════════════
        private static async Task SeedVeiculos(AppDbContext context)
        {
            var veiculos = new List<VeiculoDbModel>
            {
                new VeiculoDbModel(
                    id: VeiculoUnoId,
                    modelo: "Uno Mille",
                    marcaVeiculo: "Fiat",
                    clienteId: ClienteJoaoId,
                    ano: 2018,
                    placa: "ABC1234",
                    cor: "Branco",
                    idUsuarioCriacao: UsuarioFuncionarioId,
                    dataCriacao: DataBase,
                    idUsuarioAtualizacao: null,
                    dataAtualizacao: null,
                    ativo: true),

                new VeiculoDbModel(
                    id: VeiculoHRVId,
                    modelo: "HR-V EX",
                    marcaVeiculo: "Honda",
                    clienteId: ClienteMariaId,
                    ano: 2022,
                    placa: "DEF5678",
                    cor: "Prata",
                    idUsuarioCriacao: UsuarioFuncionarioId,
                    dataCriacao: DataBase.AddDays(2),
                    idUsuarioAtualizacao: null,
                    dataAtualizacao: null,
                    ativo: true),

                new VeiculoDbModel(
                    id: VeiculoSprinterId,
                    modelo: "Sprinter 415 CDI",
                    marcaVeiculo: "Mercedes-Benz",
                    clienteId: ClienteEmpresaId,
                    ano: 2020,
                    placa: "GHI9012",
                    cor: "Branco",
                    idUsuarioCriacao: UsuarioFuncionarioId,
                    dataCriacao: DataBase.AddDays(5),
                    idUsuarioAtualizacao: null,
                    dataAtualizacao: null,
                    ativo: true),
            };

            foreach (var veiculo in veiculos)
            {
                if (!await context.Veiculos.AnyAsync(v => v.Id == veiculo.Id))
                    await context.Veiculos.AddAsync(veiculo);
            }

            await context.SaveChangesAsync();
        }

        // ══════════════════════════════════════════════════════════════════════
        // SERVIÇOS
        // ══════════════════════════════════════════════════════════════════════
        private static async Task SeedServicos(AppDbContext context)
        {
            var servicos = new List<ServicoDbModel>
            {
                new ServicoDbModel(
                    id: ServicoTrocaOleoId,
                    nome: "Troca de Óleo",
                    descricao: "Troca de óleo do motor com substituição do filtro de óleo.",
                    valorUnitario: 80.00m,
                    tempoMedioExecucao: TimeSpan.FromMinutes(30),
                    idUsuarioCriacao: UsuarioAdminId,
                    dataCriacao: DataBase,
                    idUsuarioAtualizacao: null,
                    dataAtualizacao: null,
                    ativo: true),

                new ServicoDbModel(
                    id: ServicoAlinhamId,
                    nome: "Alinhamento e Balanceamento",
                    descricao: "Alinhamento das rodas dianteiras e traseiras com balanceamento dos quatro pneus.",
                    valorUnitario: 120.00m,
                    tempoMedioExecucao: TimeSpan.FromHours(1),
                    idUsuarioCriacao: UsuarioAdminId,
                    dataCriacao: DataBase,
                    idUsuarioAtualizacao: null,
                    dataAtualizacao: null,
                    ativo: true),

                new ServicoDbModel(
                    id: ServicoRevisaoId,
                    nome: "Revisão Completa",
                    descricao: "Revisão geral do veículo: motor, freios, suspensão, elétrica e fluídos.",
                    valorUnitario: 350.00m,
                    tempoMedioExecucao: TimeSpan.FromHours(4),
                    idUsuarioCriacao: UsuarioAdminId,
                    dataCriacao: DataBase,
                    idUsuarioAtualizacao: null,
                    dataAtualizacao: null,
                    ativo: true),
            };

            foreach (var servico in servicos)
            {
                if (!await context.Servicos.AnyAsync(s => s.Id == servico.Id))
                    await context.Servicos.AddAsync(servico);
            }

            await context.SaveChangesAsync();
        }

        // ══════════════════════════════════════════════════════════════════════
        // PEÇAS
        // ══════════════════════════════════════════════════════════════════════
        private static async Task SeedPecas(AppDbContext context)
        {
            var pecas = new List<PecaDbModel>
            {
                new PecaDbModel(
                    id: PecaFiltroOleoId,
                    nome: "Filtro de Óleo",
                    descricao: "Filtro de óleo para motor 1.0 e 1.4 flex.",
                    marcaPeca: "Mann-Filter",
                    valorUnitario: 35.90m,
                    idUsuarioCriacao: UsuarioAdminId,
                    dataCriacao: DataBase,
                    idUsuarioAtualizacao: null,
                    dataAtualizacao: null,
                    ativo: true),

                new PecaDbModel(
                    id: PecaPastilhaFreioId,
                    nome: "Pastilha de Freio Dianteira",
                    descricao: "Jogo de pastilhas de freio dianteiro para veículos leves.",
                    marcaPeca: "Fras-le",
                    valorUnitario: 89.90m,
                    idUsuarioCriacao: UsuarioAdminId,
                    dataCriacao: DataBase,
                    idUsuarioAtualizacao: null,
                    dataAtualizacao: null,
                    ativo: true),

                new PecaDbModel(
                    id: PecaVelaIgnicaoId,
                    nome: "Vela de Ignição",
                    descricao: "Vela de ignição iridium para motores flex 1.0 a 2.0.",
                    marcaPeca: "NGK",
                    valorUnitario: 42.50m,
                    idUsuarioCriacao: UsuarioAdminId,
                    dataCriacao: DataBase,
                    idUsuarioAtualizacao: null,
                    dataAtualizacao: null,
                    ativo: true),
            };

            foreach (var peca in pecas)
            {
                if (!await context.Pecas.AnyAsync(p => p.Id == peca.Id))
                    await context.Pecas.AddAsync(peca);
            }

            await context.SaveChangesAsync();
        }

        // ══════════════════════════════════════════════════════════════════════
        // INSUMOS
        // ══════════════════════════════════════════════════════════════════════
        private static async Task SeedInsumos(AppDbContext context)
        {
            var insumos = new List<InsumoDbModel>
            {
                new InsumoDbModel(
                    id: InsumoOleoMotorId,
                    nome: "Óleo de Motor 5W30",
                    descricao: "Óleo sintético para motor a gasolina, diesel e flex — 1 litro.",
                    custoUnitario: 28.50m,
                    idUsuarioCriacao: UsuarioAdminId,
                    dataCriacao: DataBase,
                    idUsuarioAtualizacao: null,
                    dataAtualizacao: null,
                    ativo: true),

                new InsumoDbModel(
                    id: InsumoLiquidoFreioId,
                    nome: "Líquido de Freio DOT 4",
                    descricao: "Fluido de freio DOT 4 — frasco 500ml.",
                    custoUnitario: 18.00m,
                    idUsuarioCriacao: UsuarioAdminId,
                    dataCriacao: DataBase,
                    idUsuarioAtualizacao: null,
                    dataAtualizacao: null,
                    ativo: true),
            };

            foreach (var insumo in insumos)
            {
                if (!await context.Insumos.AnyAsync(i => i.Id == insumo.Id))
                    await context.Insumos.AddAsync(insumo);
            }

            await context.SaveChangesAsync();
        }

        // ══════════════════════════════════════════════════════════════════════
        // ESTOQUES
        // Criados automaticamente ao cadastrar peça/insumo, com 0 unidades
        // ══════════════════════════════════════════════════════════════════════
        private static async Task SeedEstoques(AppDbContext context)
        {
            var estoques = new List<EstoqueDbModel>
            {
                // ── Estoques de peças ─────────────────────────────────────────
                new EstoqueDbModel(
                    id: EstoqueFiltroId,
                    pecaId: PecaFiltroOleoId,
                    insumoId: null,
                    quantidadeDisponivel: 0,
                    quantidadeReservada: 0,
                    historicos: new List<EstoqueHistoricoDbmodel>(),
                    peca: null!,
                    insumo: null!,
                    ativo: true),

                new EstoqueDbModel(
                    id: EstoquePastilhaId,
                    pecaId: PecaPastilhaFreioId,
                    insumoId: null,
                    quantidadeDisponivel: 0,
                    quantidadeReservada: 0,
                    historicos: new List<EstoqueHistoricoDbmodel>(),
                    peca: null!,
                    insumo: null!,
                    ativo: true),

                new EstoqueDbModel(
                    id: EstoqueVelaId,
                    pecaId: PecaVelaIgnicaoId,
                    insumoId: null,
                    quantidadeDisponivel: 0,
                    quantidadeReservada: 0,
                    historicos: new List<EstoqueHistoricoDbmodel>(),
                    peca: null!,
                    insumo: null!,
                    ativo: true),

                // ── Estoques de insumos ───────────────────────────────────────
                new EstoqueDbModel(
                    id: EstoqueOleoId,
                    pecaId: null,
                    insumoId: InsumoOleoMotorId,
                    quantidadeDisponivel: 0,
                    quantidadeReservada: 0,
                    historicos: new List<EstoqueHistoricoDbmodel>(),
                    peca: null!,
                    insumo: null!,
                    ativo: true),

                new EstoqueDbModel(
                    id: EstoqueLiquidoFreioId,
                    pecaId: null,
                    insumoId: InsumoLiquidoFreioId,
                    quantidadeDisponivel: 0,
                    quantidadeReservada: 0,
                    historicos: new List<EstoqueHistoricoDbmodel>(),
                    peca: null!,
                    insumo: null!,
                    ativo: true),
            };

            foreach (var estoque in estoques)
            {
                if (!await context.Estoques.AnyAsync(e => e.Id == estoque.Id))
                    await context.Estoques.AddAsync(estoque);
            }

            await context.SaveChangesAsync();
        }
    }
}
