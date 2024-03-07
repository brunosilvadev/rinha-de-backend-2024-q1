using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Rinha.Data;
public class Cliente(int Id, int Limite, long Saldo)
{
    public int Id { get; set; } = Id;
    public int Limite { get; set; } = Limite;
    public long Saldo { get; set; } = Saldo;
    public ICollection<Transacao> Transacoes { get; set; } = [];
}

public class Transacao
{
    public Transacao()
    {
        Realizada_em = DateTime.UtcNow;
    }
    public int TransacaoId { get; set; }
    public decimal Valor { get; set; }
    [MaxLength(1)]
    public string Tipo { get; set; }
    [MaxLength(100)]
    public string? Descricao { get; set; }
    [JsonIgnore]
    public int? ClienteId { get; set; }
    [JsonIgnore]
    public Cliente? Cliente { get; set; }
    public DateTime Realizada_em { get; set; }
}
public class TransacaoResponse(int Limite, long Saldo)
{
    public int Limite { get; set; } = Limite;
    public long Saldo { get; set; } = Saldo;
}
public class Saldo
{
        public long Total { get; set; }
        public DateTime Data_extrato { get; set; }
        public int Limite { get; set; }
}
public class SaldoResponse
{
    public required Saldo Saldo { get; set; }
    public List<Transacao> Ultimas_transacoes { get; set; } = [];
}

