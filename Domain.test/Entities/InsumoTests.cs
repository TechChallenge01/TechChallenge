using Domain.Entities;

namespace Domain.test.Entities;

public class InsumoTests
{
    private Guid _usuarioId = Guid.NewGuid();
    private DateTime _dataCriacao = DateTime.UtcNow;

    [Fact]
    public void Constructor_ValidInsumo_CreatesInsumoSuccessfully()
    {
        // Arrange
        string nome = "Óleo Motor";
        string descricao = "Óleo Sintético 5W30";
        decimal custoUnitario = 150.00m;

        // Act
        var insumo = new Insumo(nome, descricao, custoUnitario, _usuarioId, _dataCriacao);

        // Assert
        Assert.NotNull(insumo);
        Assert.Equal("Óleo Motor", insumo.Nome);
        Assert.Equal("Óleo Sintético 5W30", insumo.Descricao);
        Assert.Equal(150.00m, insumo.CustoUnitario);
    }

    [Fact]
    public void Constructor_EmptyNome_ThrowsArgumentException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => 
            new Insumo("", "Descrição", 150.00m, _usuarioId, _dataCriacao));
        Assert.Contains("obrigatório", ex.Message);
    }

    [Fact]
    public void Constructor_EmptyDescricao_ThrowsArgumentException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => 
            new Insumo("Óleo Motor", "", 150.00m, _usuarioId, _dataCriacao));
        Assert.Contains("obrigatória", ex.Message);
    }

    [Fact]
    public void Constructor_NegativeCustoUnitario_ThrowsArgumentException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => 
            new Insumo("Óleo Motor", "Óleo Sintético", -10.00m, _usuarioId, _dataCriacao));
        Assert.Contains("negativo", ex.Message);
    }

    [Fact]
    public void Constructor_ZeroCustoUnitario_CreatesInsumo()
    {
        // Act
        var insumo = new Insumo("Insumo Grátis", "Descrição", 0, _usuarioId, _dataCriacao);

        // Assert
        Assert.Equal(0, insumo.CustoUnitario);
    }

    [Fact]
    public void Constructor_InsumoSetsAtivoTrue()
    {
        // Act
        var insumo = new Insumo("Óleo Motor", "Óleo Sintético", 150.00m, _usuarioId, _dataCriacao);

        // Assert
        Assert.True(insumo.Ativo);
    }

    [Fact]
    public void AtualizarNome_ValidNome_ChangesName()
    {
        // Arrange
        var insumo = new Insumo("Óleo Motor", "Óleo Sintético", 150.00m, _usuarioId, _dataCriacao);

        // Act
        insumo.AtualizarNome("Óleo Premium");

        // Assert
        Assert.Equal("Óleo Premium", insumo.Nome);
    }

    [Fact]
    public void AtualizarDescricao_ValidDescricao_ChangesDescription()
    {
        // Arrange
        var insumo = new Insumo("Óleo Motor", "Óleo Sintético", 150.00m, _usuarioId, _dataCriacao);

        // Act
        insumo.AtualizarDescricao("Óleo Sintético Premium");

        // Assert
        Assert.Equal("Óleo Sintético Premium", insumo.Descricao);
    }
}
