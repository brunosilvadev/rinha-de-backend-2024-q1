using System.Text.Json.Serialization;

namespace rinha.model;
public class Cliente
{
    public int Id { get; set; }
    public decimal Limite { get; set; }
    public decimal Saldo { get; set; }
}

public class TransacaoRequest
{
    public decimal Valor { get; set; }
    public char Tipo { get; set; }
    public string? Descricao { get; set; }
}
public class Transacao
{
    public Transacao() { }
    public Transacao(TransacaoRequest request, int id)
    {
        if (id == 0)
        {
            throw new ArgumentNullException("Transacao deve conter Id do cliente");
        }
        if (request.Tipo is not ('d' or 'c'))
        {
            throw new ArgumentNullException("Transacao deve ser credito ou debito");
        }

        Valor = request.Valor;
        Tipo = request.Tipo;
        Descricao = request.Descricao;
        Realizada_em = DateTime.Now.ToUniversalTime();
        ClienteId = id;

    }
    [JsonIgnore]
    public int TransacaoId { get; set; }
    public decimal Valor { get; set; }
    public char Tipo { get; set; }
    public string? Descricao { get; set; }
    public DateTime Realizada_em { get; set; }

    [JsonIgnore]
    public int ClienteId { get; set; }
}
public class TransacaoResponse
{
    public decimal Limite { get; set; }
    public decimal Saldo { get; set; }
}
public class Saldo
{
    public decimal Total { get; set; }
    public DateTime Data_extrato { get; set; }
    public decimal Limite { get; set; }
}
public class SaldoResponse
{
    public Saldo Saldo { get; set; }
    public List<Transacao> Ultimas_transacoes { get; set; } = [];
}