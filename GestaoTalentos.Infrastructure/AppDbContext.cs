using Microsoft.EntityFrameworkCore;
using GestaoTalentos.Domain;

namespace GestaoTalentos.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<Cliente> Clientes { get; set; } = null!;

    public DbSet<Perfil> Perfis { get; set; } = null!;
    public DbSet<Pais> Paises { get; set; } = null!;

    public DbSet<Area> Areas { get; set; } = null!;
    public DbSet<Skill> Skills { get; set; } = null!;
    public DbSet<SkillNecessaria> SkillsNecessarias { get; set; } = null!;
    public DbSet<ExperienciaProfissional> ExperienciasProfissionais { get; set; } = null!;
    public DbSet<PerfilSkill> PerfilSkills { get; set; } = null!;

    public DbSet<Log> Logs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // USER → ROLE
        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        // PERFIL → PAIS
        modelBuilder.Entity<Perfil>()
            .HasOne(p => p.Pais)
            .WithMany(p => p.Perfis)
            .HasForeignKey(p => p.PaisId)
            .OnDelete(DeleteBehavior.Restrict);

        // SKILL → AREA
        modelBuilder.Entity<Skill>()
            .HasOne(s => s.Area)
            .WithMany(a => a.Skills)
            .HasForeignKey(s => s.AreaId)
            .OnDelete(DeleteBehavior.Restrict);

        // SKILL NECESSARIA → SKILL
        modelBuilder.Entity<SkillNecessaria>()
            .HasOne(sn => sn.Skill)
            .WithMany(s => s.SkillsNecessarias)
            .HasForeignKey(sn => sn.SkillId)
            .OnDelete(DeleteBehavior.Restrict);

        // PERFIL → EXPERIENCIAS
        modelBuilder.Entity<ExperienciaProfissional>()
            .HasOne(e => e.Perfil)
            .WithMany(p => p.Experiencias)
            .HasForeignKey(e => e.PerfilId)
            .OnDelete(DeleteBehavior.Cascade);

        // PERFIL ↔ SKILL (N:N)
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

        // ÍNDICES
        modelBuilder.Entity<Skill>()
            .HasIndex(s => s.Nome)
            .IsUnique();

        modelBuilder.Entity<Area>()
            .HasIndex(a => a.Nome)
            .IsUnique();

        modelBuilder.Entity<Role>()
            .HasIndex(r => r.Nome)
            .IsUnique();

        modelBuilder.Entity<Pais>()
            .HasIndex(p => p.Nome)
            .IsUnique();

        // SEED
        modelBuilder.Entity<Area>().HasData(
            new Area { Id = 1, Nome = "Developer" },
            new Area { Id = 2, Nome = "Designer" },
            new Area { Id = 3, Nome = "Product Manager" },
            new Area { Id = 4, Nome = "Project Manager" }
        );
    }
}