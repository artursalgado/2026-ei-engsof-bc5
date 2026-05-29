using GestaoTalentos.Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoTalentos.Infrastructure;

public class PropostaRepository : Repository<Proposta>, IPropostaRepository
{
    public PropostaRepository(AppDbContext context) : base(context) { }

    public async Task<Proposta?> GetByIdWithSkillsAsync(int id)
    {
        return await _context.Propostas
            .Include(p => p.Area)
            .Include(p => p.SkillsNecessarias)
            .ThenInclude(sn => sn.Skill)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Proposta>> GetAllWithSkillsAsync()
    {
        return await _context.Propostas
            .Include(p => p.Area)
            .Include(p => p.SkillsNecessarias)
            .ThenInclude(sn => sn.Skill)
            .Include(p => p.TalentosElegiveis)
            .OrderBy(p => p.Nome)
            .ToListAsync();
    }

    public async Task<IEnumerable<Proposta>> GetAllWithSkillsByCreatorAsync(int creatorId)
    {
        return await _context.Propostas
            .Include(p => p.Area)
            .Include(p => p.SkillsNecessarias)
            .ThenInclude(sn => sn.Skill)
            .Include(p => p.TalentosElegiveis)
            .Where(p => p.CriadorId == creatorId)
            .OrderBy(p => p.Nome)
            .ToListAsync();
    }

    public async Task<Proposta?> GetByNomeAsync(string nome)
    {
        var n = (nome ?? string.Empty).Trim();
        return await _context.Propostas.FirstOrDefaultAsync(p => p.Nome == n);
    }

    public async Task<IEnumerable<Proposta>> GetByAreaIdAsync(int areaId)
    {
        return await _context.Propostas
            .Include(p => p.Area)
            .Where(p => p.AreaId == areaId)
            .OrderBy(p => p.Nome)
            .ToListAsync();
    }

    public override async Task UpdateAsync(Proposta proposta)
    {
        _context.Propostas.Update(proposta);
        await _context.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}