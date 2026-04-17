using Microsoft.EntityFrameworkCore;
using GestaoTalentos.Domain;

namespace GestaoTalentos.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; } = null!;
   // public DbSet<Record> Records { get; set; } = null!;
    public DbSet<Area> Areas { get; set; } = null!;
    public DbSet<Skill> Skills { get; set; } = null!;
    public DbSet<SkillNecessaria> SkillsNecessarias { get; set; } = null!;
    public DbSet<Proposta> Propostas { get; set; } = null!;
    public DbSet<TalentoElegivel> TalentosElegiveis { get; set; } = null!;

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
    
        modelBuilder.Entity<Proposta>()
            .HasOne(p => p.Area)
            .WithMany()
            .HasForeignKey(p => p.AreaId)
            .OnDelete(DeleteBehavior.Restrict);

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

        // Índices para performance
        modelBuilder.Entity<Proposta>()
            .HasIndex(p => p.Nome)
            .IsUnique();
    }
}