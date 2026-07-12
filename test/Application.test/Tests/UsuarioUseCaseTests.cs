using Application.Auth.DTOs.Requests;
using Application.Gateways.Usuarios;
using Application.Interfaces;
using Application.UseCases.Usuarios;
using Domain.Enums;
using Moq;
using Shared.DTOs.Usuarios.Input;

namespace Application.test.Tests;

public class UsuarioUseCaseTests
{
    private static UsuarioInputDTO CriarUsuarioInputDTO(Guid? id = null, bool ativo = true, string? senhaHash = null) => new UsuarioInputDTO
    {
        Id = id ?? Guid.NewGuid(),
        Nome = "João Silva",
        Email = "joao@email.com",
        SenhaHash = senhaHash ?? BCrypt.Net.BCrypt.HashPassword("senha123"),
        Perfil = EPerfilUsuario.Funcionario.ToString(),
        IdUsuarioCriacao = Guid.NewGuid(),
        DataCriacao = DateTime.UtcNow,
        Ativo = ativo
    };

    [Fact]
    public async Task Login_ComCredenciaisValidas_DeveRetornarToken()
    {
        var senhaHash = BCrypt.Net.BCrypt.HashPassword("senha123");
        var mockDS = new Mock<IUsuarioDataSource>();
        mockDS.Setup(m => m.GetByEmail(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarUsuarioInputDTO(senhaHash: senhaHash));
        var mockJwt = new Mock<IJwtService>();
        mockJwt.Setup(m => m.GerarToken(It.IsAny<Domain.Entities.Usuario>()))
            .Returns(("jwt_token_valido", DateTime.UtcNow.AddHours(1)));

        var gateway = UsuarioGateway.Create(mockDS.Object);
        var useCase = LoginUseCase.Create(gateway, mockJwt.Object);
        var request = new LoginRequestDTO { Email = "joao@email.com", Senha = "senha123" };

        var result = await useCase.Run(request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("jwt_token_valido", result.Token);
    }

    [Fact]
    public async Task Login_ComSenhaErrada_DeveThrowUnauthorizedAccessException()
    {
        var senhaHash = BCrypt.Net.BCrypt.HashPassword("senha_correta");
        var mockDS = new Mock<IUsuarioDataSource>();
        mockDS.Setup(m => m.GetByEmail(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarUsuarioInputDTO(senhaHash: senhaHash));

        var gateway = UsuarioGateway.Create(mockDS.Object);
        var useCase = LoginUseCase.Create(gateway, new Mock<IJwtService>().Object);
        var request = new LoginRequestDTO { Email = "joao@email.com", Senha = "senha_errada" };

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            useCase.Run(request, CancellationToken.None));
    }

    [Fact]
    public async Task Login_ComUsuarioInativo_DeveThrowUnauthorizedAccessException()
    {
        var senhaHash = BCrypt.Net.BCrypt.HashPassword("senha123");
        var mockDS = new Mock<IUsuarioDataSource>();
        mockDS.Setup(m => m.GetByEmail(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarUsuarioInputDTO(ativo: false, senhaHash: senhaHash));

        var gateway = UsuarioGateway.Create(mockDS.Object);
        var useCase = LoginUseCase.Create(gateway, new Mock<IJwtService>().Object);
        var request = new LoginRequestDTO { Email = "joao@email.com", Senha = "senha123" };

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            useCase.Run(request, CancellationToken.None));
    }

    [Fact]
    public async Task CriarUsuario_EmailJaCadastrado_DeveThrowArgumentException()
    {
        var mockDS = new Mock<IUsuarioDataSource>();
        mockDS.Setup(m => m.GetByEmail(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarUsuarioInputDTO());
        mockDS.Setup(m => m.Create(It.IsAny<UsuarioInputDTO>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var gateway = UsuarioGateway.Create(mockDS.Object);
        var useCase = new CriarUsuarioUseCase(gateway);
        var request = new CriarUsuarioRequestDTO
        {
            Nome = "João Silva",
            Email = "joao@email.com",
            Senha = "senha12345",
            Perfil = EPerfilUsuario.Funcionario
        };

        await Assert.ThrowsAsync<ArgumentException>(() =>
            useCase.Run(request, Guid.NewGuid(), CancellationToken.None));
    }
}
