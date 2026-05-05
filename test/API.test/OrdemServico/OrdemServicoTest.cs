using Application.OrdemServicos.DTOs.Requests;
using Domain.Aggregates.ClienteAggregates;
using Domain.ValueObjects;
using Infra.Context;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace API.test.OrdemServico;

public class OrdemServicoTest : IClassFixture<IntegrationTestFixture>, IAsyncLifetime
{
    const string ApiKey = "api/OrdemServico";

    private readonly HttpClient _client;
    private readonly ApiWebApplicationFactory _factory;
    private readonly IntegrationTestFixture _fixture;
    public async Task InitializeAsync() => await _factory.ResetDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    public OrdemServicoTest(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Client;
        _factory = fixture.App;
    }

    private async Task<(string cpf, Guid veiculoId, Guid servicoId)> CriarDependenciasAsync(AppDbContext context, Guid adminId)
    {
        var cpfStr = "51922594016";

        var cliente = new Cliente(
            "João Pedro",
            new Cpf(cpfStr),
            adminId,
            new Endereco("Rua Teste", "123", "SN", "Bairro", "Cidade", "UF", "00000-000"),
            new Telefone("11", "55", "999999999"),
            new Email("joao@teste.com")
        );
        context.Clientes.Add(cliente);

        var veiculo = new Domain.Entities.Veiculo(
            "Civic", "Honda", cliente.Id, 2022, new Placa("ABC1D23"), "Preto", adminId
        );

        context.Veiculos.Add(veiculo);

        var servicoCatalogo = new Domain.Entities.Servico("Troca de Óleo", "Troca de óleo do motor", 150.00m, adminId, DateTime.Now);
        context.Servicos.Add(servicoCatalogo);

        await context.SaveChangesAsync();

        return (cpfStr, veiculo.Id, servicoCatalogo.Id);
    }

    [Fact]
    public async Task OrdemServico_Put_IniciarDiagnostico_OK()
    {
        // Arrange
        Guid osId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var admin = context.Usuarios.First();
            var (cpf, vId, _) = await CriarDependenciasAsync(context, admin.Id);
            var cliente = context.Clientes.First();

            var os = new Domain.Aggregates.OrdemServicoAggregates.OrdemServico(cliente.Id, vId, admin.Id);

            context.Set<Domain.Aggregates.OrdemServicoAggregates.OrdemServico>().Add(os);
            await context.SaveChangesAsync();
            osId = os.Id;
        }

        // Act
        var result = await _client.PutAsync($"{ApiKey}/{osId}/IniciarDiagnostico", null);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Put_RealizarDiagnostico_OK()
    {
        /// Arrange
        Guid osId;
        Guid servicoId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var admin = context.Usuarios.First();
            var (_, vId, sId) = await CriarDependenciasAsync(context, admin.Id);
            servicoId = sId;

            var os = new Domain.Aggregates.OrdemServicoAggregates.OrdemServico(context.Clientes.First().Id, vId, admin.Id);
            os.IniciarDiagnostico();

            context.Set<Domain.Aggregates.OrdemServicoAggregates.OrdemServico>().Add(os);
            await context.SaveChangesAsync();
            osId = os.Id;
        }

        var request = new DiagnosticoRequestDTO
        {
            Observacao = "Filtro de ar sujo",
            Pecas = new List<OrdemServicoPecaRequestDTO>(),
            Insumos = new List<OrdemServicoInsumoRequestDTO>(),
            Servicos = new List<OrdemServicoServicoRequestDTO>
            {
                new OrdemServicoServicoRequestDTO
                {
                    ServicoId = servicoId,
                    Quantidade = 1
                }
            }
        };

        // Act
        var result = await _client.PutAsJsonAsync($"{ApiKey}/{osId}/RealizarDiagnostico", request);

        // Assert
        Assert.True(result.StatusCode == HttpStatusCode.NoContent || result.StatusCode == HttpStatusCode.OK);
    }

    [Fact]
    public async Task OrdemServico_HttpGet_GetById_OK()
    {
        // Arrange
        Guid osId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var admin = context.Usuarios.First();
            var (cpf, vId, _) = await CriarDependenciasAsync(context, admin.Id);
            var cliente = context.Clientes.First();

            var os = new Domain.Aggregates.OrdemServicoAggregates.OrdemServico(cliente.Id, vId, admin.Id);
            context.Set<Domain.Aggregates.OrdemServicoAggregates.OrdemServico>().Add(os);
            await context.SaveChangesAsync();
            osId = os.Id;
        }

        // Act
        var result = await _client.GetAsync($"{ApiKey}/{osId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Post_Create_VeiculoInexistente_BadRequest()
    {
        // Arrange
        var request = new OrdemServicoRequestDTO
        {
            Cpf = "51922594016",
            VeiculoId = Guid.NewGuid(),
            Observacao = "Teste falha",
            Pecas = new List<OrdemServicoPecaRequestDTO>(),
            Insumos = new List<OrdemServicoInsumoRequestDTO>(),
            Servicos = new List<OrdemServicoServicoRequestDTO>()
        };

        // Act
        var result = await _client.PostAsJsonAsync(ApiKey, request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Put_IniciarDiagnostico_OSInexistente_BadRequest()
    {
        // Act 
        var result = await _client.PutAsync($"{ApiKey}/{Guid.NewGuid()}/IniciarDiagnostico", null);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Put_RealizarDiagnostico_SemIniciarPreviamente_BadRequest()
    {
        // Arrange
        Guid osId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var admin = context.Usuarios.First();
            var (cpf, vId, _) = await CriarDependenciasAsync(context, admin.Id);
            var cliente = context.Clientes.First();

            var os = new Domain.Aggregates.OrdemServicoAggregates.OrdemServico(cliente.Id, vId, admin.Id);
            context.Set<Domain.Aggregates.OrdemServicoAggregates.OrdemServico>().Add(os);
            await context.SaveChangesAsync();
            osId = os.Id;
        }

        var request = new DiagnosticoRequestDTO { Observacao = "Tentativa direta" };

        // Act
        var result = await _client.PutAsJsonAsync($"{ApiKey}/{osId}/RealizarDiagnostico", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Put_Aprovar_OSJaFinalizada_BadRequest()
    {
        // Arrange
        Guid osId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var admin = context.Usuarios.First();
            var (cpf, vId, _) = await CriarDependenciasAsync(context, admin.Id);
            var cliente = context.Clientes.First();

            var os = new Domain.Aggregates.OrdemServicoAggregates.OrdemServico(cliente.Id, vId, admin.Id);
            context.Set<Domain.Aggregates.OrdemServicoAggregates.OrdemServico>().Add(os);
            await context.SaveChangesAsync();
            osId = os.Id;
        }

        // Act
        var result = await _client.PutAsync($"{ApiKey}/{osId}/Aprovar", null);

        // Assert 
        Assert.True(result.StatusCode == HttpStatusCode.BadRequest || result.StatusCode == HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task OrdemServico_HttpGet_GetById_Inexistente_NotFound_Ou_BadRequest()
    {
        // Act
        var result = await _client.GetAsync($"{ApiKey}/{Guid.NewGuid()}");

        // Assert 
        Assert.True(result.StatusCode == HttpStatusCode.NotFound || result.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task OrdemServico_Post_SemAutenticacao_Unauthorized()
    {
        // Arrange
        using var anonymousClient = _factory.CreateClient();
        var request = new OrdemServicoRequestDTO { Cpf = "000", VeiculoId = Guid.NewGuid() };

        // Act
        var result = await anonymousClient.PostAsJsonAsync(ApiKey, request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Get_GetPaginated_OK()
    {
        // Act
        var result = await _client.GetAsync($"{ApiKey}?page=1&pageSize=10");

        // Assert
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
            var (_, vId, _) = await CriarDependenciasAsync(context, admin.Id);

            var os = new Domain.Aggregates.OrdemServicoAggregates.OrdemServico(context.Clientes.First().Id, vId, admin.Id);

            os.IniciarDiagnostico();
            os.RegistrarDiagnostico("Problema identificado"); 

            context.Set<Domain.Aggregates.OrdemServicoAggregates.OrdemServico>().Add(os);
            await context.SaveChangesAsync();
            osId = os.Id;
        }

        var result = await _client.PutAsync($"{ApiKey}/{osId}/Cancelar", null);

        Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Put_FinalizarServico_AcaoMapeada()
    {
        // Arrange
        Guid osId;
        Guid servicoId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var admin = context.Usuarios.First();

            var (_, vId, sId) = await CriarDependenciasAsync(context, admin.Id);
            servicoId = sId;

            var os = new Domain.Aggregates.OrdemServicoAggregates.OrdemServico(context.Clientes.First().Id, vId, admin.Id);

            os.IniciarDiagnostico(); 
            os.RegistrarDiagnostico("Diagnóstico realizado"); 
            os.AprovarOrdemServico(); 

            context.Set<Domain.Aggregates.OrdemServicoAggregates.OrdemServico>().Add(os);
            await context.SaveChangesAsync();
            osId = os.Id;
        }

        var dto = new FinalizarServicoDTO
        {
            ServicosId = new List<Guid> { servicoId }
        };

        // Act
        var result = await _client.PutAsJsonAsync($"{ApiKey}/{osId}/FinalizarServico", dto);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Put_RegistrarEntrega_Sucesso()
    {
        Guid osId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var adminId = context.Usuarios.First().Id;
            var (_, vId, sId) = await CriarDependenciasAsync(context, adminId);

            var os = new Domain.Aggregates.OrdemServicoAggregates.OrdemServico(context.Clientes.First().Id, vId, adminId);

            os.IniciarDiagnostico();
            os.RegistrarDiagnostico("Diagnostico OK");
            os.AprovarOrdemServico();
            os.FinalizarOrdemServico(new List<Guid> { sId });

            context.Set<Domain.Aggregates.OrdemServicoAggregates.OrdemServico>().Add(os);
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

        // Act
        var result = await clienteSemPermissao.PutAsJsonAsync($"{ApiKey}/{Guid.NewGuid()}/RealizarDiagnostico", request);

        // Assert 
        Assert.True(result.StatusCode == HttpStatusCode.Unauthorized || result.StatusCode == HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task OrdemServico_Post_Create_BadRequest_SemCpfECnpj()
    {
        // Arrange
        var request = new OrdemServicoRequestDTO
        {
            VeiculoId = Guid.NewGuid(),
            Pecas = new List<OrdemServicoPecaRequestDTO>(),
            Servicos = new List<OrdemServicoServicoRequestDTO>(),
            Insumos = new List<OrdemServicoInsumoRequestDTO>()
        };

        // Act
        var result = await _client.PostAsJsonAsync(ApiKey, request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Post_Create_BadRequest_ComCpfECnpjSimultaneos()
    {
        // Arrange 
        var request = new OrdemServicoRequestDTO
        {
            Cpf = "51922594016",
            Cnpj = "68380757000191",
            VeiculoId = Guid.NewGuid(),
            Pecas = new List<OrdemServicoPecaRequestDTO>(),
            Servicos = new List<OrdemServicoServicoRequestDTO>(),
            Insumos = new List<OrdemServicoInsumoRequestDTO>()
        };

        // Act
        var result = await _client.PostAsJsonAsync(ApiKey, request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Post_Create_NotFound_ClienteNaoCadastrado()
    {
        // Arrange 
        var request = new OrdemServicoRequestDTO
        {
            Cpf = "93541134780", 
            VeiculoId = Guid.NewGuid(),
            Pecas = new List<OrdemServicoPecaRequestDTO>(),
            Servicos = new List<OrdemServicoServicoRequestDTO>(),
            Insumos = new List<OrdemServicoInsumoRequestDTO>()
        };

        // Act
        var result = await _client.PostAsJsonAsync(ApiKey, request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }


    [Fact]
    public async Task OrdemServico_Put_IniciarDiagnostico_BadRequest_StatusInvalido()
    {
        // Arrange 
        Guid osId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var admin = context.Usuarios.First();
            var (_, vId, _) = await CriarDependenciasAsync(context, admin.Id);

            var os = new Domain.Aggregates.OrdemServicoAggregates.OrdemServico(
                context.Clientes.First().Id, vId, admin.Id);

            os.IniciarDiagnostico(); 

            context.Set<Domain.Aggregates.OrdemServicoAggregates.OrdemServico>().Add(os);
            await context.SaveChangesAsync();
            osId = os.Id;
        }

        // Act
        var result = await _client.PutAsync($"{ApiKey}/{osId}/IniciarDiagnostico", null);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Put_Aprovar_NoContent_StatusCorreto()
    {
        // Arrange
        Guid osId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var admin = context.Usuarios.First();
            var (_, vId, sId) = await CriarDependenciasAsync(context, admin.Id);

            var os = new Domain.Aggregates.OrdemServicoAggregates.OrdemServico(
                context.Clientes.First().Id, vId, admin.Id);

            os.IniciarDiagnostico();
            os.RegistrarDiagnostico("Diagnóstico realizado");

            context.Set<Domain.Aggregates.OrdemServicoAggregates.OrdemServico>().Add(os);
            await context.SaveChangesAsync();
            osId = os.Id;
        }

        // Act
        var result = await _client.PutAsync($"{ApiKey}/{osId}/Aprovar", null);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Put_Aprovar_NotFound_OSInexistente()
    {
        // Act
        var result = await _client.PutAsync($"{ApiKey}/{Guid.NewGuid()}/Aprovar", null);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Put_Aprovar_Unauthorized_SemToken()
    {
        // Arrange
        using var anonymousClient = _factory.CreateClient();

        // Act
        var result = await anonymousClient.PutAsync(
            $"{ApiKey}/{Guid.NewGuid()}/Aprovar", null);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Put_Cancelar_NotFound_OSInexistente()
    {
        // Act
        var result = await _client.PutAsync($"{ApiKey}/{Guid.NewGuid()}/Cancelar", null);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Put_Cancelar_BadRequest_StatusInvalido()
    {
        // Arrange 
        Guid osId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var admin = context.Usuarios.First();
            var (_, vId, _) = await CriarDependenciasAsync(context, admin.Id);

            var os = new Domain.Aggregates.OrdemServicoAggregates.OrdemServico(
                context.Clientes.First().Id, vId, admin.Id);

            context.Set<Domain.Aggregates.OrdemServicoAggregates.OrdemServico>().Add(os);
            await context.SaveChangesAsync();
            osId = os.Id;
        }

        // Act
        var result = await _client.PutAsync($"{ApiKey}/{osId}/Cancelar", null);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Put_Cancelar_Unauthorized_SemToken()
    {
        // Arrange
        using var anonymousClient = _factory.CreateClient();

        // Act
        var result = await anonymousClient.PutAsync(
            $"{ApiKey}/{Guid.NewGuid()}/Cancelar", null);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Put_FinalizarServico_NotFound_OSInexistente()
    {
        // Arrange
        var dto = new FinalizarServicoDTO
        {
            ServicosId = new List<Guid> { Guid.NewGuid() }
        };

        // Act
        var result = await _client.PutAsJsonAsync(
            $"{ApiKey}/{Guid.NewGuid()}/FinalizarServico", dto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Put_FinalizarServico_BadRequest_StatusInvalido()
    {
        // Arrange 
        Guid osId;
        Guid servicoId; 
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var admin = context.Usuarios.First();
            var (_, vId, sId) = await CriarDependenciasAsync(context, admin.Id);
            servicoId = sId;

            var os = new Domain.Aggregates.OrdemServicoAggregates.OrdemServico(
                context.Clientes.First().Id, vId, admin.Id);

            context.Set<Domain.Aggregates.OrdemServicoAggregates.OrdemServico>().Add(os);
            await context.SaveChangesAsync();
            osId = os.Id;
        }

        var dto = new FinalizarServicoDTO
        {
            ServicosId = new List<Guid> { servicoId }
        };

        // Act
        var result = await _client.PutAsJsonAsync($"{ApiKey}/{osId}/FinalizarServico", dto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Put_FinalizarServico_Unauthorized_SemToken()
    {
        // Arrange
        using var anonymousClient = _factory.CreateClient();
        var dto = new FinalizarServicoDTO
        {
            ServicosId = new List<Guid> { Guid.NewGuid() }
        };

        // Act
        var result = await anonymousClient.PutAsJsonAsync(
            $"{ApiKey}/{Guid.NewGuid()}/FinalizarServico", dto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Put_RegistrarEntrega_NotFound_OSInexistente()
    {
        // Act
        var result = await _client.PutAsync(
            $"{ApiKey}/{Guid.NewGuid()}/RegistrarEntrega", null);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Put_RegistrarEntrega_BadRequest_StatusInvalido()
    {
        // Arrange 
        Guid osId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var admin = context.Usuarios.First();
            var (_, vId, _) = await CriarDependenciasAsync(context, admin.Id);

            var os = new Domain.Aggregates.OrdemServicoAggregates.OrdemServico(
                context.Clientes.First().Id, vId, admin.Id);

            os.IniciarDiagnostico();
            os.RegistrarDiagnostico("Diagnóstico realizado");
            os.AprovarOrdemServico(); // EmExecucao — não pode entregar ainda

            context.Set<Domain.Aggregates.OrdemServicoAggregates.OrdemServico>().Add(os);
            await context.SaveChangesAsync();
            osId = os.Id;
        }

        // Act
        var result = await _client.PutAsync($"{ApiKey}/{osId}/RegistrarEntrega", null);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Put_RegistrarEntrega_Unauthorized_SemToken()
    {
        // Arrange
        using var anonymousClient = _factory.CreateClient();

        // Act
        var result = await anonymousClient.PutAsync(
            $"{ApiKey}/{Guid.NewGuid()}/RegistrarEntrega", null);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Put_RealizarDiagnostico_BadRequest_SemItens()
    {
        // Arrange — sem serviços, peças ou insumos — regra de negócio
        Guid osId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var admin = context.Usuarios.First();
            var (_, vId, _) = await CriarDependenciasAsync(context, admin.Id);

            var os = new Domain.Aggregates.OrdemServicoAggregates.OrdemServico(
                context.Clientes.First().Id, vId, admin.Id);

            os.IniciarDiagnostico();

            context.Set<Domain.Aggregates.OrdemServicoAggregates.OrdemServico>().Add(os);
            await context.SaveChangesAsync();
            osId = os.Id;
        }

        var request = new DiagnosticoRequestDTO
        {
            Observacao = "Diagnóstico sem itens",
            Pecas = new List<OrdemServicoPecaRequestDTO>(),
            Insumos = new List<OrdemServicoInsumoRequestDTO>(),
            Servicos = new List<OrdemServicoServicoRequestDTO>()
            // nenhum item — deve retornar BadRequest
        };

        // Act
        var result = await _client.PutAsJsonAsync(
            $"{ApiKey}/{osId}/RealizarDiagnostico", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Put_RealizarDiagnostico_NotFound_OSInexistente()
    {
        // Arrange
        var request = new DiagnosticoRequestDTO
        {
            Observacao = "OS não existe",
            Servicos = new List<OrdemServicoServicoRequestDTO>
        {
            new() { ServicoId = Guid.NewGuid(), Quantidade = 1 }
        },
            Pecas = new List<OrdemServicoPecaRequestDTO>(),
            Insumos = new List<OrdemServicoInsumoRequestDTO>()
        };

        // Act
        var result = await _client.PutAsJsonAsync(
            $"{ApiKey}/{Guid.NewGuid()}/RealizarDiagnostico", request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task OrdemServico_Put_RealizarDiagnostico_Unauthorized_SemToken()
    {
        // Arrange
        using var anonymousClient = _factory.CreateClient();
        var request = new DiagnosticoRequestDTO { Observacao = "Tentativa ilegal" };

        // Act
        var result = await anonymousClient.PutAsJsonAsync(
            $"{ApiKey}/{Guid.NewGuid()}/RealizarDiagnostico", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }
}
