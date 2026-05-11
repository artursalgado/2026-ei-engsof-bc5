using GestaoTalentos.Domain;
using Microsoft.EntityFrameworkCore;

namespace GestaoTalentos.Infrastructure;

public class AreaRepository : IAreaRepository
{
    private readonly GestaoTalentos.Infrastructure.AppDbContext _context;
    
    /// Construtor que injeta o contexto da base de dados.
    /// <param name="context">Instância do AppDbContext.</param>
    public AreaRepository(GestaoTalentos.Infrastructure.AppDbContext context) => _context = context;

    public async Task<Area?> GetByNomeAsync(string nome) /// Obtém um utilizador pelo nome de utilizador.
    {
        var nomeNormalizado = nome.Trim().ToLower();
        return await _context.Areas
            .Include(a => a.Skills)
            .FirstOrDefaultAsync(a => a.Nome.ToLower() == nomeNormalizado);
    }
    public async Task<List<Area>> GetAllAsync() /// Obtém todos os utilizadores.
        => await _context.Areas.AsNoTracking().ToListAsync();
    public async Task<IEnumerable<Area>> GetAllWithSkilsAsync()
    {
        return await _context.Areas
            .Include(a => a.Skills)
            .OrderBy(a => a.Nome)
            .ToListAsync();
    }
    
    
    public async Task AddAsync(Area area)     /// Adiciona um novo utilizador na base de dados.
    {
        await _context.Areas.AddAsync(area);
        await _context.SaveChangesAsync();
    }
    public async Task<Area?> GetByIdAsync(int id)
        => await _context.Areas.Include(a => a.Skills).FirstOrDefaultAsync(a => a.Id == id);

    public async Task UpdateAsync(Area area)
    {
        _context.Areas.Update(area);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(string nome)    /// Exclui uma área pelo nome.
    {
        var area = await _context.Areas
            .FirstOrDefaultAsync(a => a.Nome.ToLower() == nome.ToLower());
        if (area != null)
        {
            _context.Areas.Remove(area);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(int id)    /// Exclui uma área pelo ID.
    {
        var area = await _context.Areas.FindAsync(id);
        if (area != null)
        {
            _context.Areas.Remove(area);
            await _context.SaveChangesAsync();
        }
    }
}