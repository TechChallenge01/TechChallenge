using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.test.ValueObjects;

public class TelefoneTests
{
    [Fact]
    public void Constructor_ValidTelefone_CreatesTelefoneSuccessfully()
    {
        // Arrange
        string ddd = "11";
        string ddi = "55";
        string numero = "98765-4321";
        ETipoTelefone tipo = ETipoTelefone.Celular;

        // Act
        var telefone = new Telefone(ddd, ddi, numero, tipo);

        // Assert
        Assert.NotNull(telefone);
        Assert.Equal("11", telefone.DDD);
        Assert.Equal("55", telefone.DDI);
        Assert.Equal("98765-4321", telefone.Numero);
        Assert.Equal("Celular", telefone.Tipo);
    }

    [Fact]
    public void Constructor_TelefoneWithWhitespace_TrimsValues()
    {
        // Arrange
        string ddd = "  11  ";
        string ddi = "  55  ";
        string numero = "  98765-4321  ";

        // Act
        var telefone = new Telefone(ddd, ddi, numero, ETipoTelefone.Residencial);

        // Assert
        Assert.Equal("11", telefone.DDD);
        Assert.Equal("55", telefone.DDI);
        Assert.Equal("98765-4321", telefone.Numero);
    }

    [Fact]
    public void Constructor_EmptyDdd_ThrowsArgumentException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => 
            new Telefone("", "55", "98765-4321", ETipoTelefone.Celular));
        Assert.Contains("DDD não pode ser nulo ou vazio", ex.Message);
    }

    [Fact]
    public void Constructor_EmptyDdi_ThrowsArgumentException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => 
            new Telefone("11", "", "98765-4321", ETipoTelefone.Celular));
        Assert.Contains("DDI não pode ser nulo ou vazio", ex.Message);
    }

    [Fact]
    public void Constructor_EmptyNumero_ThrowsArgumentException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => 
            new Telefone("11", "55", "", ETipoTelefone.Celular));
        Assert.Contains("número não pode ser nulo ou vazio", ex.Message);
    }

    [Fact]
    public void Equals_SameTelefone_ReturnsTrue()
    {
        // Arrange
        var telefone1 = new Telefone("11", "55", "98765-4321", ETipoTelefone.Celular);
        var telefone2 = new Telefone("11", "55", "98765-4321", ETipoTelefone.Residencial);

        // Act & Assert
        Assert.Equal(telefone1, telefone2); // Equals compares only DDI, DDD, Numero
    }

    [Fact]
    public void Equals_DifferentTelefone_ReturnsFalse()
    {
        // Arrange
        var telefone1 = new Telefone("11", "55", "98765-4321", ETipoTelefone.Celular);
        var telefone2 = new Telefone("21", "55", "98765-4321", ETipoTelefone.Celular);

        // Act & Assert
        Assert.NotEqual(telefone1, telefone2);
    }

    [Fact]
    public void GetHashCode_SameTelefone_ReturnsSameHashCode()
    {
        // Arrange
        var telefone1 = new Telefone("11", "55", "98765-4321", ETipoTelefone.Celular);
        var telefone2 = new Telefone("11", "55", "98765-4321", ETipoTelefone.Residencial);

        // Act & Assert
        Assert.Equal(telefone1.GetHashCode(), telefone2.GetHashCode());
    }
}
