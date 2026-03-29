using Microsoft.EntityFrameworkCore;
using GestaoTalentos.Domain;

namespace GestaoTalentos.Infrastructure;

/// Contexto de base de dados da aplicação, herda de DbContext do Entity Framework.
/// Define os DbSets para as entidades User, Cliente e Record.
public class AppDbContext : DbContext
{
    /// Construtor que recebe as opções de configuração do DbContext.
    /// <param name="options">Opções de configuração para o DbContext.</param>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Cliente> Clientes { get; set; } = null!;
    public DbSet<Record> Records { get; set; } = null!;
    /*public DbSet<Apresentacao> Apresentacoes { get; set; } = null!;/// DbSet para a entidade Apresentação,
                                                                    //representante de apresentações na base de dados.*/

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurar relacionamentos
        modelBuilder.Entity<Cliente>()
            .HasOne(c => c.User)        // navigation property
            .WithMany(u => u.Clientes)
            .HasForeignKey(c => c.IdCriador)
            .OnDelete(DeleteBehavior.Restrict); // !!! Pode ser mudado implica não poder apagar User se ele tiver
                                               // algum cliente que não é o que queremos, mas para já não afeta nada!!!
        // Índices para performance
        modelBuilder.Entity<Cliente>()
            .HasIndex(c => c.IdMinhaConta)
            .IsUnique()
            .HasFilter("\"IdMinhaConta\" IS NOT NULL");

        /*modelBuilder.Entity<Apresentacao>()
            .HasOne(a => a.Cliente)
            .WithMany(s => s.Apresentacoes)
            .HasForeignKey(a => a.IdCliente)
            .OnDelete(DeleteBehavior.Cascade);*/
    }
}
