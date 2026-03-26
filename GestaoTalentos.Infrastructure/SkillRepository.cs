
using System.Collections.Generic;
using System.Threading.Tasks;
using GestaoTalentos.Domain;
using Microsoft.EntityFrameworkCore;

namespace GestaoTalentos.Infrastructure;

public class SkillRepository : Repository<Skill>, ISkillRepository
{
	public SkillRepository(AppDbContext context) : base(context) { }

	public async Task<Skill?> GetByNomeAsync(string nome)
	{
		var n = (nome ?? string.Empty).Trim();
		return await _context.Skills.FirstOrDefaultAsync(s => s.Nome == n);
	}

	public async Task<IEnumerable<Skill>> GetByAreaIdAsync(int areaId)
	{
		return await _context.Skills
			.Where(s => s.AreaId == areaId)
			.OrderBy(s => s.Nome)
			.ToListAsync();
	}

	public async Task<IEnumerable<Skill>> GetAllWithAreaAsync()
	{
		return await _context.Skills
			.Include(s => s.Area)
			.OrderBy(s => s.Nome)
			.ToListAsync();
	}
}