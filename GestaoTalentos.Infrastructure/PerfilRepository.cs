using GestaoTalentos.Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoTalentos.Infrastructure;

// Repositório completo de Perfis com suporte a Experiências e Skills
public class PerfilRepository : IPerfilRepository
{
    private readonly AppDbContext _context;

    public PerfilRepository(AppDbContext context)
    {
        _context = context;
    }

    // Busca simples por ID (com tudo relacionado incluído)
    public async Task<Perfil?> GetByIdAsync(int id)
        => await _context.Perfis
            .Include(p => p.Experiencias)
            .Include(p => p.PerfilSkills)
                .ThenInclude(ps => ps.Skill)
            .FirstOrDefaultAsync(p => p.Id == id);

    // Lista todos os perfis com experiências e skills
    public async Task<IEnumerable<Perfil>> GetAllAsync()
        => await _context.Perfis
            .Include(p => p.Experiencias)
            .Include(p => p.PerfilSkills)
                .ThenInclude(ps => ps.Skill)
            .AsNoTracking()
            .ToListAsync();

    public async Task<List<Perfil>> GetByOwnerAsync(int userId)
        => await _context.Perfis
            .Include(p => p.Experiencias)
            .Include(p => p.PerfilSkills)
                .ThenInclude(ps => ps.Skill)
            .AsNoTracking()
            .Where(p => p.OwnerId == userId)
            .ToListAsync();

    public async Task<List<Perfil>> GetPublicAsync()
        => await _context.Perfis
            .Include(p => p.Experiencias)
            .Include(p => p.PerfilSkills)
                .ThenInclude(ps => ps.Skill)
            .AsNoTracking()
            .Where(p => p.IsShared)
            .ToListAsync();

    // Lista apenas os perfis visíveis para um utilizador (os seus + os públicos)
    public async Task<List<Perfil>> GetVisibleForUserAsync(int userId)
        => await _context.Perfis
            .Include(p => p.Experiencias)
            .Include(p => p.PerfilSkills)
                .ThenInclude(ps => ps.Skill)
            .AsNoTracking()
            .Where(p => p.OwnerId == userId || p.IsShared)
            .ToListAsync();

    // Cria um novo Perfil (com experiências e skills em cascata)
    public async Task AddAsync(Perfil perfil)
    {
        await _context.Perfis.AddAsync(perfil);
    }

    // Atualiza um perfil existente, apagando e recriando experiências e skills
    public async Task UpdateAsync(Perfil perfil)
    {
        // Remover experiências antigas e associações de skills antes de gravar as novas
        var experienciasAntigas = await _context.ExperienciasProfissionais
            .Where(e => e.PerfilId == perfil.Id)
            .ToListAsync();
        _context.ExperienciasProfissionais.RemoveRange(experienciasAntigas);

        var skillsAntigas = await _context.PerfilSkills
            .Where(ps => ps.PerfilId == perfil.Id)
            .ToListAsync();
        _context.PerfilSkills.RemoveRange(skillsAntigas);

        _context.Perfis.Update(perfil);
        await _context.SaveChangesAsync();
    }

    // Apaga o perfil (as experiências e skills são removidas em cascata pela BD)
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