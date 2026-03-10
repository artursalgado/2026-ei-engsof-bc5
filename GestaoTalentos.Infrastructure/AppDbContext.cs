using Microsoft.EntityFrameworkCore;
using GestaoTalentos.Domain;

namespace GestaoTalentos.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; } = null!;
}
