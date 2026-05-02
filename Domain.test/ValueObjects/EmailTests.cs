using Domain.ValueObjects;

namespace Domain.test.ValueObjects;

public class EmailTests
{
    [Fact]
    public void Constructor_ValidEmail_CreatesEmailSuccessfully()
    {
        // Arrange
        string validEmail = "test@example.com";

        // Act
        var email = new Email(validEmail);

        // Assert
        Assert.NotNull(email);
        Assert.Equal("test@example.com", email.EnderecoEmail);
    }

    [Fact]
    public void Constructor_ValidEmailUpperCase_StoresLowerCase()
    {
        // Arrange
        string emailUpperCase = "TEST@EXAMPLE.COM";

        // Act
        var email = new Email(emailUpperCase);

        // Assert
        Assert.Equal("test@example.com", email.EnderecoEmail);
    }

    [Fact]
    public void Constructor_EmptyEmail_ThrowsArgumentException()
    {
        // Arrange
        string emptyEmail = "";

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => new Email(emptyEmail));
        Assert.Contains("não pode ser nulo ou vazio", ex.Message);
    }

    [Fact]
    public void Constructor_NullEmail_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Email(null));
    }

    [Fact]
    public void Constructor_InvalidEmailFormat_ThrowsArgumentException()
    {
        // Arrange
        string invalidEmail = "notanemail";

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => new Email(invalidEmail));
        Assert.Contains("inválido", ex.Message);
    }

    [Fact]
    public void Constructor_InvalidEmailNoAt_ThrowsArgumentException()
    {
        // Arrange
        string invalidEmail = "testexample.com";

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => new Email(invalidEmail));
        Assert.Contains("inválido", ex.Message);
    }

    [Fact]
    public void Equals_SameEmail_ReturnsTrue()
    {
        // Arrange
        var email1 = new Email("test@example.com");
        var email2 = new Email("test@example.com");

        // Act & Assert
        Assert.Equal(email1, email2);
    }

    [Fact]
    public void Equals_DifferentEmails_ReturnsFalse()
    {
        // Arrange
        var email1 = new Email("test@example.com");
        var email2 = new Email("other@example.com");

        // Act & Assert
        Assert.NotEqual(email1, email2);
    }

    [Fact]
    public void GetHashCode_SameEmail_ReturnsSameHashCode()
    {
        // Arrange
        var email1 = new Email("test@example.com");
        var email2 = new Email("test@example.com");

        // Act & Assert
        Assert.Equal(email1.GetHashCode(), email2.GetHashCode());
    }
}
