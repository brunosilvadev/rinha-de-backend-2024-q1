using rinha.persistence;
using rinha.model;

namespace rinha.transacao;

public class TransacaoWorker(RinhaDbContext context) : ITransacaoWorker
{
    private readonly RinhaDbContext _context = context;

    public async Task<TransacaoResponse> ProcessarTransacao(Transacao transacao)
    {
        return await Task.FromResult(new TransacaoResponse());
    }
    public async Task<SaldoResponse> ConsultarSaldo(int id)
    {
        return await Task.FromResult(new SaldoResponse());
    }
}

public interface  ITransacaoWorker
{
    Task<TransacaoResponse> ProcessarTransacao(Transacao transacao);
    Task<SaldoResponse> ConsultarSaldo(int id);
}