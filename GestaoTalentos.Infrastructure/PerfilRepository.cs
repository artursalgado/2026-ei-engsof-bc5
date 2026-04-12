using GestaoTalentos.Domain;
using Microsoft.EntityFrameworkCore;

namespace GestaoTalentos.Infrastructure;

public class PerfilRepository : IPerfilRepository
{
    private readonly AppDbContext _context;
    private DbSet<Perfil> Perfis => _context.Set<Perfil>();

    public PerfilRepository(AppDbContext context) => _context = context;

    public async Task<Perfil?> GetByIdAsync(int id)
        => await Perfis.FindAsync(id);

    public async Task<List<Perfil>> GetAllAsync()
        => await Perfis.AsNoTracking().ToListAsync();

    public async Task<List<Perfil>> GetVisibleForUserAsync(int userId)
        => await Perfis.AsNoTracking()
            .Where(r => r.OwnerId == userId || r.IsShared)//Ou quando user é Cliente e foi Apresentado
            .ToListAsync();
    
    public async Task<IEnumerable<Perfil>> GetByPaisIdAsync(int paisId)
    {
        return await _context.Perfis
            .Where(p => p.PaisId == paisId)
            .OrderBy(p => p.Id)
            .ToListAsync();
    }
    
    public async Task AddAsync(Perfil perfil)
    {
        await Perfis.AddAsync(perfil);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Perfil perfil)
    {
        Perfis.Update(perfil);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await Perfis.FindAsync(id);
        if (entity != null)
        {
            Perfis.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}