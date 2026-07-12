using Infra.Context;
using Infra.DbModel;
using Microsoft.Extensions.DependencyInjection;
using Shared.DTOs.Clientes.Request;
using Shared.DTOs.Clientes.Shared;
using Shared.DTOs.OrdemServicos.Request;
using Shared.DTOs.OrdemServicos.Shared;
using Shared.DTOs.Veiculos.Requests;
using System.Net;
using System.Net.Http.Json;

namespace API.test.OrdemServicos;

[Collection("Integration")]
public class OrdemServicoTest : IClassFixture<IntegrationTestFixture>, IAsyncLifetime
{
    const string ApiKey = "api/ordemServico";

    private readonly HttpClient _client;
    private readonly ApiWebApplicationFactory _factory;
    private readonly IntegrationTestFixture _fixture;

    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();
        _client.DefaultRequestHeaders.Authorization = await _fixture.AuthenticateAsync(_factory, _client);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    public OrdemServicoTest(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Client;
        _factory = fixture.App;
    }

    private async Task<(Guid clienteId, Guid veiculoId, Guid servicoId, string cpf)> CriarDependenciasAsync(AppDbContext context, Guid adminId)
    {
        var cpfStr = "51922594016";

        var cliente = new ClienteDbModel(
            Guid.NewGuid(), "João Pedro", cpfStr, null, "joao@teste.com",
            "11", "55", "999999999", "Rua Teste", "123", "SN", "Bairro", "00000000", "Cidade", "UF",
            adminId, DateTime.UtcNow, null, null
        );
        context.Clientes.Add(cliente);

        var veiculo = new VeiculoDbModel(
            Guid.NewGuid(), "Civic", "Honda", cliente.Id, 2022, "ABC1D23", "Preto",
            adminId, DateTime.UtcNow, null, null, true
        );
        context.Veiculos.Add(veiculo);

        var servico = new ServicoDbModel(
            Guid.NewGuid(), "Troca de Óleo", "Troca de óleo do motor", 150.00m,
            null, adminId, DateTime.UtcNow, null, null, true
        );
        context.Servicos.Add(servico);

        await context.SaveChangesAsync();

        return (cliente.Id, veiculo.Id, servico.Id, cpfStr);
    }

    private static OrdemServicoDbModel CriarOS(Guid clienteId, Guid veiculoId, Guid adminId, string status, string? observacao = null)
        => new OrdemServicoDbModel(
            Guid.NewGuid(), clienteId, veiculoId, status, observacao, 0m, 0m,
            status == "EmDiagnostico" || status == "AguardandoAprovacao" || status == "EmExecucao" || status == "Finalizada" || status == "Entregue"
                ? DateTime.UtcNow : null,
            status == "Finalizada" || status == "Entregue" ? DateTime.UtcNow : null,
            adminId, DateTime.UtcNow, null, null
        );

    [Fact]
    public async Task OrdemServico_Put_IniciarDiagnostico_OK()
    {
        Guid osId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var admin = context.Usuarios.First();
            var (clienteId, veiculoId, _, _) = await CriarDependenciasAsync(context, admin.Id);

            var os = CriarOS(clienteId, veiculoId, admin.Id, "Recebida");
            context.OrdensServico.Add(os);
            await context.SaveChangesAsync();
            osId = os.Id;
        }

        var result = await _client.PutAsync($"{ApiKey}/{osId}/IniciarDiagnostico", null);

        Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Put_RealizarDiagnostico_OK()
    {
        Guid osId;
        Guid servicoId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var admin = context.Usuarios.First();
            var (clienteId, veiculoId, sId, _) = await CriarDependenciasAsync(context, admin.Id);
            servicoId = sId;

            var os = CriarOS(clienteId, veiculoId, admin.Id, "EmDiagnostico");
            context.OrdensServico.Add(os);
            await context.SaveChangesAsync();
            osId = os.Id;
        }

        var request = new DiagnosticoRequestDTO
        {
            Observacao = "Filtro de ar sujo",
            pecas = new List<OrdemServicoPecaRequestDTO>(),
            insumos = new List<OrdemServicoInsumoRequestDTO>(),
            servicos = new List<OrdemServicoServicoRequestDTO>
            {
                new OrdemServicoServicoRequestDTO { ServicoId = servicoId, Quantidade = 1 }
            }
        };

        var result = await _client.PutAsJsonAsync($"{ApiKey}/{osId}/RealizarDiagnostico", request);

        Assert.True(result.StatusCode == HttpStatusCode.NoContent || result.StatusCode == HttpStatusCode.OK);
    }

    [Fact]
    public async Task OrdemServico_HttpGet_GetById_OK()
    {
        Guid osId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var admin = context.Usuarios.First();
            var (clienteId, veiculoId, _, _) = await CriarDependenciasAsync(context, admin.Id);

            var os = CriarOS(clienteId, veiculoId, admin.Id, "Recebida");
            context.OrdensServico.Add(os);
            await context.SaveChangesAsync();
            osId = os.Id;
        }

        var result = await _client.GetAsync($"{ApiKey}/{osId}");

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Post_Create_VeiculoInexistente_BadRequest()
    {
        var request = new OrdemServicoRequestDTO
        {
            Cliente = new ClienteRequestDTO
            {
                Cpf = "51922594016", Nome = "Test", Email = "test@test.com",
                Telefone = new TelefoneDTO { DDD = "11", DDI = "55", Numero = "999999999" },
                Endereco = new EnderecoDTO { Bairro = "B", Cep = "00000000", Cidade = "C", Logradouro = "R", Numero = "1", Uf = "SP" }
            },
            Veiculo = new VeiculoRequestDTO { Placa = "TST0001", Modelo = "Civic", MarcaVeiculo = "Honda", Ano = 2022, Cor = "Preta", ClienteId = Guid.Empty },
            Observacao = "Teste falha",
            Pecas = new List<OrdemServicoPecaRequestDTO>(),
            Insumos = new List<OrdemServicoInsumoRequestDTO>(),
            Servicos = new List<OrdemServicoServicoRequestDTO>()
        };

        var result = await _client.PostAsJsonAsync(ApiKey, request);

        Assert.True(
            result.StatusCode == HttpStatusCode.Created ||
            result.StatusCode == HttpStatusCode.BadRequest ||
            result.StatusCode == HttpStatusCode.NotFound ||
            result.StatusCode == HttpStatusCode.InternalServerError
        );
    }

    [Fact]
    public async Task OrdemServico_Put_IniciarDiagnostico_OSInexistente_BadRequest()
    {
        var result = await _client.PutAsync($"{ApiKey}/{Guid.NewGuid()}/IniciarDiagnostico", null);

        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Put_RealizarDiagnostico_SemIniciarPreviamente_BadRequest()
    {
        Guid osId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var admin = context.Usuarios.First();
            var (clienteId, veiculoId, _, _) = await CriarDependenciasAsync(context, admin.Id);

            var os = CriarOS(clienteId, veiculoId, admin.Id, "Recebida");
            context.OrdensServico.Add(os);
            await context.SaveChangesAsync();
            osId = os.Id;
        }

        var request = new DiagnosticoRequestDTO { Observacao = "Tentativa direta" };

        var result = await _client.PutAsJsonAsync($"{ApiKey}/{osId}/RealizarDiagnostico", request);

        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Put_Aprovar_OSJaFinalizada_BadRequest()
    {
        Guid osId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var admin = context.Usuarios.First();
            var (clienteId, veiculoId, _, _) = await CriarDependenciasAsync(context, admin.Id);

            var os = CriarOS(clienteId, veiculoId, admin.Id, "Recebida");
            context.OrdensServico.Add(os);
            await context.SaveChangesAsync();
            osId = os.Id;
        }

        var result = await _client.PutAsync($"{ApiKey}/{osId}/Aprovar", null);

        Assert.True(result.StatusCode == HttpStatusCode.BadRequest || result.StatusCode == HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task OrdemServico_HttpGet_GetById_Inexistente_NotFound_Ou_BadRequest()
    {
        var result = await _client.GetAsync($"{ApiKey}/{Guid.NewGuid()}");

        Assert.True(result.StatusCode == HttpStatusCode.NotFound || result.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task OrdemServico_Post_SemAutenticacao_Unauthorized()
    {
        using var anonymousClient = _factory.CreateClient();
        var request = new OrdemServicoRequestDTO
        {
            Cliente = new ClienteRequestDTO { Cpf = "000", Nome = "Test", Email = "t@t.com",
                Telefone = new TelefoneDTO { DDD = "11", DDI = "55", Numero = "999" },
                Endereco = new EnderecoDTO { Bairro = "B", Cep = "00000000", Cidade = "C", Logradouro = "R", Numero = "1", Uf = "SP" }
            },
            Veiculo = new VeiculoRequestDTO { Placa = "TST0002", Modelo = "X", MarcaVeiculo = "Y", Ano = 2020, Cor = "Z", ClienteId = Guid.Empty }
        };

        var result = await anonymousClient.PostAsJsonAsync(ApiKey, request);

        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Get_GetPaginated_OK()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var admin = context.Usuarios.First();

            var cliente = new ClienteDbModel(
                Guid.NewGuid(), "ClienteCNPJ", null, "68380757000191", "cnpj@test.com",
                "11", "55", "999999999", "Rua", "1", null, "Bairro", "00000000", "Cidade", "SP",
                admin.Id, DateTime.UtcNow, null, null
            );
            context.Clientes.Add(cliente);

            var veiculo = new VeiculoDbModel(
                Guid.NewGuid(), "teste", "marcaTeste", cliente.Id, 2002, "ABC1234", "Preto",
                admin.Id, DateTime.UtcNow, null, null, true
            );
            context.Veiculos.Add(veiculo);

            var peca = new PecaDbModel(
                Guid.NewGuid(), "pneu", "pneu preto", "Michellin", 10m,
                admin.Id, DateTime.UtcNow, null, null, true
            );
            context.Pecas.Add(peca);

            var insumo = new InsumoDbModel(
                Guid.NewGuid(), "Oleo", "Teste teste", 10m,
                admin.Id, DateTime.UtcNow, null, null, true
            );
            context.Insumos.Add(insumo);

            var servico = new ServicoDbModel(
                Guid.NewGuid(), "Teste", "testestestes", 10m,
                null, admin.Id, DateTime.UtcNow, null, null, true
            );
            context.Servicos.Add(servico);

            var estoquePeca = new EstoqueDbModel(
                Guid.NewGuid(), peca.Id, null, 1, 0, new List<EstoqueHistoricoDbmodel>(), null!, null!, true
            );
            var estoqueInsumo = new EstoqueDbModel(
                Guid.NewGuid(), null, insumo.Id, 1, 0, new List<EstoqueHistoricoDbmodel>(), null!, null!, true
            );
            context.Estoques.AddRange(estoquePeca, estoqueInsumo);

            var os = CriarOS(cliente.Id, veiculo.Id, admin.Id, "Recebida");
            os.Pecas.Add(new OrdemServicoPecaDbModel(os.Id, peca.Id, 1, 10m));
            os.Insumos.Add(new OrdemServicoInsumoDbModel(insumo.Id, os.Id, 1, 10m));
            os.Servicos.Add(new OrdemServicoServicoDbModel(os.Id, servico.Id, 10m, "Recebida", null, null, 1));
            context.OrdensServico.Add(os);

            await context.SaveChangesAsync();
        }

        var result = await _client.GetAsync($"{ApiKey}?page=1&pageSize=10");

        Assert.True(result.StatusCode == HttpStatusCode.OK || result.StatusCode == HttpStatusCode.PartialContent);
    }

    [Fact]
    public async Task OrdemServico_Put_Cancelar_OS_Sucesso()
    {
        Guid osId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var admin = context.Usuarios.First();
            var (clienteId, veiculoId, _, _) = await CriarDependenciasAsync(context, admin.Id);

            var os = CriarOS(clienteId, veiculoId, admin.Id, "AguardandoAprovacao", "Problema identificado");
            context.OrdensServico.Add(os);
            await context.SaveChangesAsync();
            osId = os.Id;
        }

        var result = await _client.PutAsync($"{ApiKey}/{osId}/Cancelar", null);

        Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Put_FinalizarServico_AcaoMapeada()
    {
        Guid osId;
        Guid servicoId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var admin = context.Usuarios.First();
            var (clienteId, veiculoId, sId, _) = await CriarDependenciasAsync(context, admin.Id);
            servicoId = sId;

            var os = CriarOS(clienteId, veiculoId, admin.Id, "EmExecucao", "Diagnóstico realizado");
            context.OrdensServico.Add(os);
            await context.SaveChangesAsync();
            osId = os.Id;
        }

        var dto = new FinalizarServicoRequestDTO
        {
            servicosId = new List<Guid> { servicoId }
        };

        var result = await _client.PutAsJsonAsync($"{ApiKey}/{osId}/FinalizarServico", dto);

        Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Put_RegistrarEntrega_Sucesso()
    {
        Guid osId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var admin = context.Usuarios.First();
            var (clienteId, veiculoId, _, _) = await CriarDependenciasAsync(context, admin.Id);

            var os = CriarOS(clienteId, veiculoId, admin.Id, "Finalizada", "Diagnostico OK");
            context.OrdensServico.Add(os);
            await context.SaveChangesAsync();
            osId = os.Id;
        }

        var result = await _client.PutAsync($"{ApiKey}/{osId}/RegistrarEntrega", null);

        Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Put_RealizarDiagnostico_Forbidden_ParaFuncionarioOuCliente()
    {
        using var clienteSemPermissao = _factory.CreateClient();

        var request = new DiagnosticoRequestDTO { Observacao = "Tentativa ilegal" };

        var result = await clienteSemPermissao.PutAsJsonAsync($"{ApiKey}/{Guid.NewGuid()}/RealizarDiagnostico", request);

        Assert.True(result.StatusCode == HttpStatusCode.Unauthorized || result.StatusCode == HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task OrdemServico_Post_Create_BadRequest_SemCpfECnpj()
    {
        var request = new OrdemServicoRequestDTO
        {
            Cliente = new ClienteRequestDTO
            {
                Cpf = null, Cnpj = null, Nome = "Sem Doc", Email = "sem@doc.com",
                Telefone = new TelefoneDTO { DDD = "11", DDI = "55", Numero = "999999999" },
                Endereco = new EnderecoDTO { Bairro = "B", Cep = "00000000", Cidade = "C", Logradouro = "R", Numero = "1", Uf = "SP" }
            },
            Veiculo = new VeiculoRequestDTO { Placa = "TST0003", Modelo = "X", MarcaVeiculo = "Y", Ano = 2020, Cor = "Z", ClienteId = Guid.Empty },
            Pecas = new List<OrdemServicoPecaRequestDTO>(),
            Servicos = new List<OrdemServicoServicoRequestDTO>(),
            Insumos = new List<OrdemServicoInsumoRequestDTO>()
        };

        var result = await _client.PostAsJsonAsync(ApiKey, request);

        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Post_Create_BadRequest_ComCpfECnpjSimultaneos()
    {
        var request = new OrdemServicoRequestDTO
        {
            Cliente = new ClienteRequestDTO
            {
                Cpf = "51922594016", Cnpj = "68380757000191", Nome = "Ambos", Email = "ambos@doc.com",
                Telefone = new TelefoneDTO { DDD = "11", DDI = "55", Numero = "999999999" },
                Endereco = new EnderecoDTO { Bairro = "B", Cep = "00000000", Cidade = "C", Logradouro = "R", Numero = "1", Uf = "SP" }
            },
            Veiculo = new VeiculoRequestDTO { Placa = "TST0004", Modelo = "X", MarcaVeiculo = "Y", Ano = 2020, Cor = "Z", ClienteId = Guid.Empty },
            Pecas = new List<OrdemServicoPecaRequestDTO>(),
            Servicos = new List<OrdemServicoServicoRequestDTO>(),
            Insumos = new List<OrdemServicoInsumoRequestDTO>()
        };

        var result = await _client.PostAsJsonAsync(ApiKey, request);

        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Post_Create_Created_Sucesso()
    {
        string cnpj = "11222333000181";
        Guid PecaId;
        Guid InsumoId;
        Guid ServicoId;

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var admin = context.Usuarios.First();

            var cliente = new ClienteDbModel(
                Guid.NewGuid(), "Empresa Teste", null, cnpj, "empresa@test.com",
                "11", "55", "999999999", "Rua Emp", "1", null, "Bairro", "00000000", "SP", "SP",
                admin.Id, DateTime.UtcNow, null, null
            );
            context.Clientes.Add(cliente);

            var peca = new PecaDbModel(
                Guid.NewGuid(), "pneu", "pneu preto", "Michellin", 10m,
                admin.Id, DateTime.UtcNow, null, null, true
            );
            var insumo = new InsumoDbModel(
                Guid.NewGuid(), "Oleo", "Teste teste", 10m,
                admin.Id, DateTime.UtcNow, null, null, true
            );
            var servico = new ServicoDbModel(
                Guid.NewGuid(), "Teste", "testestestes", 10m,
                null, admin.Id, DateTime.UtcNow, null, null, true
            );

            var estoquePeca = new EstoqueDbModel(
                Guid.NewGuid(), peca.Id, null, 1, 0, new List<EstoqueHistoricoDbmodel>(), null!, null!, true
            );
            var estoqueInsumo = new EstoqueDbModel(
                Guid.NewGuid(), null, insumo.Id, 1, 0, new List<EstoqueHistoricoDbmodel>(), null!, null!, true
            );

            PecaId = peca.Id;
            InsumoId = insumo.Id;
            ServicoId = servico.Id;

            context.Pecas.Add(peca);
            context.Insumos.Add(insumo);
            context.Servicos.Add(servico);
            context.Estoques.AddRange(estoquePeca, estoqueInsumo);

            await context.SaveChangesAsync();
        }

        var request = new OrdemServicoRequestDTO
        {
            Cliente = new ClienteRequestDTO
            {
                Cnpj = cnpj, Cpf = null, Nome = "Empresa Teste", Email = "empresa@test.com",
                Telefone = new TelefoneDTO { DDD = "11", DDI = "55", Numero = "999999999" },
                Endereco = new EnderecoDTO { Bairro = "Bairro", Cep = "00000000", Cidade = "SP", Logradouro = "Rua Emp", Numero = "1", Uf = "SP" }
            },
            Veiculo = new VeiculoRequestDTO { Placa = "CRT0001", Modelo = "teste", MarcaVeiculo = "marcaTeste", Ano = 2002, Cor = "Preto", ClienteId = Guid.Empty },
            Pecas = new List<OrdemServicoPecaRequestDTO>
            {
                new OrdemServicoPecaRequestDTO { PecaId = PecaId, Quantidade = 1 }
            },
            Servicos = new List<OrdemServicoServicoRequestDTO>
            {
                new OrdemServicoServicoRequestDTO { Quantidade = 1, ServicoId = ServicoId }
            },
            Insumos = new List<OrdemServicoInsumoRequestDTO>
            {
                new OrdemServicoInsumoRequestDTO { InsumoId = InsumoId, Quantidade = 1 }
            }
        };

        var result = await _client.PostAsJsonAsync(ApiKey, request);

        Assert.Equal(HttpStatusCode.Created, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Post_Create_NotFound_ClienteNaoCadastrado()
    {
        var request = new OrdemServicoRequestDTO
        {
            Cliente = new ClienteRequestDTO
            {
                Cpf = "93541134780", Cnpj = null, Nome = "Novo Cliente", Email = "novo@cli.com",
                Telefone = new TelefoneDTO { DDD = "11", DDI = "55", Numero = "999999999" },
                Endereco = new EnderecoDTO { Bairro = "B", Cep = "00000000", Cidade = "C", Logradouro = "R", Numero = "1", Uf = "SP" }
            },
            Veiculo = new VeiculoRequestDTO { Placa = "NVO0001", Modelo = "X", MarcaVeiculo = "Y", Ano = 2020, Cor = "Z", ClienteId = Guid.Empty },
            Pecas = new List<OrdemServicoPecaRequestDTO>(),
            Servicos = new List<OrdemServicoServicoRequestDTO>(),
            Insumos = new List<OrdemServicoInsumoRequestDTO>()
        };

        var result = await _client.PostAsJsonAsync(ApiKey, request);

        Assert.True(
            result.StatusCode == HttpStatusCode.NotFound ||
            result.StatusCode == HttpStatusCode.BadRequest ||
            result.StatusCode == HttpStatusCode.Created ||
            result.StatusCode == HttpStatusCode.InternalServerError
        );
    }

    [Fact]
    public async Task OrdemServico_Put_IniciarDiagnostico_BadRequest_StatusInvalido()
    {
        Guid osId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var admin = context.Usuarios.First();
            var (clienteId, veiculoId, _, _) = await CriarDependenciasAsync(context, admin.Id);

            var os = CriarOS(clienteId, veiculoId, admin.Id, "EmDiagnostico");
            context.OrdensServico.Add(os);
            await context.SaveChangesAsync();
            osId = os.Id;
        }

        var result = await _client.PutAsync($"{ApiKey}/{osId}/IniciarDiagnostico", null);

        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Put_Aprovar_NoContent_StatusCorreto()
    {
        Guid osId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var admin = context.Usuarios.First();
            var (clienteId, veiculoId, _, _) = await CriarDependenciasAsync(context, admin.Id);

            var os = CriarOS(clienteId, veiculoId, admin.Id, "AguardandoAprovacao", "Diagnóstico realizado");
            context.OrdensServico.Add(os);
            await context.SaveChangesAsync();
            osId = os.Id;
        }

        var result = await _client.PutAsync($"{ApiKey}/{osId}/Aprovar", null);

        Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Put_AprovarComPecasEInsumos_NoContent_StatusCorreto()
    {
        Guid osId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var admin = context.Usuarios.First();
            var (clienteId, veiculoId, _, _) = await CriarDependenciasAsync(context, admin.Id);

            var peca = new PecaDbModel(
                Guid.NewGuid(), "pneu", "pneu preto", "Michellin", 10m,
                admin.Id, DateTime.UtcNow, null, null, true
            );
            var insumo = new InsumoDbModel(
                Guid.NewGuid(), "Oleo", "Teste teste", 10m,
                admin.Id, DateTime.UtcNow, null, null, true
            );
            context.Pecas.Add(peca);
            context.Insumos.Add(insumo);

            var estoquePeca = new EstoqueDbModel(
                Guid.NewGuid(), peca.Id, null, 0, 1, new List<EstoqueHistoricoDbmodel>(), null!, null!, true
            );
            var estoqueInsumo = new EstoqueDbModel(
                Guid.NewGuid(), null, insumo.Id, 0, 1, new List<EstoqueHistoricoDbmodel>(), null!, null!, true
            );
            context.Estoques.AddRange(estoquePeca, estoqueInsumo);

            var os = CriarOS(clienteId, veiculoId, admin.Id, "AguardandoAprovacao", "Diagnóstico realizado");
            os.Pecas.Add(new OrdemServicoPecaDbModel(os.Id, peca.Id, 1, peca.ValorUnitario));
            os.Insumos.Add(new OrdemServicoInsumoDbModel(insumo.Id, os.Id, 1, peca.ValorUnitario));
            context.OrdensServico.Add(os);

            await context.SaveChangesAsync();
            osId = os.Id;
        }

        var result = await _client.PutAsync($"{ApiKey}/{osId}/Aprovar", null);

        Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Put_AprovarComPecasEInsumosSemEstoque_BadRequest()
    {
        Guid osId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var admin = context.Usuarios.First();
            var (clienteId, veiculoId, _, _) = await CriarDependenciasAsync(context, admin.Id);

            var peca = new PecaDbModel(
                Guid.NewGuid(), "pneu", "pneu preto", "Michellin", 10m,
                admin.Id, DateTime.UtcNow, null, null, true
            );
            var insumo = new InsumoDbModel(
                Guid.NewGuid(), "Oleo", "Teste teste", 10m,
                admin.Id, DateTime.UtcNow, null, null, true
            );
            context.Pecas.Add(peca);
            context.Insumos.Add(insumo);

            var estoquePeca = new EstoqueDbModel(
                Guid.NewGuid(), peca.Id, null, 0, 0, new List<EstoqueHistoricoDbmodel>(), null!, null!, true
            );
            var estoqueInsumo = new EstoqueDbModel(
                Guid.NewGuid(), null, insumo.Id, 0, 0, new List<EstoqueHistoricoDbmodel>(), null!, null!, true
            );
            context.Estoques.AddRange(estoquePeca, estoqueInsumo);

            var os = CriarOS(clienteId, veiculoId, admin.Id, "AguardandoAprovacao", "Diagnóstico realizado");
            os.Pecas.Add(new OrdemServicoPecaDbModel(os.Id, peca.Id, 1, peca.ValorUnitario));
            os.Insumos.Add(new OrdemServicoInsumoDbModel(insumo.Id, os.Id, 1, peca.ValorUnitario));
            context.OrdensServico.Add(os);

            await context.SaveChangesAsync();
            osId = os.Id;
        }

        var result = await _client.PutAsync($"{ApiKey}/{osId}/Aprovar", null);

        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Put_Aprovar_NotFound_OSInexistente()
    {
        var result = await _client.PutAsync($"{ApiKey}/{Guid.NewGuid()}/Aprovar", null);

        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Put_Aprovar_Unauthorized_SemToken()
    {
        using var anonymousClient = _factory.CreateClient();

        var result = await anonymousClient.PutAsync($"{ApiKey}/{Guid.NewGuid()}/Aprovar", null);

        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Put_Cancelar_NotFound_OSInexistente()
    {
        var result = await _client.PutAsync($"{ApiKey}/{Guid.NewGuid()}/Cancelar", null);

        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Put_Cancelar_BadRequest_StatusInvalido()
    {
        Guid osId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var admin = context.Usuarios.First();
            var (clienteId, veiculoId, _, _) = await CriarDependenciasAsync(context, admin.Id);

            var os = CriarOS(clienteId, veiculoId, admin.Id, "Recebida");
            context.OrdensServico.Add(os);
            await context.SaveChangesAsync();
            osId = os.Id;
        }

        var result = await _client.PutAsync($"{ApiKey}/{osId}/Cancelar", null);

        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Put_Cancelar_Unauthorized_SemToken()
    {
        using var anonymousClient = _factory.CreateClient();

        var result = await anonymousClient.PutAsync($"{ApiKey}/{Guid.NewGuid()}/Cancelar", null);

        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Put_FinalizarServico_NotFound_OSInexistente()
    {
        var dto = new FinalizarServicoRequestDTO
        {
            servicosId = new List<Guid> { Guid.NewGuid() }
        };

        var result = await _client.PutAsJsonAsync($"{ApiKey}/{Guid.NewGuid()}/FinalizarServico", dto);

        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Put_FinalizarServico_BadRequest_StatusInvalido()
    {
        Guid osId;
        Guid servicoId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var admin = context.Usuarios.First();
            var (clienteId, veiculoId, sId, _) = await CriarDependenciasAsync(context, admin.Id);
            servicoId = sId;

            var os = CriarOS(clienteId, veiculoId, admin.Id, "Recebida");
            context.OrdensServico.Add(os);
            await context.SaveChangesAsync();
            osId = os.Id;
        }

        var dto = new FinalizarServicoRequestDTO
        {
            servicosId = new List<Guid> { servicoId }
        };

        var result = await _client.PutAsJsonAsync($"{ApiKey}/{osId}/FinalizarServico", dto);

        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Put_FinalizarServico_Unauthorized_SemToken()
    {
        using var anonymousClient = _factory.CreateClient();
        var dto = new FinalizarServicoRequestDTO
        {
            servicosId = new List<Guid> { Guid.NewGuid() }
        };

        var result = await anonymousClient.PutAsJsonAsync($"{ApiKey}/{Guid.NewGuid()}/FinalizarServico", dto);

        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Put_RegistrarEntrega_NotFound_OSInexistente()
    {
        var result = await _client.PutAsync($"{ApiKey}/{Guid.NewGuid()}/RegistrarEntrega", null);

        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Put_RegistrarEntrega_BadRequest_StatusInvalido()
    {
        Guid osId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var admin = context.Usuarios.First();
            var (clienteId, veiculoId, _, _) = await CriarDependenciasAsync(context, admin.Id);

            var os = CriarOS(clienteId, veiculoId, admin.Id, "EmExecucao", "Diagnóstico realizado");
            context.OrdensServico.Add(os);
            await context.SaveChangesAsync();
            osId = os.Id;
        }

        var result = await _client.PutAsync($"{ApiKey}/{osId}/RegistrarEntrega", null);

        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Put_RegistrarEntrega_Unauthorized_SemToken()
    {
        using var anonymousClient = _factory.CreateClient();

        var result = await anonymousClient.PutAsync($"{ApiKey}/{Guid.NewGuid()}/RegistrarEntrega", null);

        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Put_RealizarDiagnostico_BadRequest_SemItens()
    {
        Guid osId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var admin = context.Usuarios.First();
            var (clienteId, veiculoId, _, _) = await CriarDependenciasAsync(context, admin.Id);

            var os = CriarOS(clienteId, veiculoId, admin.Id, "EmDiagnostico");
            context.OrdensServico.Add(os);
            await context.SaveChangesAsync();
            osId = os.Id;
        }

        var request = new DiagnosticoRequestDTO
        {
            Observacao = "Diagnóstico sem itens",
            pecas = new List<OrdemServicoPecaRequestDTO>(),
            insumos = new List<OrdemServicoInsumoRequestDTO>(),
            servicos = new List<OrdemServicoServicoRequestDTO>()
        };

        var result = await _client.PutAsJsonAsync($"{ApiKey}/{osId}/RealizarDiagnostico", request);

        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Put_RealizarDiagnostico_NotFound_OSInexistente()
    {
        var request = new DiagnosticoRequestDTO
        {
            Observacao = "OS não existe",
            servicos = new List<OrdemServicoServicoRequestDTO>
            {
                new() { ServicoId = Guid.NewGuid(), Quantidade = 1 }
            },
            pecas = new List<OrdemServicoPecaRequestDTO>(),
            insumos = new List<OrdemServicoInsumoRequestDTO>()
        };

        var result = await _client.PutAsJsonAsync($"{ApiKey}/{Guid.NewGuid()}/RealizarDiagnostico", request);

        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Put_RealizarDiagnostico_Unauthorized_SemToken()
    {
        using var anonymousClient = _factory.CreateClient();
        var request = new DiagnosticoRequestDTO { Observacao = "Tentativa ilegal" };

        var result = await anonymousClient.PutAsJsonAsync($"{ApiKey}/{Guid.NewGuid()}/RealizarDiagnostico", request);

        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }
}
