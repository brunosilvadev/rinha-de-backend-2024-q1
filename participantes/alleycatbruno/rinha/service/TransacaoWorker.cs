using rinha.persistence;
using rinha.model;
using Microsoft.EntityFrameworkCore;

namespace rinha.transacao;

public class TransacaoWorker(RinhaDbContext context) : ITransacaoWorker
{
    private readonly RinhaDbContext _context = context;

    public async Task<TransacaoResponse> ProcessarTransacao(Transacao transacao)
    {
        await _context.Transacoes.AddAsync(transacao);
        var cliente = await _context.Clientes
            .Where(c => c.Id == transacao.ClienteId)
            .FirstOrDefaultAsync() ?? throw new Exception("Cliente nao encontrado");

        if (transacao.Tipo == 'c')
        {
            cliente.Saldo += transacao.Valor;
            cliente.Limite -= transacao.Valor;
        }
        if (transacao.Tipo  == 'd')
        {
            cliente.Saldo -=  transacao.Valor;
            cliente.Limite += transacao.Valor;
        }
        
        await _context.SaveChangesAsync();
        return await Task.FromResult(new TransacaoResponse()
        {
            Saldo = cliente.Saldo,
            Limite = cliente.Limite
        });
    }
    public async Task<SaldoResponse> ConsultarSaldo(int id)
    {
        var ultimasTransacoes = _context.Transacoes.Where(t => t.ClienteId == id)
            .OrderByDescending(t => t.TransacaoId)
            .Take(10)
            .ToList();

        return await Task.FromResult(new SaldoResponse()
        {
            Saldo = new Saldo()
            {
                Data_extrato = DateTime.Now.ToUniversalTime(),
                Limite = 0,
                Total = ultimasTransacoes.Sum(t => t.Valor)
            },
            Ultimas_transacoes = ultimasTransacoes
        });
    }
}

public interface  ITransacaoWorker
{
    Task<TransacaoResponse> ProcessarTransacao(Transacao transacao);
    Task<SaldoResponse> ConsultarSaldo(int id);
}