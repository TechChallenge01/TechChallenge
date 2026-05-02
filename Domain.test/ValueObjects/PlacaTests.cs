using Domain.ValueObjects;

namespace Domain.test.ValueObjects;

public class PlacaTests
{
    [Fact]
    public void Constructor_ValidOldFormatPlaca_CreatesPlacaSuccessfully()
    {
        // Arrange
        string validPlaca = "ABC1234";

        // Act
        var placa = new Placa(validPlaca);

        // Assert
        Assert.NotNull(placa);
        Assert.Equal("ABC1234", placa.Valor);
    }

    [Fact]
    public void Constructor_ValidMercosulFormatPlaca_CreatesPlacaSuccessfully()
    {
        // Arrange
        string validPlaca = "ABC1D23";

        // Act
        var placa = new Placa(validPlaca);

        // Assert
        Assert.Equal("ABC1D23", placa.Valor);
    }

    [Fact]
    public void Constructor_PlacaWithDash_RemovesDash()
    {
        // Arrange
        string formattedPlaca = "ABC-1234";

        // Act
        var placa = new Placa(formattedPlaca);

        // Assert
        Assert.Equal("ABC1234", placa.Valor);
    }

    [Fact]
    public void Constructor_PlacaWithSpace_RemovesSpace()
    {
        // Arrange
        string formattedPlaca = "ABC 1234";

        // Act
        var placa = new Placa(formattedPlaca);

        // Assert
        Assert.Equal("ABC1234", placa.Valor);
    }

    [Fact]
    public void Constructor_LowercasePlaca_ConvertsToUppercase()
    {
        // Arrange
        string lowercasePlaca = "abc1234";

        // Act
        var placa = new Placa(lowercasePlaca);

        // Assert
        Assert.Equal("ABC1234", placa.Valor);
    }

    [Fact]
    public void Constructor_EmptyPlaca_ThrowsArgumentException()
    {
        // Arrange
        string emptyPlaca = "";

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => new Placa(emptyPlaca));
        Assert.Contains("não pode ser vazia", ex.Message);
    }

    [Fact]
    public void Constructor_InvalidPlacaFormat_ThrowsArgumentException()
    {
        // Arrange
        string invalidPlaca = "ABC12";

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => new Placa(invalidPlaca));
        Assert.Contains("inválida", ex.Message);
    }

    [Fact]
    public void Constructor_InvalidPlacaOnlyNumbers_ThrowsArgumentException()
    {
        // Arrange
        string invalidPlaca = "1234567";

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => new Placa(invalidPlaca));
        Assert.Contains("inválida", ex.Message);
    }
}
