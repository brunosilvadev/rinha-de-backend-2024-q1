using rinha.persistence;
using rinha.model;
using Microsoft.EntityFrameworkCore;

namespace rinha.transacao;

public class TransacaoWorker(RinhaDbContext context, IErrorService errService) : ITransacaoWorker
{
    public async Task<TransacaoResponse> ProcessarTransacao(Transacao transacao, int id)
    {
        var cliente = await ClienteExiste(id);

        if (cliente == null)
        {
            errService.NaoExiste = true;
            return new TransacaoResponse();
        }
            
        using var transaction = context.Database
            .BeginTransaction(System.Data.IsolationLevel.Serializable);

        if(transacao.Tipo == 'd')
        {
            var novoSaldo = cliente.Saldo - transacao.Valor;
            if((novoSaldo * -1) > cliente.Limite)
            {
                errService.Overdraft = true;
                return new TransacaoResponse();
            }
            cliente.Saldo -= transacao.Valor;
            cliente.Version = Guid.NewGuid().ToByteArray();
        }
        await context.Transacoes.AddAsync(transacao);
        

        if (transacao.Tipo == 'c')
        {
            cliente.Saldo += transacao.Valor;
        }
        
        if(await UpdateClienteAsync(cliente))
        {
            await transaction.CommitAsync();
            return await Task.FromResult(new TransacaoResponse()
            {
                Saldo = cliente.Saldo,
                Limite = cliente.Limite
            });
        }
        return new TransacaoResponse();
    }
    public async Task<SaldoResponse> ConsultarSaldo(int id, decimal limite)
    {        
        var ultimasTransacoes = await context.Transacoes.Where(t => t.ClienteId == id)
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
        => await context.Clientes.FirstOrDefaultAsync(c =>c.Id == id);

    public async Task<string> TestarDB()
    {
        try
        {
            await context.Database.ExecuteSqlRawAsync("SELECT 1;");
            return "Estamos no ar";
        }
        catch (Exception e)
        {
            return e.Message;
        }
    }

    private async Task<bool> UpdateClienteAsync(Cliente updatedCliente)
    {
        int tentativas = 3;
        for (int retry = 0; retry < tentativas; retry++)
        {
            try
            {
                context.Clientes.Update(updatedCliente);
                await context.SaveChangesAsync();
                return true;
            }
            catch(DbUpdateConcurrencyException ex)
            {
                Console.WriteLine("pegou");
                if(retry == tentativas -1)
                {
                    throw;
                }
                foreach(var entry in ex.Entries)
                {
                    if(entry.Entity is Cliente)
                    {
                        var dbValue = await entry.GetDatabaseValuesAsync();
                        if(dbValue == null)
                        {
                            return false;
                        }

                        entry.OriginalValues.SetValues(dbValue);
                    }
                }
            }
        }
        return false;
    }
}

public interface  ITransacaoWorker
{
    Task<TransacaoResponse> ProcessarTransacao(Transacao transacao, int id);
    Task<SaldoResponse> ConsultarSaldo(int id, decimal limite);
    Task<Cliente?> ClienteExiste(int id);
    Task<string> TestarDB();
}