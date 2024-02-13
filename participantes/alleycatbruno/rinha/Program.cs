using Microsoft.EntityFrameworkCore;
using rinha.model;
using rinha.persistence;
using rinha.transacao;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ITransacaoWorker, TransacaoWorker>();

builder.Services.AddDbContext<RinhaDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/clientes/{id}/transacoes",
    async (int id, TransacaoRequest txn, ITransacaoWorker worker) =>
{
    var cliente = await worker.ClienteExiste(id);
    if(cliente == null)
    {
        return Results.NotFound();
    }
    var transacao = new Transacao(txn,id);
    var operacao = await worker.ProcessarTransacao(transacao);
    return Results.Ok(operacao);
});

app.MapGet("/clientes/{id}/extrato",
    async (int id, ITransacaoWorker worker) =>
{
    var cliente = await worker.ClienteExiste(id);
    if(cliente == null)
    {
        return Results.NotFound();
    }
    var saldo = await worker.ConsultarSaldo(id, cliente.Limite);
    return Results.Ok(saldo);
});

app.Run();