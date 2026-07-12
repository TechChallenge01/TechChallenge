using Domain.Entities;
using Domain.Enums;

namespace Domain.test.Tests
{
    public class UsuarioTests
    {
        [Fact]
        public void CriarUsuario_ComDadosValidos_DeveCriarComSucesso()
        {
            var usuario = new Usuario("João Silva", "joao@email.com", "senha123", EPerfilUsuario.Funcionario, Guid.NewGuid());

            Assert.NotNull(usuario);
            Assert.Equal("João Silva", usuario.Nome);
            Assert.Equal("joao@email.com", usuario.Email);
            Assert.Equal(EPerfilUsuario.Funcionario.ToString(), usuario.Perfil);
            Assert.NotEqual(Guid.Empty, usuario.Id);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void CriarUsuario_ComNomeVazio_DeveThrowArgumentException(string nomeInvalido)
        {
            Assert.Throws<ArgumentException>(() =>
                new Usuario(nomeInvalido, "joao@email.com", "senha123", EPerfilUsuario.Funcionario, Guid.NewGuid()));
        }

        [Theory]
        [InlineData("email_invalido")]
        [InlineData("sem_arroba")]
        [InlineData("")]
        public void CriarUsuario_ComEmailInvalido_DeveThrowArgumentException(string emailInvalido)
        {
            Assert.Throws<ArgumentException>(() =>
                new Usuario("João", emailInvalido, "senha123", EPerfilUsuario.Funcionario, Guid.NewGuid()));
        }

        [Theory]
        [InlineData("")]
        [InlineData("123")]
        [InlineData("ab")]
        public void CriarUsuario_ComSenhaMenorQue6Caracteres_DeveThrowArgumentException(string senhaInvalida)
        {
            Assert.Throws<ArgumentException>(() =>
                new Usuario("João", "joao@email.com", senhaInvalida, EPerfilUsuario.Funcionario, Guid.NewGuid()));
        }

        [Fact]
        public void CriarUsuario_ViaConstrutorReconstituicao_DeveTerPropriedadesCorretas()
        {
            var id = Guid.NewGuid();
            var usuario = new Usuario(id, "Maria", "maria@email.com", "hashedpassword", "Administrador");

            Assert.Equal(id, usuario.Id);
            Assert.Equal("Maria", usuario.Nome);
            Assert.Equal("maria@email.com", usuario.Email);
            Assert.Equal("hashedpassword", usuario.SenhaHash);
            Assert.Equal("Administrador", usuario.Perfil);
        }
    }
}
