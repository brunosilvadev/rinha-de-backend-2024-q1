using Microsoft.EntityFrameworkCore;
using rinha.model;
using rinha.persistence;
using rinha.transacao;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ITransacaoWorker, TransacaoWorker>();
builder.Services.AddScoped<IExtratoWorker, ExtratoWorker>();
builder.Services.AddScoped<IValidatorService, ValidatorService>();

builder.Services.AddDbContext<RinhaDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/clientes/{id}/transacoes",
    async (int id, TransacaoRequest txn, ITransacaoWorker worker, IValidatorService errService) =>
{
    if(!errService.ValidaRequest(txn))
    {
        return Results.UnprocessableEntity();
    }

    var transacao = new Transacao(txn,id);
    var operacao = await worker.ProcessarTransacao(transacao, id);
    
    if(errService.NaoExiste)
    {
        return Results.NotFound();
    }

    if(errService.Overdraft)
    {
        return Results.UnprocessableEntity();
    }

    return Results.Ok(operacao);
});

app.MapGet("/clientes/{id}/extrato",
    async (int id, IExtratoWorker worker, IValidatorService errorService) =>
{
    var saldo = await worker.ConsultarSaldo(id);

    if(errorService.NaoExiste)
    {
        return Results.NotFound();
    }
    
    return Results.Ok(saldo);
});

app.MapGet("/test-api", () => true);

app.MapGet("/test-db", (ITransacaoWorker worker) =>
{
    return worker.TestarDB();
});

app.Run();