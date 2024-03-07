using Microsoft.EntityFrameworkCore;
using Rinha.Data;
using Rinha.Transacao;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddDbContextPool<RinhaDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")), poolSize:100);

builder.Services.AddScoped<ITransacaoService, TransacaoService>();
builder.Services.AddScoped<IExtratoService, ExtratoService>();

var app = builder.Build();

app.MapPost("/clientes/{id}/transacoes", async (int id, Transacao transacao, ITransacaoService service)
    => await service.ExecutaTransacao(id,transacao));

app.MapGet("/clientes/{id}/extrato", async (int id, IExtratoService service)
    => await service.ConsultaExtrato(id));

app.Run();