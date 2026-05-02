using Domain.BaseEntity;

namespace Domain.test.BaseEntity;

public class BaseEntityTests
{
    private Guid _usuarioId = Guid.NewGuid();
    private DateTime _dataCriacao = DateTime.UtcNow;

    [Fact]
    public void Constructor_WithoutParameters_SetsDefaultValues()
    {
        // Arrange & Act
        var baseEntity = new ConcreteBase();

        // Assert
        Assert.True(baseEntity.Ativo);
        Assert.Equal(Guid.Empty, baseEntity.IdUsuarioCriacao);
    }

    [Fact]
    public void Constructor_WithParameters_SetsCriacaoValues()
    {
        // Arrange & Act
        var baseEntity = new ConcreteBase(_usuarioId, _dataCriacao, null, null);

        // Assert
        Assert.Equal(_usuarioId, baseEntity.IdUsuarioCriacao);
        Assert.Equal(_dataCriacao, baseEntity.DataCriacao);
        Assert.Null(baseEntity.IdUsuarioAtualizacao);
        Assert.Null(baseEntity.DataAtualizacao);
        Assert.True(baseEntity.Ativo);
    }

    [Fact]
    public void Inativar_ChangesAtivoToFalse()
    {
        // Arrange
        var baseEntity = new ConcreteBase(_usuarioId, _dataCriacao, null, null);
        Assert.True(baseEntity.Ativo);

        // Act
        baseEntity.Inativar();

        // Assert
        Assert.False(baseEntity.Ativo);
    }

    [Fact]
    public void RastrearAlteracao_UpdatesAtualizacaoValues()
    {
        // Arrange
        var baseEntity = new ConcreteBase(_usuarioId, _dataCriacao, null, null);
        var novoUsuarioId = Guid.NewGuid();
        var dataAtualizacao = DateTime.UtcNow;

        // Act
        baseEntity.RastrearAlteracao(novoUsuarioId, dataAtualizacao);

        // Assert
        Assert.Equal(novoUsuarioId, baseEntity.IdUsuarioAtualizacao);
        Assert.Equal(dataAtualizacao, baseEntity.DataAtualizacao);
    }

    [Fact]
    public void RastrearAlteracao_MultipleUpdates_UpdatesLatestValues()
    {
        // Arrange
        var baseEntity = new ConcreteBase(_usuarioId, _dataCriacao, null, null);
        var usuarioId1 = Guid.NewGuid();
        var data1 = DateTime.UtcNow.AddHours(-1);
        var usuarioId2 = Guid.NewGuid();
        var data2 = DateTime.UtcNow;

        // Act
        baseEntity.RastrearAlteracao(usuarioId1, data1);
        baseEntity.RastrearAlteracao(usuarioId2, data2);

        // Assert
        Assert.Equal(usuarioId2, baseEntity.IdUsuarioAtualizacao);
        Assert.Equal(data2, baseEntity.DataAtualizacao);
    }

    /// <summary>
    /// Concrete implementation of Base for testing purposes
    /// </summary>
    private class ConcreteBase : Base
    {
        public ConcreteBase() : base()
        {
        }

        public ConcreteBase(Guid idUsuarioCriacao, DateTime dataCriacao, Guid? idUsuarioAtualizacao, DateTime? dataAtualizacao)
            : base(idUsuarioCriacao, dataCriacao, idUsuarioAtualizacao, dataAtualizacao)
        {
        }
    }
}
