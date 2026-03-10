using GestaoTalentos.Domain;
using Microsoft.EntityFrameworkCore;

namespace GestaoTalentos.Infrastructure;

public class RecordRepository : IRecordRepository
{
    private readonly AppDbContext _context;
    private DbSet<Record> Records => _context.Set<Record>();

    public RecordRepository(AppDbContext context) => _context = context;

    public async Task<Record?> GetByIdAsync(int id)
        => await Records.FindAsync(id);

    public async Task<List<Record>> GetAllAsync()
        => await Records.AsNoTracking().ToListAsync();

    public async Task<List<Record>> GetVisibleForUserAsync(int userId)
        => await Records.AsNoTracking()
            .Where(r => r.OwnerId == userId || r.IsShared)
            .ToListAsync();

    public async Task AddAsync(Record record)
    {
        await Records.AddAsync(record);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Record record)
    {
        Records.Update(record);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await Records.FindAsync(id);
        if (entity != null)
        {
            Records.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}