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
        }
        if (transacao.Tipo  == 'd')
        {
            cliente.Saldo -=  transacao.Valor;
        }
        
        await _context.SaveChangesAsync();
        return await Task.FromResult(new TransacaoResponse()
        {
            Saldo = cliente.Saldo,
            Limite = cliente.Limite
        });
    }
    public async Task<SaldoResponse> ConsultarSaldo(int id, decimal limite)
    {        
        var ultimasTransacoes = await _context.Transacoes.Where(t => t.ClienteId == id)
            .OrderByDescending(t => t.TransacaoId)
            .Take(10)
            .ToListAsync();
        
        return await Task.FromResult(new SaldoResponse()
        {
            Saldo = new Saldo()
            {
                Data_extrato = DateTime.Now.ToUniversalTime(),
                Limite = limite,
                Total = ultimasTransacoes
                    .Sum(t => t.Tipo == 'c' ? t.Valor : -t.Valor)
            },  
            Ultimas_transacoes = ultimasTransacoes
        });
    }

    public async Task<Cliente?> ClienteExiste(int id)
        => await _context.Clientes.FirstOrDefaultAsync(c =>c.Id == id);

    public async Task<string> TestarDB()
    {
        try
        {
            await _context.Database.ExecuteSqlRawAsync("SELECT 1;");
            return "Estamos no ar";
        }
        catch (Exception e)
        {
            return e.Message;
        }
    }
}

public interface  ITransacaoWorker
{
    Task<TransacaoResponse> ProcessarTransacao(Transacao transacao);
    Task<SaldoResponse> ConsultarSaldo(int id, decimal limite);
    Task<Cliente?> ClienteExiste(int id);
    Task<string> TestarDB();
}