namespace rinha.model;

public record Cliente(int Id, decimal Limite, decimal SaldoInicial);

public class Transacao
{
    public int TransacaoId { get; set; }
    public decimal Valor { get; set; }
    public char Tipo { get; set; }
    public string? Descricao { get; set; }
    public DateTime Realizada_em { get; set; }
    public int ClienteId { get; set; }
    public virtual required Cliente Cliente { get; set; }
}
public class TransacaoResponse
{
    public decimal Limite { get; set; }
    public decimal Saldo { get; set; }
}
public class SaldoResponse
{
    public decimal Total { get; set; }
    public DateTime Data_extrato { get; set; }
    public List<Transacao> Ultimas_transacoes { get; set; } = [];
}