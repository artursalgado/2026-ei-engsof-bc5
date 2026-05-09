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

    public async Task<List<Perfil>> SearchBySkillsAsync(IEnumerable<int> skillIds, bool todasAsSkills)
    {
        var ids = (skillIds ?? Enumerable.Empty<int>()).Distinct().ToList();

        if (ids.Count == 0)
            return new List<Perfil>();

        var query = _context.Perfis
            .Include(p => p.Experiencias)
            .Include(p => p.PerfilSkills)
            .ThenInclude(ps => ps.Skill)
            .AsNoTracking()
            .AsQueryable();

        if (todasAsSkills)
        {
            // AND: o perfil tem de conter TODAS as skills indicadas
            query = query.Where(p =>
                ids.All(skillId => p.PerfilSkills.Any(ps => ps.SkillId == skillId)));
        }
        else
        {
            // OR: pelo menos uma das skills
            query = query.Where(p =>
                p.PerfilSkills.Any(ps => ids.Contains(ps.SkillId)));
        }

        return await query
            .OrderBy(p => p.Nome)
            .ToListAsync();
    }

    public async Task<HashSet<int>> GetPerfilIdsApresentadosAoClienteAsync(int clienteId)
    {
        // Ajusta os nomes (Apresentacoes / ClienteId / PerfilId) caso a tua entidade use outros
        var ids = await _context.Apresentacoes
            .Where(a => a.ClienteId == clienteId)
            .Select(a => a.PerfilId)
            .Distinct()
            .ToListAsync();

        return ids.ToHashSet();
    }
}