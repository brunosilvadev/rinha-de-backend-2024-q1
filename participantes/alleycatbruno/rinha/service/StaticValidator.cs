using rinha.model;

namespace rinha.transacao;

public static class StaticValidator
{
    public static readonly int[] Limites = [0, 1000 * 100, 800 * 100, 10000 * 100, 100000 * 100, 5000 * 100];
    public static bool IdOutOfRange(int id) => id < 1 || id > 5;
    public static bool ZanValidator(Transacao transacao) =>
                string.IsNullOrEmpty(transacao.Descricao) ||
                transacao.Descricao.Length > 10 ||
                transacao.Valor % 1 != 0 || 
                transacao.Valor < 0 ||
                (transacao.Tipo != "c" && transacao.Tipo != "d");
}