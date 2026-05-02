using System.Net;
using System.Net.Http.Json;
using System.Text;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Xunit;
using FluentAssertions;
using Application.Clientes.DTOs.Requests;
using Application.Clientes.DTOs.Responses;
using Application.Clientes.DTOs.Shared;
using Integration.test.Infrastructure;

namespace Integration.test.Clientes;

[Collection("IntegrationTests")]
public sealed class ClienteIntegrationTests
{
    private readonly HttpClient _client;
    private readonly IntegrationTestBase _fixture;

    private const string BaseRoute = "/api/cliente";

    public ClienteIntegrationTests(IntegrationTestBase fixture)
    {
        _client = fixture.Client;

        _fixture.AutenticarClient(Guid.NewGuid(), "Admin Teste", "Administrador");
    }

    [Fact]
    public async Task Post_ClienteComCpfValido_RetornaCreated()
    {
        // Arrange
        var request = CriarRequestComCpf();

        // Act
        var response = await _client.PostAsJsonAsync(BaseRoute, request);
        var body = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        body.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Post_ClienteComCnpjValido_RetornaCreated()
    {
        // Arrange
        var request = CriarRequestComCnpj();

        // Act
        var response = await _client.PostAsJsonAsync(BaseRoute, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Post_ClienteSemCpfECnpj_RetornaBadRequest()
    {
        // Arrange
        var request = new ClienteRequestDTO
        {
            Nome = "Sem Documento",
            Cpf = null,
            Cnpj = null,
            Emails = new List<string> { "sem@documento.com" },
            Telefones = new List<TelefoneDTO>(),
            Enderecos = new List<EnderecoDTO>()
        };

        // Act
        var response = await _client.PostAsJsonAsync(BaseRoute, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_ClienteComCpfECnpjSimultaneos_RetornaBadRequest()
    {
        // Arrange
        var request = CriarRequestComCpf();
        request.Cnpj = "11222333000181"; // inválido ter os dois

        // Act
        var response = await _client.PostAsJsonAsync(BaseRoute, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_ClienteComTipoTelefoneInvalido_RetornaBadRequest()
    {
        // Arrange
        var request = CriarRequestComCpf();
        request.Telefones = new List<TelefoneDTO>
        {
            new TelefoneDTO { DDD = "11", DDI = "55", Numero = "99999-9999", Tipo = "TipoQueNaoExiste" }
        };

        // Act
        var response = await _client.PostAsJsonAsync(BaseRoute, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Get_ClienteCriadoAnteriormente_RetornaOkComDados()
    {
        // Arrange — cria o cliente primeiro
        var requestCriacao = CriarRequestComCpf("44455566600");
        var respostaCriacao = await _client.PostAsJsonAsync(BaseRoute, requestCriacao);
        respostaCriacao.StatusCode.Should().Be(HttpStatusCode.Created);

        // Extrai o ID do cliente recém-criado do corpo da resposta
        var idCliente = await ExtrairIdDaRespostaAsync(respostaCriacao);

        // Act
        var response = await _client.GetAsync($"{BaseRoute}/{idCliente}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var clienteRetornado = await response.Content.ReadFromJsonAsync<ClienteResponseDTO>();
        clienteRetornado.Should().NotBeNull();
        clienteRetornado!.Nome.Should().Be(requestCriacao.Nome);
        clienteRetornado.Cpf.Should().Be(requestCriacao.Cpf);
    }

    [Fact]
    public async Task Get_ClienteInexistente_RetornaNotFound()
    {
        // Act
        var response = await _client.GetAsync($"{BaseRoute}/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Put_AlterarNomeDeClienteExistente_RetornaOk()
    {
        // Arrange — cria o cliente
        var requestCriacao = CriarRequestComCpf("55566677700");
        var respostaCriacao = await _client.PostAsJsonAsync(BaseRoute, requestCriacao);
        var idCliente = await ExtrairIdDaRespostaAsync(respostaCriacao);

        // Monta o request de atualização com nome diferente
        var requestAtualizacao = CriarRequestComCpf("55566677700");
        requestAtualizacao.Nome = "Nome Atualizado via Integração";

        // Act
        var response = await _client.PutAsJsonAsync($"{BaseRoute}/{idCliente}", requestAtualizacao);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Put_ClienteInexistente_RetornaNotFound()
    {
        // Arrange
        var request = CriarRequestComCpf();

        // Act
        var response = await _client.PutAsJsonAsync($"{BaseRoute}/{Guid.NewGuid()}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_ClienteExistente_RetornaNoContent()
    {
        // Arrange — cria o cliente
        var requestCriacao = CriarRequestComCpf("77788899900");
        var respostaCriacao = await _client.PostAsJsonAsync(BaseRoute, requestCriacao);
        var idCliente = await ExtrairIdDaRespostaAsync(respostaCriacao);

        // Act
        var response = await _client.DeleteAsync($"{BaseRoute}/{idCliente}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_ClienteInexistente_RetornaNotFound()
    {
        // Act
        var response = await _client.DeleteAsync($"{BaseRoute}/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private static ClienteRequestDTO CriarRequestComCpf(string cpf = "11144477735") =>
        new ClienteRequestDTO
        {
            Nome = "João da Integração",
            Cpf = cpf,
            Cnpj = null,
            Emails = new List<string> { "joao.integracao@example.com" },
            Telefones = new List<TelefoneDTO>
            {
                new TelefoneDTO
                {
                    DDD    = "11",
                    DDI    = "55",
                    Numero = "98765-4321",
                    Tipo   = "Celular"
                }
            },
            Enderecos = new List<EnderecoDTO>
            {
                new EnderecoDTO
                {
                    Logradouro  = "Rua da Integração",
                    Numero      = "100",
                    Complemento = "Sala 1",
                    Bairro      = "Centro",
                    Cidade      = "São Paulo",
                    Uf          = "SP",
                    Cep         = "01234-567"
                }
            }
        };

    private static ClienteRequestDTO CriarRequestComCnpj() =>
        new ClienteRequestDTO
        {
            Nome = "Empresa Integração LTDA",
            Cnpj = "11222333000181",
            Cpf = null,
            Emails = new List<string> { "empresa.integracao@example.com" },
            Telefones = new List<TelefoneDTO>
            {
                new TelefoneDTO
                {
                    DDD    = "11",
                    DDI    = "55",
                    Numero = "3333-4444",
                    Tipo   = "Comercial"
                }
            },
            Enderecos = new List<EnderecoDTO>
            {
                new EnderecoDTO
                {
                    Logradouro  = "Av. Empresarial",
                    Numero      = "500",
                    Complemento = string.Empty,
                    Bairro      = "Jardins",
                    Cidade      = "São Paulo",
                    Uf          = "SP",
                    Cep         = "04567-890"
                }
            }
        };

    private static async Task<Guid> ExtrairIdDaRespostaAsync(HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync();

        // Tenta como Guid puro
        if (Guid.TryParse(body.Trim('"'), out var guidPuro))
            return guidPuro;

        // Tenta como objeto JSON com campo "id" ou "data"
        try
        {
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;

            foreach (var campo in new[] { "id", "Id", "data", "Data" })
            {
                if (root.TryGetProperty(campo, out var prop) &&
                    Guid.TryParse(prop.GetString(), out var guidCampo))
                    return guidCampo;
            }
        }
        catch (JsonException)
        {
            // ignora — o formato não era JSON
        }

        throw new InvalidOperationException(
            $"Não foi possível extrair o ID da resposta de criação. Corpo recebido: {body}");
    }

    [Fact]
    public async Task Get_ListagemPaginada_RetornaOkComDados()
    {
        await _client.PostAsJsonAsync(BaseRoute, CriarRequestComCpf("12312312301"));
        await _client.PostAsJsonAsync(BaseRoute, CriarRequestComCpf("12312312302"));

        // Act
        var response = await _client.GetAsync($"{BaseRoute}?page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadAsStringAsync();
        body.Should().NotBeNullOrEmpty();
    }
}