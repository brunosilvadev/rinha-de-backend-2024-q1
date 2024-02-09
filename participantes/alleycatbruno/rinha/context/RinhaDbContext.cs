using Microsoft.EntityFrameworkCore;
using rinha.model;


namespace rinha.persistence;

public class RinhaDbContext(DbContextOptions<RinhaDbContext> options) : DbContext(options)
{
    public DbSet<Cliente> Clientes { get; set; }
}