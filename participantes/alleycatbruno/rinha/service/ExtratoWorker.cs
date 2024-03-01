using Microsoft.EntityFrameworkCore;
using rinha.model;
using rinha.persistence;

namespace rinha.transacao;

public class ExtratoWorker(RinhaDbContext context, IValidatorService validator) : IExtratoWorker
{
    public async Task<SaldoResponse?> ConsultarSaldo(int id)
    {        
        var transacoes = await context.Transacoes.Where(t => t.ClienteId == id)
                .OrderByDescending(t => t.TransacaoId)
                .Take(10)
                .AsNoTracking()
                .ToListAsync();
        
        var cliente = await ClienteExiste(id);
        if(cliente == null)
        {
            validator.NaoExiste = true;
            return null;
        }

        var response = new SaldoResponse()
        {
            Saldo = new Saldo()
            {
                Data_extrato = DateTime.UtcNow,
                Limite = cliente.Limite,
                Total = cliente.Saldo
            },
            Ultimas_transacoes = transacoes
        };
        
        return response;
    }

    public async Task<Cliente?> ClienteExiste(int id)
        => await context.Clientes.AsNoTracking().FirstOrDefaultAsync(c =>c.Id == id);
}

public interface IExtratoWorker
{
    Task<SaldoResponse?> ConsultarSaldo(int id);
    Task<Cliente?> ClienteExiste(int id);
}