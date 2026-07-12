using Infra.Context;
using Infra.DbModel;
using Microsoft.Extensions.DependencyInjection;
using Shared.DTOs.Veiculos.Requests;
using System.Net;
using System.Net.Http.Json;

namespace API.test.Veiculos;

[Collection("Integration")]
public class VeiculoTest : IClassFixture<IntegrationTestFixture>, IAsyncLifetime
{
    const string ApiKey = "api/veiculos";

    private readonly HttpClient _client;
    private readonly ApiWebApplicationFactory _factory;
    private readonly IntegrationTestFixture _fixture;

    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();
        _client.DefaultRequestHeaders.Authorization = await _fixture.AuthenticateAsync(_factory, _client);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    public VeiculoTest(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Client;
        _factory = fixture.App;
    }

    private async Task<(Guid clienteId, Guid adminId)> CriarClienteAsync(string cpf = "72814249061")
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var admin = context.Usuarios.First();
        var cliente = new ClienteDbModel(
            Guid.NewGuid(), "João Silva", cpf, null, "joao@email.com",
            "11", "55", "988887777", "Avenida Paulista", "1000", "SN", "Bela Vista", "01310100", "São Paulo", "SP",
            admin.Id, DateTime.UtcNow, null, null
        );
        context.Clientes.Add(cliente);
        await context.SaveChangesAsync();
        return (cliente.Id, admin.Id);
    }

    [Fact]
    public async Task Veiculo_Post_Create_Created()
    {
        var (clienteId, _) = await CriarClienteAsync();

        var request = new VeiculoRequestDTO
        {
            Modelo = "Civic",
            MarcaVeiculo = "Honda",
            ClienteId = clienteId,
            Ano = 2022,
            Placa = "ABC1D23",
            Cor = "Prata"
        };

        var result = await _client.PostAsJsonAsync(ApiKey, request);

        Assert.Equal(HttpStatusCode.Created, result.StatusCode);
    }

    [Fact]
    public async Task Veiculo_Post_Create_BadRequest()
    {
        var (clienteId, _) = await CriarClienteAsync();

        var request = new VeiculoRequestDTO
        {
            Modelo = "Civic",
            MarcaVeiculo = "Honda",
            ClienteId = clienteId,
            Ano = 1800,
            Placa = "ABC1D23",
            Cor = "Prata"
        };

        var result = await _client.PostAsJsonAsync(ApiKey, request);

        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task Veiculo_Get_GetById_OK()
    {
        Guid veiculoId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var admin = context.Usuarios.First();

            var cliente = new ClienteDbModel(
                Guid.NewGuid(), "João Silva", "72814249061", null, "joao@email.com",
                "11", "55", "988887777", "Avenida Paulista", "1000", "SN", "Bela Vista", "01310100", "São Paulo", "SP",
                admin.Id, DateTime.UtcNow, null, null
            );
            context.Clientes.Add(cliente);

            var veiculo = new VeiculoDbModel(
                Guid.NewGuid(), "Corolla", "Toyota", cliente.Id, 2023, "BRA2E19", "Preto",
                admin.Id, DateTime.UtcNow, null, null, true
            );
            context.Veiculos.Add(veiculo);
            await context.SaveChangesAsync();
            veiculoId = veiculo.Id;
        }

        var result = await _client.GetAsync($"{ApiKey}/{veiculoId}");

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
    }

    [Fact]
    public async Task Veiculo_Get_GetPaginated_PartialContent()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var admin = context.Usuarios.First();

            var cliente = new ClienteDbModel(
                Guid.NewGuid(), "João Silva", "72814249061", null, "joao@email.com",
                "11", "55", "988887777", "Avenida Paulista", "1000", "SN", "Bela Vista", "01310100", "São Paulo", "SP",
                admin.Id, DateTime.UtcNow, null, null
            );
            context.Clientes.Add(cliente);

            var veiculo = new VeiculoDbModel(
                Guid.NewGuid(), "Corolla", "Toyota", cliente.Id, 2023, "BRA2E19", "Preto",
                admin.Id, DateTime.UtcNow, null, null, true
            );
            context.Veiculos.Add(veiculo);
            await context.SaveChangesAsync();
        }

        var result = await _client.GetAsync($"{ApiKey}/");

        Assert.Equal(HttpStatusCode.PartialContent, result.StatusCode);
    }

    [Fact]
    public async Task Veiculo_Get_GetById_NotFound()
    {
        var result = await _client.GetAsync($"{ApiKey}/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task Veiculo_Post_Create_Unauthorized()
    {
        using var anonymousClient = _factory.CreateClient();
        var request = new VeiculoRequestDTO { Modelo = "Teste" };

        var result = await anonymousClient.PostAsJsonAsync(ApiKey, request);

        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task Veiculo_Put_Update_NotFound()
    {
        var request = new VeiculoRequestDTO
        {
            Modelo = "Civic",
            MarcaVeiculo = "Honda",
            ClienteId = Guid.NewGuid(),
            Ano = 2022,
            Placa = "ABC1234",
            Cor = "Azul"
        };

        var result = await _client.PutAsJsonAsync($"{ApiKey}/{Guid.NewGuid()}", request);

        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task Veiculo_Post_Create_MissingData_BadRequest()
    {
        var request = new { Modelo = "Civic" };

        var result = await _client.PostAsJsonAsync(ApiKey, request);

        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task Veiculo_Put_Update_NoContent()
    {
        Guid veiculoId;
        Guid clienteId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var admin = context.Usuarios.First();

            var cliente = new ClienteDbModel(
                Guid.NewGuid(), "Maria Souza", "12345678909", null, "maria@email.com",
                "11", "55", "911112222", "Rua A", "1", "SN", "Bairro", "01000000", "SP", "SP",
                admin.Id, DateTime.UtcNow, null, null
            );
            context.Clientes.Add(cliente);

            var veiculo = new VeiculoDbModel(
                Guid.NewGuid(), "Fit", "Honda", cliente.Id, 2020, "XYZ1A23", "Azul",
                admin.Id, DateTime.UtcNow, null, null, true
            );
            context.Veiculos.Add(veiculo);
            await context.SaveChangesAsync();
            veiculoId = veiculo.Id;
            clienteId = cliente.Id;
        }

        var request = new VeiculoRequestDTO
        {
            Modelo = "Fit Atualizado",
            MarcaVeiculo = "Honda",
            ClienteId = clienteId,
            Ano = 2021,
            Placa = "XYZ1A23",
            Cor = "Branco"
        };

        var result = await _client.PutAsJsonAsync($"{ApiKey}/{veiculoId}", request);

        Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
    }

    [Fact]
    public async Task Veiculo_Delete_NoContent()
    {
        Guid veiculoId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var admin = context.Usuarios.First();

            var cliente = new ClienteDbModel(
                Guid.NewGuid(), "Carlos Delete", "52998224725", null, "carlos@email.com",
                "11", "55", "999999999", "Rua B", "2", null, "Centro", "01310100", "São Paulo", "SP",
                admin.Id, DateTime.UtcNow, null, null
            );
            context.Clientes.Add(cliente);

            var veiculo = new VeiculoDbModel(
                Guid.NewGuid(), "Civic", "Honda", cliente.Id, 2022, "DEL1A23", "Preto",
                admin.Id, DateTime.UtcNow, null, null, true
            );
            context.Veiculos.Add(veiculo);
            await context.SaveChangesAsync();
            veiculoId = veiculo.Id;
        }

        var result = await _client.DeleteAsync($"{ApiKey}/{veiculoId}");

        Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
    }

    [Fact]
    public async Task Veiculo_Get_GetPaginated_PaginaZero_BadRequest()
    {
        var result = await _client.GetAsync($"{ApiKey}?page=0");

        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task Veiculo_Post_Create_ClienteInexistente_BadRequest()
    {
        var request = new VeiculoRequestDTO
        {
            Modelo = "Uno",
            MarcaVeiculo = "Fiat",
            ClienteId = Guid.NewGuid(),
            Ano = 2010,
            Placa = "UNO1A23",
            Cor = "Escada"
        };

        var result = await _client.PostAsJsonAsync(ApiKey, request);

        Assert.True(result.StatusCode == HttpStatusCode.BadRequest || result.StatusCode == HttpStatusCode.NotFound);
    }
}
