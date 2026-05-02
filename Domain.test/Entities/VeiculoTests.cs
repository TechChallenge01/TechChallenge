using Domain.Entities;
using Domain.ValueObjects;

namespace Domain.test.Entities;

public class VeiculoTests
{
    private Guid _clienteId = Guid.NewGuid();
    private Guid _usuarioId = Guid.NewGuid();

    [Fact]
    public void Constructor_ValidVeiculo_CreatesVeiculoSuccessfully()
    {
        // Arrange
        string modelo = "Civic";
        string marca = "Honda";
        int ano = 2023;
        var placa = new Placa("ABC1234");
        string cor = "Branco";

        // Act
        var veiculo = new Veiculo(modelo, marca, _clienteId, ano, placa, cor, _usuarioId);

        // Assert
        Assert.NotNull(veiculo);
        Assert.Equal("Civic", veiculo.Modelo);
        Assert.Equal("Honda", veiculo.MarcaVeiculo);
        Assert.Equal(2023, veiculo.Ano);
        Assert.Equal("ABC1234", veiculo.Placa);
        Assert.Equal("Branco", veiculo.Cor);
        Assert.Equal(_clienteId, veiculo.ClienteId);
    }

    [Fact]
    public void Constructor_EmptyModelo_ThrowsArgumentException()
    {
        // Arrange
        var placa = new Placa("ABC1234");

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => 
            new Veiculo("", "Honda", _clienteId, 2023, placa, "Branco", _usuarioId));
        Assert.Contains("obrigatório", ex.Message);
    }

    [Fact]
    public void Constructor_EmptyMarca_ThrowsArgumentException()
    {
        // Arrange
        var placa = new Placa("ABC1234");

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => 
            new Veiculo("Civic", "", _clienteId, 2023, placa, "Branco", _usuarioId));
        Assert.Contains("obrigatória", ex.Message);
    }

    [Fact]
    public void Constructor_InvalidAno_ThrowsArgumentException()
    {
        // Arrange
        var placa = new Placa("ABC1234");
        int invalidAno = 1800;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new Veiculo("Civic", "Honda", _clienteId, invalidAno, placa, "Branco", _usuarioId));
    }

    [Fact]
    public void Constructor_EmptyCor_ThrowsArgumentException()
    {
        // Arrange
        var placa = new Placa("ABC1234");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new Veiculo("Civic", "Honda", _clienteId, 2023, placa, "", _usuarioId));
    }

    [Fact]
    public void Constructor_NullPlaca_ThrowsException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new Veiculo("Civic", "Honda", _clienteId, 2023, null, "Branco", _usuarioId));
    }

    [Fact]
    public void Constructor_VeiculoTrimsModelo()
    {
        // Arrange
        var placa = new Placa("ABC1234");

        // Act
        var veiculo = new Veiculo("  Civic  ", "Honda", _clienteId, 2023, placa, "Branco", _usuarioId);

        // Assert
        Assert.Equal("Civic", veiculo.Modelo);
    }

    [Fact]
    public void Constructor_VeiculoSetsAtivoTrue()
    {
        // Arrange
        var placa = new Placa("ABC1234");

        // Act
        var veiculo = new Veiculo("Civic", "Honda", _clienteId, 2023, placa, "Branco", _usuarioId);

        // Assert
        Assert.True(veiculo.Ativo);
    }

    [Fact]
    public void Constructor_VeiculoSetsCreationDate()
    {
        // Arrange
        var placa = new Placa("ABC1234");
        var beforeCreation = DateTime.UtcNow;

        // Act
        var veiculo = new Veiculo("Civic", "Honda", _clienteId, 2023, placa, "Branco", _usuarioId);
        var afterCreation = DateTime.UtcNow;

        // Assert
        Assert.True(beforeCreation <= veiculo.DataCriacao && veiculo.DataCriacao <= afterCreation);
    }
}
