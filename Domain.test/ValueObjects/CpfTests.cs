using Domain.ValueObjects;

namespace Domain.test.ValueObjects;

public class CpfTests
{
    [Fact]
    public void Constructor_ValidCpf_CreatesCpfSuccessfully()
    {
        // Arrange
        string validCpf = "11144477735";

        // Act
        var cpf = new Cpf(validCpf);

        // Assert
        Assert.NotNull(cpf);
        Assert.Equal("11144477735", cpf.Valor);
    }

    [Fact]
    public void Constructor_ValidCpfWithFormatting_RemovesFormatting()
    {
        // Arrange
        string formattedCpf = "111.444.777-35";

        // Act
        var cpf = new Cpf(formattedCpf);

        // Assert
        Assert.Equal("11144477735", cpf.Valor);
    }

    [Fact]
    public void Constructor_EmptyCpf_ThrowsArgumentException()
    {
        // Arrange
        string emptyCpf = "";

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => new Cpf(emptyCpf));
        Assert.Contains("não pode ser vazio", ex.Message);
    }

    [Fact]
    public void Constructor_NullCpf_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Cpf(null));
    }

    [Fact]
    public void Constructor_CpfWithLessThan11Digits_ThrowsArgumentException()
    {
        // Arrange
        string invalidCpf = "1234567890";

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => new Cpf(invalidCpf));
        Assert.Contains("deve conter 11 dígitos", ex.Message);
    }

    [Fact]
    public void Constructor_CpfWithAllSameDigits_ThrowsArgumentException()
    {
        // Arrange
        string invalidCpf = "11111111111";

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => new Cpf(invalidCpf));
        Assert.Contains("inválido", ex.Message);
    }

    [Fact]
    public void Constructor_InvalidCpfChecksum_ThrowsArgumentException()
    {
        // Arrange
        string invalidCpf = "11144477736"; // Last digit is wrong

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => new Cpf(invalidCpf));
        Assert.Contains("inválido", ex.Message);
    }
}
