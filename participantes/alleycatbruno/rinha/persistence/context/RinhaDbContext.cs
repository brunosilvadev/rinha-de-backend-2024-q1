using Microsoft.EntityFrameworkCore;
using rinha.model;


namespace rinha.persistence;

public class RinhaDbContext : DbContext
{
    public RinhaDbContext(DbContextOptions<RinhaDbContext> options)
        : base(options)
    {
    }

    public DbSet<Cliente> Clientes { get; set; }
}