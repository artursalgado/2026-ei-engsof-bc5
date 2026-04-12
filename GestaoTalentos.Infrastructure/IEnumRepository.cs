namespace DefaultNamespace;
namespace GestaoTalentos.Infrastructure;

using GestaoTalentos.Domain;
/*public class EnumRepository : IAreaRepository
{
    private readonly AppDbContext _context;
    public EnumRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<Enum?> GetByIdAsync(int id)
    {
        return await _context.Enums
            .Include(a => a.[Holder])
            .FirstOrDefaultAsync(a => a.Id == id);
    }
    public async Task<IEnumerable<Enum>> GetAllAsync()
    {
        return await _context.Enums
            .OrderBy(a => a.Nome)
            .ToListAsync();
    }
    public async Task<IEnumerable<Enum>> GetAllWith[Holder]Async()
    {
        return await _context.Enums
            .Include(a => a.[Holder])
            .OrderBy(a => a.Nome)
            .ToListAsync();
    }
    public async Task<Enum?> GetByNomeAsync(string nome)
    {
        return await _context.Enums
            .FirstOrDefaultAsync(a => a.Nome.ToLower() == nome.ToLower());
    }
    public async Task AddAsync(Enum entity)
    {
        await _context.Enums.AddAsync(entity);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateAsync(Enum entity)
    {
        _context.Enums.Update(entity);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteAsync(int id)
    {
        var area = await GetByIdAsync(id);
        if (enum != null)
        {
            _context.Enums.Remove(enum);
            await _context.SaveChangesAsync();
        }
    }
    public async Task<bool> AnyAsync()
    {
        return await _context.Enums.AnyAsync();
    }
    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Enums.AnyAsync(a => a.Id == id);
    }
}*/


/// Interface para repositório de areas, definindo operações CRUD e específicas.
public interface IAreaRepository
{
    Task<Area?> GetByNomeAsync(string nome);
    Task<List<Area>> GetAllAsync();
    Task<IEnumerable<Area>> GetAllWithSkillsAsync();
    
    Task AddAsync(Area area);
    Task DeleteAsync(string nome);
}

/// Interface para repositório de roles, definindo operações CRUD e específicas.
public interface IRoleRepository
{
    Task<Role?> GetByNomeAsync(string nome);
    Task<List<Role>> GetAllAsync();
    Task<IEnumerable<Role>> GetAllWithUsersAsync();
    
    Task AddAsync(Role role);
    Task DeleteAsync(string nome);
}

/// Interface para repositório de paises, definindo operações CRUD e específicas.
public interface IPaisRepository
{
    Task<Pais?> GetByNomeAsync(string nome);
    Task<List<Pais>> GetAllAsync();
    Task<IEnumerable<Pais>> GetAllWithUsersAsync();
    
    Task AddAsync(Pais pais);
    Task DeleteAsync(string nome);
}