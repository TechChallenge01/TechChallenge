using Domain.ValueObjects;

namespace Domain.test.ValueObjects;

public class CnpjTests
{
    [Fact]
    public void Constructor_ValidCnpj_CreatesCnpjSuccessfully()
    {
        // Arrange
        string validCnpj = "11222333000181";

        // Act
        var cnpj = new Cnpj(validCnpj);

        // Assert
        Assert.NotNull(cnpj);
        Assert.Equal("11222333000181", cnpj.Valor);
    }

    [Fact]
    public void Constructor_ValidCnpjWithFormatting_RemovesFormatting()
    {
        // Arrange
        string formattedCnpj = "11.222.333/0001-81";

        // Act
        var cnpj = new Cnpj(formattedCnpj);

        // Assert
        Assert.Equal("11222333000181", cnpj.Valor);
    }

    [Fact]
    public void Constructor_CnpjWithLessThan14Digits_ThrowsArgumentException()
    {
        // Arrange
        string invalidCnpj = "112223330001";

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => new Cnpj(invalidCnpj));
        Assert.Contains("deve conter 14 dígitos", ex.Message);
    }

    [Fact]
    public void Constructor_EmptyCnpj_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Cnpj(""));
    }

    [Fact]
    public void Constructor_CnpjWithAllSameDigits_ThrowsArgumentException()
    {
        // Arrange
        string invalidCnpj = "11111111111111";

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => new Cnpj(invalidCnpj));
        Assert.Contains("inválido", ex.Message);
    }

    [Fact]
    public void Constructor_InvalidCnpjChecksum_ThrowsArgumentException()
    {
        // Arrange
        string invalidCnpj = "11222333000182"; // Last digit is wrong

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => new Cnpj(invalidCnpj));
        Assert.Contains("inválido", ex.Message);
    }
}
