using GestaoTalentos.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoTalentos.Infrastructure;

public class AbilidadeRepository : IAbilidadeRepository
{
    private readonly AppDbContext _context;

    public AbilidadeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Abilidade?> GetByIdAsync(int id)
    {
        return await _context.Abilidades
            .Include(a => a.Skill)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Abilidade>> GetAllAsync()
    {
        return await _context.Abilidades
            .Include(a => a.Skill)
            .OrderBy(a => a.Nome)
            .ToListAsync();
    }

    public async Task<IEnumerable<Abilidade>> GetBySkillIdAsync(int skillId)
    {
        return await _context.Abilidades
            .Where(a => a.SkillId == skillId)
            .OrderBy(a => a.Nome)
            .ToListAsync();
    }

    public async Task<Abilidade?> GetByNomeAndSkillIdAsync(string nome, int skillId)
    {
        return await _context.Abilidades
            .FirstOrDefaultAsync(a => a.Nome.ToLower() == nome.ToLower() && a.SkillId == skillId);
    }

    public async Task AddAsync(Abilidade entity)
    {
        await _context.Abilidades.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Abilidade entity)
    {
        _context.Abilidades.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var abilidade = await GetByIdAsync(id);
        if (abilidade != null)
        {
            _context.Abilidades.Remove(abilidade);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> AnyAsync()
    {
        return await _context.Abilidades.AnyAsync();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Abilidades.AnyAsync(a => a.Id == id);
    }
}