using GestaoTalentos.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoTalentos.Infrastructure;

public class SkillRepository : ISkillRepository
{
    private readonly AppDbContext _context;

    public SkillRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Skill?> GetByIdAsync(int id)
    {
        return await _context.Skills
            .Include(s => s.Area)
            .Include(s => s.Abilidades)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<Skill>> GetAllAsync()
    {
        return await _context.Skills
            .Include(s => s.Area)
            .Include(s => s.Abilidades)
            .OrderBy(s => s.Nome)
            .ToListAsync();
    }

    public async Task<IEnumerable<Skill>> GetAllWithAreaAsync()
    {
        return await _context.Skills
            .Include(s => s.Area)
            .Include(s => s.Abilidades)
            .OrderBy(s => s.Area!.Nome)
            .ThenBy(s => s.Nome)
            .ToListAsync();
    }

    public async Task<Skill?> GetByNomeAsync(string nome)
    {
        return await _context.Skills
            .Include(s => s.Area)
            .FirstOrDefaultAsync(s => s.Nome.ToLower() == nome.ToLower());
    }

    public async Task<IEnumerable<Skill>> GetByAreaIdAsync(int areaId)
    {
        return await _context.Skills
            .Include(s => s.Area)
            .Include(s => s.Abilidades)
            .Where(s => s.AreaId == areaId)
            .OrderBy(s => s.Nome)
            .ToListAsync();
    }

    public async Task<bool> IsSkillAssociatedToPropostaAsync(int skillId)
    {
        return await _context.SkillsNecessarias
            .AnyAsync(sn => sn.SkillId == skillId);
    }

    public async Task AddAsync(Skill entity)
    {
        entity.CriadoEm = DateTime.UtcNow;
        entity.AtualizadoEm = DateTime.UtcNow;
        await _context.Skills.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Skill entity)
    {
        entity.AtualizadoEm = DateTime.UtcNow;
        _context.Skills.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var skill = await GetByIdAsync(id);
        if (skill != null)
        {
            _context.Skills.Remove(skill);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> AnyAsync()
    {
        return await _context.Skills.AnyAsync();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Skills.AnyAsync(s => s.Id == id);
    }
}