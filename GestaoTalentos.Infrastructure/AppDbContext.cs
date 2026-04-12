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
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<Perfil> Perfis { get; set; } = null!;
    public DbSet<Pais> Paises { get; set; } = null!;
    public DbSet<Skill> Skills { get; set; } = null!;
    public DbSet<Area> Areas { get; set; } = null!;
    public DbSet<Abilidade> Abilidades { get; set; } = null!;
    public DbSet<SkillNecessaria> SkillsNecessarias { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurar relacionamentos
        
        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Skill>()
            .HasOne(s => s.Area)
            .WithMany(a => a.Skills)
            .HasForeignKey(s => s.AreaId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Abilidade>()
            .HasOne(a => a.Skill)
            .WithMany(s => s.Abilidades)
            .HasForeignKey(a => a.SkillId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SkillNecessaria>()
            .HasOne(sn => sn.Skill)
            .WithMany(s => s.SkillsNecessarias)
            .HasForeignKey(sn => sn.SkillId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Perfil>()
            .HasOne(p => p.Pais)
            .WithMany(p => p.Perfis)
            .HasForeignKey(p => p.PaisId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Índices para performance
        modelBuilder.Entity<Skill>()
            .HasIndex(s => s.Nome)
            .IsUnique();

        modelBuilder.Entity<Area>()
            .HasIndex(a => a.Nome)
            .IsUnique();
        modelBuilder.Entity<Role>()
            .HasIndex(a => a.Nome)
            .IsUnique();
        modelBuilder.Entity<Pais>()
            .HasIndex(a => a.Nome)
            .IsUnique();
    }
}