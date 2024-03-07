using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.EntityFrameworkCore;
using rinha.persistence;
using rinha.transacao;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddDbContextPool<RinhaDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")), poolSize:100);

builder.Services.AddRequestTimeouts(options => options.DefaultPolicy = new RequestTimeoutPolicy { Timeout = TimeSpan.FromSeconds(60) });

var app = builder.Build();

app.MapPost("/clientes/{id}/transacoes", TransacaoWorker.ExecutaTransacao);

app.MapGet("/clientes/{id}/extrato", TransacaoWorker.ConsultaExtrato);

app.Run();