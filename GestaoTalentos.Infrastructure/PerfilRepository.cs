using GestaoTalentos.Domain;
using Microsoft.EntityFrameworkCore;

namespace GestaoTalentos.Infrastructure;

public class PerfilRepository : IPerfilRepository
{
    private readonly AppDbContext _context;
    private DbSet<Perfil> Perfis => _context.Set<Perfil>();

    public PerfilRepository(AppDbContext context)
    {
        _context = context;
    }

    // GET BY ID
    public async Task<Perfil?> GetByIdAsync(int id)
        => await Perfis
            .Include(p => p.Experiencias)
            .Include(p => p.PerfilSkills)
                .ThenInclude(ps => ps.Skill)
            .FirstOrDefaultAsync(p => p.Id == id);

    // GET ALL
    public async Task<List<Perfil>> GetAllAsync()
        => await Perfis
            .Include(p => p.Experiencias)
            .Include(p => p.PerfilSkills)
                .ThenInclude(ps => ps.Skill)
            .AsNoTracking()
            .ToListAsync();

    // BY OWNER
    public async Task<List<Perfil>> GetByOwnerAsync(int userId)
        => await Perfis
            .Include(p => p.Experiencias)
            .Include(p => p.PerfilSkills)
                .ThenInclude(ps => ps.Skill)
            .AsNoTracking()
            .Where(p => p.OwnerId == userId)
            .ToListAsync();

    // PUBLIC
    public async Task<List<Perfil>> GetPublicAsync()
        => await Perfis
            .Include(p => p.Experiencias)
            .Include(p => p.PerfilSkills)
                .ThenInclude(ps => ps.Skill)
            .AsNoTracking()
            .Where(p => p.IsShared)
            .ToListAsync();

    // VISÍVEIS PARA UTILIZADOR
    public async Task<IEnumerable<Perfil>> GetVisibleForUserAsync(int userId)
        => await Perfis
            .AsNoTracking()
            .Where(p => p.OwnerId == userId || p.IsShared)
            .ToListAsync();

    // POR PAÍS
    public async Task<IEnumerable<Perfil>> GetByPaisIdAsync(int paisId)
        => await Perfis
            .Include(p => p.Experiencias)
            .Include(p => p.PerfilSkills)
                .ThenInclude(ps => ps.Skill)
            .AsNoTracking()
            .Where(p => p.PaisId == paisId)
            .OrderBy(p => p.Id)
            .ToListAsync();

    // CREATE
    public async Task AddAsync(Perfil perfil)
    {
        await Perfis.AddAsync(perfil);
        await _context.SaveChangesAsync();
    }

    // UPDATE
    public async Task UpdateAsync(Perfil perfil)
    {
        var experienciasAntigas = await _context.ExperienciasProfissionais
            .Where(e => e.PerfilId == perfil.Id)
            .ToListAsync();

        _context.ExperienciasProfissionais.RemoveRange(experienciasAntigas);

        var skillsAntigas = await _context.PerfilSkills
            .Where(ps => ps.PerfilId == perfil.Id)
            .ToListAsync();

        _context.PerfilSkills.RemoveRange(skillsAntigas);

        _context.Update(perfil);

        await _context.SaveChangesAsync();
    }

    // DELETE
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