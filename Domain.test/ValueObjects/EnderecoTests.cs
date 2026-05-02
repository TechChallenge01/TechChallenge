using Domain.ValueObjects;

namespace Domain.test.ValueObjects;

public class EnderecoTests
{
    [Fact]
    public void Constructor_ValidEndereco_CreatesEnderecoSuccessfully()
    {
        // Arrange
        string logradouro = "Rua A";
        string numero = "123";
        string complemento = "Apartamento 101";
        string bairro = "Centro";
        string cidade = "São Paulo";
        string uf = "SP";
        string cep = "01234-567";

        // Act
        var endereco = new Endereco(logradouro, numero, complemento, bairro, cidade, uf, cep);

        // Assert
        Assert.NotNull(endereco);
        Assert.Equal("Rua A", endereco.Logradouro);
        Assert.Equal("123", endereco.Numero);
        Assert.Equal("Apartamento 101", endereco.Complemento);
        Assert.Equal("Centro", endereco.Bairro);
        Assert.Equal("São Paulo", endereco.Cidade);
        Assert.Equal("SP", endereco.Uf);
        Assert.Equal("01234-567", endereco.Cep);
    }

    [Fact]
    public void Constructor_NullComplemento_SetsEmptyString()
    {
        // Act
        var endereco = new Endereco("Rua A", "123", null, "Centro", "São Paulo", "SP", "01234-567");

        // Assert
        Assert.Equal(string.Empty, endereco.Complemento);
    }

    [Fact]
    public void Constructor_UfConvertsToUppercase()
    {
        // Act
        var endereco = new Endereco("Rua A", "123", null, "Centro", "São Paulo", "sp", "01234-567");

        // Assert
        Assert.Equal("SP", endereco.Uf);
    }

    [Fact]
    public void Constructor_TrimsWhitespace()
    {
        // Act
        var endereco = new Endereco("  Rua A  ", "  123  ", "  Apto 101  ", "  Centro  ", "  São Paulo  ", "  SP  ", "  01234-567  ");

        // Assert
        Assert.Equal("Rua A", endereco.Logradouro);
        Assert.Equal("123", endereco.Numero);
        Assert.Equal("Apto 101", endereco.Complemento);
        Assert.Equal("Centro", endereco.Bairro);
        Assert.Equal("São Paulo", endereco.Cidade);
        Assert.Equal("SP", endereco.Uf);
        Assert.Equal("01234-567", endereco.Cep);
    }

    [Fact]
    public void Constructor_EmptyLogradouro_ThrowsArgumentException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => 
            new Endereco("", "123", "", "Centro", "São Paulo", "SP", "01234-567"));
        Assert.Contains("Logradouro é obrigatório", ex.Message);
    }

    [Fact]
    public void Constructor_EmptyNumero_ThrowsArgumentException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => 
            new Endereco("Rua A", "", "", "Centro", "São Paulo", "SP", "01234-567"));
        Assert.Contains("Número é obrigatório", ex.Message);
    }

    [Fact]
    public void Constructor_EmptyCep_ThrowsArgumentException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => 
            new Endereco("Rua A", "123", "", "Centro", "São Paulo", "SP", ""));
        Assert.Contains("CEP é obrigatório", ex.Message);
    }

    [Fact]
    public void Constructor_EmptyCidade_ThrowsArgumentException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => 
            new Endereco("Rua A", "123", "", "Centro", "", "SP", "01234-567"));
        Assert.Contains("Cidade é obrigatória", ex.Message);
    }

    [Fact]
    public void Constructor_InvalidUf_ThrowsArgumentException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => 
            new Endereco("Rua A", "123", "", "Centro", "São Paulo", "S", "01234-567"));
        Assert.Contains("UF inválida", ex.Message);
    }

    [Fact]
    public void Constructor_UfWithMoreThanTwoCharacters_ThrowsArgumentException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => 
            new Endereco("Rua A", "123", "", "Centro", "São Paulo", "SPP", "01234-567"));
        Assert.Contains("UF inválida", ex.Message);
    }

    [Fact]
    public void Equals_SameEndereco_ReturnsTrue()
    {
        // Arrange
        var endereco1 = new Endereco("Rua A", "123", "Apto 101", "Centro", "São Paulo", "SP", "01234-567");
        var endereco2 = new Endereco("Rua A", "123", "Apto 101", "Centro", "São Paulo", "SP", "01234-567");

        // Act & Assert
        Assert.Equal(endereco1, endereco2);
    }

    [Fact]
    public void Equals_DifferentEndereco_ReturnsFalse()
    {
        // Arrange
        var endereco1 = new Endereco("Rua A", "123", "", "Centro", "São Paulo", "SP", "01234-567");
        var endereco2 = new Endereco("Rua B", "456", "", "Centro", "São Paulo", "SP", "01234-567");

        // Act & Assert
        Assert.NotEqual(endereco1, endereco2);
    }

    [Fact]
    public void GetHashCode_SameEndereco_ReturnsSameHashCode()
    {
        // Arrange
        var endereco1 = new Endereco("Rua A", "123", "Apto 101", "Centro", "São Paulo", "SP", "01234-567");
        var endereco2 = new Endereco("Rua A", "123", "Apto 101", "Centro", "São Paulo", "SP", "01234-567");

        // Act & Assert
        Assert.Equal(endereco1.GetHashCode(), endereco2.GetHashCode());
    }
}
