using Application.Veiculos.DTOs.Requests;
using Domain.Aggregates.ClienteAggregates;
using Domain.ValueObjects;
using Infra.Context;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace API.test.Veiculos;
public class VeiculoTest : IClassFixture<IntegrationTestFixture>, IAsyncLifetime
{
    const string ApiKey = "api/Veiculo";

    private readonly HttpClient _client;
    private readonly ApiWebApplicationFactory _factory;
    private readonly IntegrationTestFixture _fixture;
    public async Task InitializeAsync() => await _factory.ResetDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    public VeiculoTest(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Client;
        _factory = fixture.App;
    }

    [Fact]
    public async Task Veiculo_Post_Create_Created()
    {
        // Arrange
        Guid clienteId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var telefone = new Telefone("11","55", "988887777");
            var endereco = new Endereco("Avenida Paulista", "1000", "SN", "Bela Vista", "São Paulo", "SP", "01310-100");

            var cliente = new Cliente("João Silva", new Cpf("72814249061"), Guid.Empty, endereco, telefone, new Email("joao@email.com"));
            context.Clientes.Add(cliente);
            await context.SaveChangesAsync();
            clienteId = cliente.Id;
        }

        var request = new VeiculoRequestDTO
        {
            Modelo = "Civic",
            MarcaVeiculo = "Honda",
            ClienteId = clienteId,
            Ano = 2022,
            Placa = "ABC1D23",
            Cor = "Prata"
        };

        // Act
        var result = await _client.PostAsJsonAsync(ApiKey, request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, result.StatusCode);
    }

    [Fact]
    public async Task Veiculo_Post_Create_BadRequest()
    {
        // Arrange
        Guid clienteId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var telefone = new Telefone("11","55", "988887777");
            var endereco = new Endereco("Avenida Paulista", "1000", "SN", "Bela Vista", "São Paulo", "SP", "01310-100");

            var cliente = new Cliente("João Silva", new Cpf("72814249061"), Guid.Empty, endereco, telefone, new Email("joao@email.com"));
            context.Clientes.Add(cliente);
            await context.SaveChangesAsync();
            clienteId = cliente.Id;
        }

        var request = new VeiculoRequestDTO
        {
            Modelo = "Civic",
            MarcaVeiculo = "Honda",
            ClienteId = clienteId,
            Ano = 1800,
            Placa = "ABC1D23",
            Cor = "Prata"
        };

        // Act
        var result = await _client.PostAsJsonAsync(ApiKey, request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task Veiculo_Get_GetById_OK()
    {
        // Arrange
        Guid veiculoId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var telefone = new Telefone("11", "55", "988887777");
            var endereco = new Endereco("Avenida Paulista", "1000", "SN", "Bela Vista", "São Paulo", "SP", "01310-100");

            var cliente = new Cliente("João Silva", new Cpf("72814249061"), Guid.Empty, endereco, telefone, new Email("joao@email.com"));
            context.Clientes.Add(cliente);

            var veiculo = new Domain.Entities.Veiculo("Corolla", "Toyota", cliente.Id, 2023, new Placa("BRA2E19"), "Preto", Guid.Empty);
            context.Veiculos.Add(veiculo);
            await context.SaveChangesAsync();
            veiculoId = veiculo.Id;
        }

        // Act
        var result = await _client.GetAsync($"{ApiKey}/{veiculoId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
    }
    [Fact]
    public async Task Veiculo_Get_GetPaginated_PartialContent()
    {
        // Arrange
        Guid veiculoId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var telefone = new Telefone("11", "55", "988887777");
            var endereco = new Endereco("Avenida Paulista", "1000", "SN", "Bela Vista", "São Paulo", "SP", "01310-100");

            var cliente = new Cliente("João Silva", new Cpf("72814249061"), Guid.Empty, endereco, telefone, new Email("joao@email.com"));
            context.Clientes.Add(cliente);

            var veiculo = new Domain.Entities.Veiculo("Corolla", "Toyota", cliente.Id, 2023, new Placa("BRA2E19"), "Preto", Guid.Empty);
            context.Veiculos.Add(veiculo);
            await context.SaveChangesAsync();
            veiculoId = veiculo.Id;
        }

        // Act
        var result = await _client.GetAsync($"{ApiKey}/");

        // Assert
        Assert.Equal(HttpStatusCode.PartialContent, result.StatusCode);
    }

    [Fact]
    public async Task Veiculo_Get_GetById_NotFound()
    {
        // Act:
        var result = await _client.GetAsync($"{ApiKey}/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task Veiculo_Post_Create_Unauthorized()
    {
        // Arrange:
        using var anonymousClient = _factory.CreateClient();
        var request = new VeiculoRequestDTO { Modelo = "Teste" };

        // Act
        var result = await anonymousClient.PostAsJsonAsync(ApiKey, request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task Veiculo_Put_Update_NotFound()
    {
        // Arrange
        var request = new VeiculoRequestDTO
        {
            Modelo = "Civic",
            MarcaVeiculo = "Honda",
            ClienteId = Guid.NewGuid(),
            Ano = 2022,
            Placa = "ABC1234",
            Cor = "Azul"
        };

        // Act
        var result = await _client.PutAsJsonAsync($"{ApiKey}/{Guid.NewGuid()}", request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task Veiculo_Post_Create_MissingData_BadRequest()
    {
        // Arrange
        var request = new { Modelo = "Civic" };

        // Act
        var result = await _client.PostAsJsonAsync(ApiKey, request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task Veiculo_Put_Update_NoContent()
    {
        // Arrange
        Guid veiculoId;
        Guid clienteId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var cliente = new Cliente("Maria Souza", new Cpf("12345678909"), Guid.Empty,
                new Endereco("Rua A", "1", "SN", "Bairro", "SP", "SP", "01000-000"),
                new Telefone("11", "55", "911112222"), new Email("maria@email.com"));
            context.Clientes.Add(cliente);

            var veiculo = new Domain.Entities.Veiculo("Fit", "Honda", cliente.Id, 2020, new Placa("XYZ1A23"), "Azul", Guid.Empty);
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

        // Act
        var result = await _client.PutAsJsonAsync($"{ApiKey}/{veiculoId}", request);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
    }

    [Fact]
    public async Task Veiculo_Delete_NoContent()
    {
        // Arrange
        Guid veiculoId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var cliente = context.Clientes.First(); // Reaproveita cliente existente ou cria um novo
            var veiculo = new Domain.Entities.Veiculo("Civic", "Honda", cliente.Id, 2022, new Placa("DEL1A23"), "Preto", Guid.Empty);
            context.Veiculos.Add(veiculo);
            await context.SaveChangesAsync();
            veiculoId = veiculo.Id;
        }

        // Act
        var result = await _client.DeleteAsync($"{ApiKey}/{veiculoId}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
    }

    [Fact]
    public async Task Veiculo_Get_GetPaginated_PaginaZero_BadRequest()
    {
        // Act 
        var result = await _client.GetAsync($"{ApiKey}?page=0");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task Veiculo_Post_Create_ClienteInexistente_BadRequest()
    {
        // Arrange
        var request = new VeiculoRequestDTO
        {
            Modelo = "Uno",
            MarcaVeiculo = "Fiat",
            ClienteId = Guid.NewGuid(), 
            Ano = 2010,
            Placa = "UNO1A23",
            Cor = "Escada"
        };

        // Act
        var result = await _client.PostAsJsonAsync(ApiKey, request);

        // Assert
        Assert.True(result.StatusCode == HttpStatusCode.BadRequest || result.StatusCode == HttpStatusCode.NotFound);
    }
}
