namespace rinha.model;

public record Cliente(int Id, decimal Limite, decimal SaldoInicial);

public record Transacao(decimal Valor, char Tipo, string Descricao, DateTime Realizada_em);

public class TransacaoResponse
{
    public decimal Limite {get;set;}
    public decimal Saldo {get;set;}
}
public class SaldoResponse
{
    public decimal Total {get;set;}
    public DateTime Data_extrato {get;set;}
    public List<Transacao> Ultimas_transacoes {get;set;} = [];
}