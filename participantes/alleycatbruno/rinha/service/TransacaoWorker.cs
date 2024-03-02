using rinha.persistence;
using rinha.model;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Npgsql;
using NpgsqlTypes;

namespace rinha.transacao;

public class TransacaoWorker(RinhaDbContext context, IValidatorService validatorService) : ITransacaoWorker
{
    public async Task<TransacaoResponse> ProcessarTransacao(Transacao transacao, int id)
    {
        if (id < 1 || id > 5)
        {
            validatorService.NaoExiste = true;
            return new TransacaoResponse();
        }
        bool IsCreditNotDebit = transacao.Tipo == 'c';

        var idParameter = new NpgsqlParameter("@cliente", NpgsqlDbType.Integer) { Value = id};
        var valorParameter = new NpgsqlParameter("@valor", NpgsqlDbType.Bigint) { Value = transacao.Valor };
        var descricaoParameter = new NpgsqlParameter("@descricao", NpgsqlDbType.Text) { Value = transacao.Descricao };

        var conn = context.Database.GetDbConnection();
        await conn.OpenAsync();
        using (var command = conn.CreateCommand())
        {
            command.CommandText = IsCreditNotDebit ? "Select debit(@cliente, @valor, @descricao);" : "Select debit(@cliente, @valor, @descricao);";
            command.Parameters.Add(idParameter);
            command.Parameters.Add(valorParameter);
            command.Parameters.Add(descricaoParameter);
            var result = await command.ExecuteScalarAsync();
            var returnedValue = result != DBNull.Value ? Convert.ToInt64(result) : 0;

            if (result == DBNull.Value)
            {
                validatorService.Overdraft = true;
                return new TransacaoResponse();
            }

            return new TransacaoResponse()
            {
                Saldo = returnedValue,
                Limite = Limites[id]
            };
        }
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

    private static readonly int[] Limites = [0, 1000 * 100, 800 * 100, 10000 * 100, 100000 * 100, 5000 * 100];
}

public interface  ITransacaoWorker
{
    Task<TransacaoResponse> ProcessarTransacao(Transacao transacao, int id);
    Task<Cliente?> ClienteExiste(int id);
    Task<string> TestarDB();
}