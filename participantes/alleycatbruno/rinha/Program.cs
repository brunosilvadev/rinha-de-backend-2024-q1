using Microsoft.EntityFrameworkCore;
using rinha.model;
using rinha.persistence;
using rinha.transacao;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ITransacaoWorker, TransacaoWorker>();

builder.Services.AddDbContext<RinhaDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();


app.MapGet("/clientes", async (RinhaDbContext ctx)=>
{
    return await ctx.Clientes.ToListAsync();
});

app.MapPost("/cliente", async (RinhaDbContext ctx, Cliente cliente)=>
{
    ctx.Clientes.Add(cliente);
    await ctx.SaveChangesAsync();
    return Results.Created($"/clientes/{cliente.Id}", cliente);
});

app.MapPost("/clientes/{id}/transacoes", async (int id, Transacao txn, ITransacaoWorker worker) =>
{
    return await worker.ProcessarTransacao(txn);
});

app.MapGet("/clientes/{id}/extrato", async (int id, ITransacaoWorker worker) =>
{
    return await worker.ConsultarSaldo(id);
});

app.Run();