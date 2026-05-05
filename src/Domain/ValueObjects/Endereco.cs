public class Endereco
{
    public string Logradouro { get; private set; }
    public string Numero { get; private set; }
    public string? Complemento { get; private set; }
    public string Bairro { get; private set; }
    public string Cep { get; private set; }
    public string Cidade { get; private set; }
    public string Uf { get; private set; }

    public Endereco(string logradouro, string numero, string? complemento, string bairro, string cidade, string uf, string cep)
    {
        if (string.IsNullOrWhiteSpace(logradouro)) throw new ArgumentException("Logradouro é obrigatório.");
        if (string.IsNullOrWhiteSpace(numero)) throw new ArgumentException("Número é obrigatório.");
        if (string.IsNullOrWhiteSpace(cep)) throw new ArgumentException("CEP é obrigatório.");
        if (string.IsNullOrWhiteSpace(cidade)) throw new ArgumentException("Cidade é obrigatória.");
        if (string.IsNullOrWhiteSpace(uf) || uf.Trim().Length != 2) throw new ArgumentException("UF inválida.");

        Logradouro = logradouro.Trim();
        Numero = numero.Trim();
        Complemento = complemento?.Trim() ?? string.Empty;
        Bairro = bairro.Trim();
        Cidade = cidade.Trim();
        Uf = uf.Trim().ToUpper();
        Cep = cep.Trim().Replace("-", "").Replace(".", "");
    }

    protected Endereco() { }
}