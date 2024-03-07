using Rinha.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using NpgsqlTypes;

namespace Rinha.Transacao;

public class TransacaoService (RinhaDbContext dbContext): ITransacaoService
{
    public async Task<IResult> ExecutaTransacao(int id, [FromBody] Data.Transacao transacao)
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

        var result = await NovoSaldo(id, transacao.Valor, transacao.Descricao, IsCreditNotDebit);
        
        if (result == DBNull.Value)
        {
            return Results.UnprocessableEntity();
        }

        var novoSaldo = Convert.ToInt64(result);        

        return Results.Ok(new TransacaoResponse(StaticValidator.Limites[id], novoSaldo));        
    }

    private async Task<object?> NovoSaldo(int id, decimal valor, string? descricao, bool IsCreditNotDebit)
    {
        var conn = dbContext.Database.GetDbConnection();
        await conn.OpenAsync();
        using var command = conn.CreateCommand();

        var idParameter = new NpgsqlParameter("@cliente", NpgsqlDbType.Integer) { Value = id};
        var valorParameter = new NpgsqlParameter("@valor", NpgsqlDbType.Bigint) { Value = valor };
        var descricaoParameter = new NpgsqlParameter("@descricao", NpgsqlDbType.Text) { Value = descricao };     

        command.CommandText = IsCreditNotDebit ?
                "Select credit(@cliente, @valor, @descricao);"
              : "Select debit(@cliente, @valor, @descricao);";
        command.Parameters.Add(idParameter);
        command.Parameters.Add(valorParameter);
        command.Parameters.Add(descricaoParameter);
        
        return await command.ExecuteScalarAsync();
    }
}

public interface ITransacaoService
{
    Task<IResult> ExecutaTransacao(int id, [FromBody] Data.Transacao transacao);
}