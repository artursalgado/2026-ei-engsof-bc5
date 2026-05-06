using GestaoTalentos.Domain;

namespace GestaoTalentos.Infrastructure;

public class DatabaseLogObserver : ILogObserver
{
    private readonly AppDbContext _context;

    public DatabaseLogObserver(AppDbContext context)
    {
        _context = context;
    }

    public async Task UpdateAsync(Log log)
    {
        _context.Logs.Add(log);
        await _context.SaveChangesAsync();
    }
}