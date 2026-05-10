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
    public DbSet<Proposta> Propostas { get; set; } = null!;
    public DbSet<TalentoElegivel> TalentosElegiveis { get; set; } = null!;
    public DbSet<Log> Logs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Ignorar entidades sem tabela na BD (não gerar relações automáticas)
        modelBuilder.Ignore<Role>();
        modelBuilder.Ignore<Pais>();

        // CLIENTE → USER
        modelBuilder.Entity<Cliente>()
            .HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.IdCriador)
            .OnDelete(DeleteBehavior.Restrict);

        // PERFIL → USER (owner)
        modelBuilder.Entity<Perfil>()
            .HasOne(pf => pf.Owner)
            .WithMany()
            .HasForeignKey(pf => pf.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        // SKILL → AREA
        modelBuilder.Entity<Skill>()
            .HasOne(s => s.Area)
            .WithMany(a => a.Skills)
            .HasForeignKey(s => s.AreaId)
            .OnDelete(DeleteBehavior.Restrict);

        // PROPOSTA → AREA
        modelBuilder.Entity<Proposta>()
            .HasOne(p => p.Area)
            .WithMany()
            .HasForeignKey(p => p.AreaId)
            .OnDelete(DeleteBehavior.Restrict);

        // SKILL NECESSARIA → SKILL + PROPOSTA
        modelBuilder.Entity<SkillNecessaria>()
            .HasOne(sn => sn.Skill)
            .WithMany(s => s.SkillsNecessarias)
            .HasForeignKey(sn => sn.SkillId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SkillNecessaria>()
            .HasOne<Proposta>()
            .WithMany(p => p.SkillsNecessarias)
            .HasForeignKey(sn => sn.PropostaId)
            .OnDelete(DeleteBehavior.Cascade);

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

        // TALENTO ELEGIVEL → PROPOSTA + PERFIL
        modelBuilder.Entity<TalentoElegivel>()
            .HasOne(te => te.Proposta)
            .WithMany(p => p.TalentosElegiveis)
            .HasForeignKey(te => te.PropostaId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TalentoElegivel>()
            .HasOne(te => te.Perfil)
            .WithMany()
            .HasForeignKey(te => te.PerfilId)
            .OnDelete(DeleteBehavior.Cascade);

        // ÍNDICES ÚNICOS
        modelBuilder.Entity<Skill>().HasIndex(s => s.Nome).IsUnique();
        modelBuilder.Entity<Area>().HasIndex(a => a.Nome).IsUnique();
        modelBuilder.Entity<Proposta>().HasIndex(p => p.Nome).IsUnique();
    }
}
