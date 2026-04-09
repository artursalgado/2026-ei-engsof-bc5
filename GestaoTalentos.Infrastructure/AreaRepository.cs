using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestaoTalentos.Domain;
using Microsoft.EntityFrameworkCore;

namespace GestaoTalentos.Infrastructure;

public class AreaRepository : Repository<Area>, IAreaRepository
{
    public AreaRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Area>> GetAllAsync()
    {
        return await _context.Areas.OrderBy(a => a.Nome).ToListAsync();
    }

    public async Task<Area?> GetByNomeAsync(string nome)
    {
        return await _context.Areas.FirstOrDefaultAsync(a => a.Nome == nome);
    }

    public async Task<IEnumerable<Area>> GetAllWithSkillsAsync()
    {
        return await _context.Areas.Include(a => a.Skills).OrderBy(a => a.Nome).ToListAsync();
    }
}
