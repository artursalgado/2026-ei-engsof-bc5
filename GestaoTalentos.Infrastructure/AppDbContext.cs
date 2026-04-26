using Microsoft.EntityFrameworkCore;
using GestaoTalentos.Domain;

namespace GestaoTalentos.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Cliente> Clientes { get; set; } = null!;
    public DbSet<Perfil> Perfis { get; set; } = null!;
    public DbSet<Area> Areas { get; set; } = null!;
    public DbSet<Skill> Skills { get; set; } = null!;
    public DbSet<SkillNecessaria> SkillsNecessarias { get; set; } = null!;
    public DbSet<ExperienciaProfissional> ExperienciasProfissionais { get; set; } = null!;
    public DbSet<PerfilSkill> PerfilSkills { get; set; } = null!;

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

        // Perfis - Experiencias (Cascade Delete)
        modelBuilder.Entity<ExperienciaProfissional>()
            .HasOne(e => e.Perfil)
            .WithMany(p => p.Experiencias)
            .HasForeignKey(e => e.PerfilId)
            .OnDelete(DeleteBehavior.Cascade);

        // Perfil - Skills (Muitos-para-Muitos)
        modelBuilder.Entity<PerfilSkill>()
            .HasKey(ps => new { ps.PerfilId, ps.SkillId });

        modelBuilder.Entity<PerfilSkill>()
            .HasOne(ps => ps.Perfil)
            .WithMany(p => p.PerfilSkills)
            .HasForeignKey(ps => ps.PerfilId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PerfilSkill>()
            .HasOne(ps => ps.Skill)
            .WithMany()
            .HasForeignKey(ps => ps.SkillId)
            .OnDelete(DeleteBehavior.Restrict);

        // Cliente → User (criador)
        modelBuilder.Entity<Cliente>()
            .HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.IdCriador)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices para performance
        modelBuilder.Entity<Skill>()
            .HasIndex(s => s.Nome)
            .IsUnique();

        modelBuilder.Entity<Area>()
            .HasIndex(a => a.Nome)
            .IsUnique();

        // SEED DE DADOS PARA TESTE INICIAL (De acordo com o enunciado):
        modelBuilder.Entity<Area>().HasData(
            new Area { Id = 1, Nome = "Developer" },
            new Area { Id = 2, Nome = "Designer" },
            new Area { Id = 3, Nome = "Product Manager" },
            new Area { Id = 4, Nome = "Project Manager" }
        );
    }
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
