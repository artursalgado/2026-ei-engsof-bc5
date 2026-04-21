using GestaoTalentos.Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoTalentos.Infrastructure;

public class PerfilRepository : IPerfilRepository
{
    private readonly AppDbContext _context;

    public PerfilRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Perfil>> GetAllAsync()
    {
        return await _context.Perfis
            .Include(pf => pf.Owner)
            .ToListAsync();
    }

    public async Task<Perfil?> GetByIdAsync(int id)
    {
        return await _context.Perfis
            .Include(pf => pf.Owner)
            .FirstOrDefaultAsync(pf => pf.Id == id);
    }

    public async Task<List<Perfil>> GetVisibleForUserAsync(int userId)
    {
        return await _context.Perfis
            .AsNoTracking()
            .Where(r => r.OwnerId == userId || r.IsShared)
            .ToListAsync();
    }

    public async Task AddAsync(Perfil perfil)
    {
        await _context.Perfis.AddAsync(perfil);
    }

    public async Task UpdateAsync(Perfil perfil)
    {
        _context.Perfis.Update(perfil);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(int id)
    {
        var perfil = await GetByIdAsync(id);
        if (perfil != null)
        {
            _context.Perfis.Remove(perfil);
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}