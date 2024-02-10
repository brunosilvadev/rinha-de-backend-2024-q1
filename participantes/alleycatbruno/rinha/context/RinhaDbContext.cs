using Microsoft.EntityFrameworkCore;
using rinha.model;


namespace rinha.persistence;

public class RinhaDbContext(DbContextOptions<RinhaDbContext> options) : DbContext(options)
{
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Transacao> Transacoes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>().HasKey(c => c.Id);
        modelBuilder.Entity<Transacao>().HasKey(t => t.TransacaoId);
        modelBuilder.Entity<Transacao>().HasOne(t => t.Cliente).WithMany()
			.HasForeignKey(c => c.ClienteId)
			.OnDelete(DeleteBehavior.SetNull)
			.IsRequired(true);

        base.OnModelCreating(modelBuilder);
    }
}