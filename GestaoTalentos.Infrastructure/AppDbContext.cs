using Microsoft.EntityFrameworkCore;
using GestaoTalentos.Domain;

namespace GestaoTalentos.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Record> Records { get; set; } = null!;
    public DbSet<Area> Areas { get; set; } = null!;
    public DbSet<Skill> Skills { get; set; } = null!;
    public DbSet<Abilidade> Abilidades { get; set; } = null!;
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

        // Índices para performance
        modelBuilder.Entity<Skill>()
            .HasIndex(s => s.Nome)
            .IsUnique();

        modelBuilder.Entity<Area>()
            .HasIndex(a => a.Nome)
            .IsUnique();
    }
}