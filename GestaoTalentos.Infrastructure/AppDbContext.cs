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
    
    /// DbSet para a entidade User, representando utilizadores na base de dados.
    public DbSet<User> Users { get; set; } = null!;
    
    /// DbSet para a entidade Cliente, representando clientes na base de dados.
    public DbSet<Cliente> Clientes { get; set; } = null!;
    
    /// DbSet para a entidade Apresentação, representante de apresentações na base de dados.
   // public DbSet<Apresentacao> Apresentacoes { get; set; } = null!;
    
    /// DbSet para a entidade Record, representando registros na base de dados.
    public DbSet<Record> Records { get; set; } = null!;
    
    // Configurar relacionamentos
    modelBuilder.Entity<Cliente>()
    .HasOne(c => c.User)
        .WithMany(a => a.Clientes)
        .HasForeignKey(s => s.IdCriador)
        .OnDelete(DeleteBehavior.Restrict);

 //   modelBuilder.Entity<Apresentacao>()
  //  .HasOne(a => a.Cliente)
   //     .WithMany(s => s.Apresentacoes)
    //    .HasForeignKey(a => a.IdCliente)
 //       .OnDelete(DeleteBehavior.Cascade);


    // Índices para performance
    modelBuilder.Entity<Cliente>()
    .HasIndex(c => c.IdMinhaConta)
        .IsUnique();

}
