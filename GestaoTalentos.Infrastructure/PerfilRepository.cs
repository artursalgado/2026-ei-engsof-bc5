using GestaoTalentos.Domain;
using Microsoft.EntityFrameworkCore;

namespace GestaoTalentos.Infrastructure;

// Repositório completo de Perfis com suporte a Experiências e Skills
public class PerfilRepository : IPerfilRepository
{
    private readonly AppDbContext _context;
    private DbSet<Perfil> Perfis => _context.Set<Perfil>();

    public PerfilRepository(AppDbContext context) => _context = context;

    // Busca simples por ID (com tudo relacionado incluído)
    public async Task<Perfil?> GetByIdAsync(int id)
        => await Perfis
            .Include(p => p.Experiencias)
            .Include(p => p.PerfilSkills)
                .ThenInclude(ps => ps.Skill)
            .FirstOrDefaultAsync(p => p.Id == id);

    // Lista todos os perfis com experiências e skills
    public async Task<List<Perfil>> GetAllAsync()
        => await Perfis
            .Include(p => p.Experiencias)
            .Include(p => p.PerfilSkills)
                .ThenInclude(ps => ps.Skill)
            .AsNoTracking()
            .ToListAsync();

    public async Task<List<Perfil>> GetByOwnerAsync(int userId)
        => await Perfis
            .Include(p => p.Experiencias)
            .Include(p => p.PerfilSkills)
                .ThenInclude(ps => ps.Skill)
            .AsNoTracking()
            .Where(p => p.OwnerId == userId)
            .ToListAsync();

    public async Task<List<Perfil>> GetPublicAsync()
        => await Perfis
            .Include(p => p.Experiencias)
            .Include(p => p.PerfilSkills)
                .ThenInclude(ps => ps.Skill)
            .AsNoTracking()
            .Where(p => p.IsShared)
            .ToListAsync();

    // Lista apenas os perfis visíveis para um utilizador (os seus + os públicos)
    public async Task<List<Perfil>> GetVisibleForUserAsync(int userId)
        => await Perfis
            .Include(p => p.Experiencias)
            .Include(p => p.PerfilSkills)
                .ThenInclude(ps => ps.Skill)
            .AsNoTracking()
            .Where(p => p.OwnerId == userId || p.IsShared)
            .ToListAsync();

    // Cria um novo Perfil (com experiências e skills em cascata)
    public async Task AddAsync(Perfil perfil)
    {
        await Perfis.AddAsync(perfil);
        await _context.SaveChangesAsync();
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

        Perfis.Update(perfil);
        await _context.SaveChangesAsync();
    }

    // Apaga o perfil (as experiências e skills são removidas em cascata pela BD)
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