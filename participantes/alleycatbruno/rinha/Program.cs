using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.EntityFrameworkCore;
using rinha.model;
using rinha.persistence;
using rinha.transacao;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddDbContextPool<RinhaDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")), poolSize:100);

builder.Services.AddScoped<ITransacaoWorker, TransacaoWorker>();

var app = builder.Build();

app.MapPost("/clientes/{id}/transacoes", async(int id, Transacao transacao, ITransacaoWorker worker)
    => await worker.ExecutaTransacao(id,transacao));

app.MapGet("/clientes/{id}/extrato", async(int id, ITransacaoWorker worker)
    => await worker.ConsultaExtrato(id));

app.Run();