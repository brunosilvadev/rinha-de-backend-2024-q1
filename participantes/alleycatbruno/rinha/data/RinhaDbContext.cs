using Microsoft.EntityFrameworkCore;

namespace Rinha.Data;

public class RinhaDbContext(DbContextOptions<RinhaDbContext> options) : DbContext(options)
{
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Transacao> Transacoes { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Transacao>()
            .HasOne(t => t.Cliente)
            .WithMany(c => c.Transacoes)
            .HasForeignKey(t => t.ClienteId);

        base.OnModelCreating(modelBuilder);
    }
}