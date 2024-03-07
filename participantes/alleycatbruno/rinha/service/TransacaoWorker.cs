using rinha.model;
using rinha.persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using NpgsqlTypes;

namespace rinha.transacao;

public class TransacaoWorker (RinhaDbContext dbContext): ITransacaoWorker
{
    public async Task<IResult> ExecutaTransacao(int id, [FromBody] Transacao transacao)
    {
        if(StaticValidator.IdOutOfRange(id))
        {
            return Results.NotFound();
        }

        if(StaticValidator.ZanValidator(transacao))
        {
            return Results.UnprocessableEntity();
        }

        bool IsCreditNotDebit = transacao.Tipo == "c";

        var conn = dbContext.Database.GetDbConnection();
        await conn.OpenAsync();
        using var command = conn.CreateCommand();

        var idParameter = new NpgsqlParameter("@cliente", NpgsqlDbType.Integer) { Value = id};
        var valorParameter = new NpgsqlParameter("@valor", NpgsqlDbType.Bigint) { Value = transacao.Valor };
        var descricaoParameter = new NpgsqlParameter("@descricao", NpgsqlDbType.Text) { Value = transacao.Descricao };
        
        command.CommandText = IsCreditNotDebit ? "Select credit(@cliente, @valor, @descricao);" : "Select debit(@cliente, @valor, @descricao);";
        command.Parameters.Add(idParameter);
        command.Parameters.Add(valorParameter);
        command.Parameters.Add(descricaoParameter);
        var result = await command.ExecuteScalarAsync();
        var returnedValue = result != DBNull.Value ? Convert.ToInt64(result) : 0;

        if (result == DBNull.Value)
        {
            return Results.UnprocessableEntity();
        }

        return Results.Ok(new TransacaoResponse(StaticValidator.Limites[id], returnedValue));        
    }

    public async Task<IResult> ConsultaExtrato(int id)
    {
        if(StaticValidator.IdOutOfRange(id))
        {
            return Results.NotFound();
        }

        var cliente = await dbContext.Clientes
            .Include(c => c.Transacoes)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        var saldo = new Saldo() { Data_extrato = DateTime.Now, Limite = StaticValidator.Limites[id], Total = cliente.Saldo };
        var ultimasTransacoes = cliente.Transacoes
            .OrderByDescending(t => t.Realizada_em)
            .Take(10)
            .ToList();
        return Results.Ok(new SaldoResponse() { Saldo = saldo, Ultimas_transacoes = ultimasTransacoes });
    }
}

public interface ITransacaoWorker
{
    Task<IResult> ExecutaTransacao(int id, [FromBody] Transacao transacao);
    Task<IResult> ConsultaExtrato(int id);
}