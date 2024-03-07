using rinha.model;
using rinha.persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace rinha.transacao;

public static class TransacaoWorker
{
    public static async Task<IResult> ExecutaTransacao(int id, [FromBody] Transacao transacao, [FromServices] RinhaDbContext dbContext)
    {
        if(StaticValidator.IdOutOfRange(id))
        {
            return Results.NotFound();
        }

        if(StaticValidator.ZanValidator(transacao))
        {
            return Results.UnprocessableEntity();
        }

        var saldos = transacao.Tipo == "c" ?
        await dbContext.RetornoTransacao
            .FromSqlInterpolated($"SELECT * FROM credit({id}, {(int)transacao.Valor}, {transacao.Descricao})")
            .ToListAsync()
            :
        await dbContext.RetornoTransacao
            .FromSqlInterpolated($"SELECT * FROM debit({id}, {(int)transacao.Valor}, {transacao.Descricao})")
            .ToListAsync();

        if (saldos.FirstOrDefault()?.saldo_atual is null)
                return Results.UnprocessableEntity();

        return Results.Ok(new TransacaoResponse(StaticValidator.Limites[id], saldos.FirstOrDefault()?.saldo_atual ?? 0));

        /*
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
        */
    }

    // public async Task<string> TestarDB()
    // {
    //     try
    //     {
    //         await context.Database.ExecuteSqlRawAsync("SELECT 1;");
    //         return "Estamos no ar";
    //     }
    //     catch (Exception e)
    //     {
    //         return e.Message;
    //     }
    // }

    public static async Task<IResult> ConsultaExtrato(int id, [FromServices] RinhaDbContext dbContext)
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