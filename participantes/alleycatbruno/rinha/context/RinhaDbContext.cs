using Microsoft.EntityFrameworkCore;
using rinha.model;

namespace rinha.persistence;

public class RinhaDbContext(DbContextOptions<RinhaDbContext> options) : DbContext(options)
{
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Transacao> Transacoes { get; set; }
    public DbSet<RetornoTransacao> RetornoTransacao { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Transacao>()
            .HasOne(t => t.Cliente)
            .WithMany(c => c.Transacoes)
            .HasForeignKey(t => t.ClienteId);

        modelBuilder.Entity<RetornoTransacao>().HasNoKey();

        base.OnModelCreating(modelBuilder);
    }
}