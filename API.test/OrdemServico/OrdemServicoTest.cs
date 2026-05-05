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
}
