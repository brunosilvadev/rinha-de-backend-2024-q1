using rinha.persistence;
using rinha.model;
using Microsoft.EntityFrameworkCore;

namespace rinha.transacao;

public class TransacaoWorker(RinhaDbContext context, IValidatorService validatorService, IHostApplicationLifetime app) : ITransacaoWorker
{
    public async Task<TransacaoResponse> ProcessarTransacao(Transacao transacao, int id)
    {

        

        var cliente = await ClienteExiste(id);
        bool creditOrDebit = true;

        if (cliente == null)
        {
            validatorService.NaoExiste = true;
            return new TransacaoResponse();
        }            

        if(transacao.Tipo == 'd')
        {
            var novoSaldo = cliente.Saldo - transacao.Valor;
            if((novoSaldo * -1) > cliente.Limite)
            {
                validatorService.Overdraft = true;
                return new TransacaoResponse();
            }
            creditOrDebit = false;
        }
        
        int tentativas = 50;
        for (int retry = 0; retry < tentativas; retry++)
        {
            try
            {
                using var transaction = await context.Database
                    .BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
                
                await context.Transacoes.AddAsync(transacao);

                var clienteToUpdate = await context.Clientes.FindAsync(id);
                if(clienteToUpdate == null) return new TransacaoResponse();

                clienteToUpdate.Saldo = creditOrDebit ?
                    clienteToUpdate.Saldo + transacao.Valor :
                    clienteToUpdate.Saldo - transacao.Valor;
                
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new TransacaoResponse()
                {
                    Saldo = cliente.Saldo,
                    Limite = clienteToUpdate.Limite
                };
            }
            catch
            {
                if(retry >= tentativas -1)
                {
                    throw;
                }               
            }
        }
        Console.WriteLine("falha fatal");
        app.StopApplication();
        return new TransacaoResponse();
    }

    public async Task<Cliente?> ClienteExiste(int id)
        => await context.Clientes.AsNoTracking().FirstOrDefaultAsync(c =>c.Id == id);

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
}

public interface  ITransacaoWorker
{
    Task<TransacaoResponse> ProcessarTransacao(Transacao transacao, int id);
    Task<Cliente?> ClienteExiste(int id);
    Task<string> TestarDB();
}