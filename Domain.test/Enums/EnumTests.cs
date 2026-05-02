using Domain.Enums;

namespace Domain.test.Enums;

public class EnumTests
{
    [Fact]
    public void EStatusOS_HasAllRequiredStatuses()
    {
        // Assert
        Assert.True(Enum.IsDefined(typeof(EStatusOS), EStatusOS.Recebida));
        Assert.True(Enum.IsDefined(typeof(EStatusOS), EStatusOS.EmDiagnostico));
        Assert.True(Enum.IsDefined(typeof(EStatusOS), EStatusOS.AguardandoAprovacao));
        Assert.True(Enum.IsDefined(typeof(EStatusOS), EStatusOS.EmExecucao));
        Assert.True(Enum.IsDefined(typeof(EStatusOS), EStatusOS.Finalizada));
        Assert.True(Enum.IsDefined(typeof(EStatusOS), EStatusOS.Entregue));
        Assert.True(Enum.IsDefined(typeof(EStatusOS), EStatusOS.Cancelada));
    }

    [Fact]
    public void EStatusOS_ToString_ReturnsCorrectValues()
    {
        // Assert
        Assert.Equal("Recebida", EStatusOS.Recebida.ToString());
        Assert.Equal("EmDiagnostico", EStatusOS.EmDiagnostico.ToString());
        Assert.Equal("AguardandoAprovacao", EStatusOS.AguardandoAprovacao.ToString());
        Assert.Equal("EmExecucao", EStatusOS.EmExecucao.ToString());
    }

    [Fact]
    public void ETipoTelefone_HasAllRequiredTypes()
    {
        // Assert
        Assert.True(Enum.IsDefined(typeof(ETipoTelefone), ETipoTelefone.Celular));
        Assert.True(Enum.IsDefined(typeof(ETipoTelefone), ETipoTelefone.Residencial));
        Assert.True(Enum.IsDefined(typeof(ETipoTelefone), ETipoTelefone.Comercial));
    }

    [Fact]
    public void ETipoMovimentacao_HasAllRequiredTypes()
    {
        // Assert
        Assert.True(Enum.IsDefined(typeof(ETipoMovimentacao), ETipoMovimentacao.Entrada));
        Assert.True(Enum.IsDefined(typeof(ETipoMovimentacao), ETipoMovimentacao.Saida));
    }

    [Fact]
    public void EPerfilUsuario_HasAllRequiredProfiles()
    {
        // Assert
        Assert.True(Enum.IsDefined(typeof(EPerfilUsuario), EPerfilUsuario.Administrador));
        Assert.True(Enum.IsDefined(typeof(EPerfilUsuario), EPerfilUsuario.Funcionario));
        Assert.True(Enum.IsDefined(typeof(EPerfilUsuario), EPerfilUsuario.Mecanico));
        Assert.True(Enum.IsDefined(typeof(EPerfilUsuario), EPerfilUsuario.Cliente));
    }
}
