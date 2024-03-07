using Microsoft.EntityFrameworkCore;
using Rinha.Data;

namespace Rinha.Transacao;

public class ExtratoService(RinhaDbContext dbContext) : IExtratoService
{
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

        var saldo = new Saldo()
        {
            Data_extrato = DateTime.Now,
            Limite = StaticValidator.Limites[id],
            Total = cliente.Saldo
        };

        var ultimasTransacoes = cliente.Transacoes
            .OrderByDescending(t => t.Realizada_em)
            .Take(10)
            .ToList();

        return Results.Ok(new SaldoResponse()
        {
            Saldo = saldo,
            Ultimas_transacoes = ultimasTransacoes
        });
    }
}

public interface IExtratoService
{
    Task<IResult> ConsultaExtrato(int id);
}