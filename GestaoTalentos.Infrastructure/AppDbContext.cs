using Microsoft.EntityFrameworkCore;
using GestaoTalentos.Domain;

namespace GestaoTalentos.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Perfil> Perfis { get; set; } = null!;
    public DbSet<Area> Areas { get; set; } = null!;
    public DbSet<Skill> Skills { get; set; } = null!;
    public DbSet<SkillNecessaria> SkillsNecessarias { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurar relacionamentos
        modelBuilder.Entity<Skill>()
            .HasOne(s => s.Area)
            .WithMany(a => a.Skills)
            .HasForeignKey(s => s.AreaId)
            .OnDelete(DeleteBehavior.Restrict);



        modelBuilder.Entity<SkillNecessaria>()
            .HasOne(sn => sn.Skill)
            .WithMany(s => s.SkillsNecessarias)
            .HasForeignKey(sn => sn.SkillId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices para performance
        modelBuilder.Entity<Skill>()
            .HasIndex(s => s.Nome)
            .IsUnique();

        modelBuilder.Entity<Area>()
            .HasIndex(a => a.Nome)
            .IsUnique();

        // SEED DE DADOS PARA TESTE INICIAL:
        modelBuilder.Entity<Area>().HasData(
            new Area { Id = 1, Nome = "Eletromecânica" },
            new Area { Id = 2, Nome = "Engenharia de Software" },
            new Area { Id = 3, Nome = "Gestão de Projetos" }
        );
    }
}