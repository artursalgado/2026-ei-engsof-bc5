namespace GestaoTalentos.Infrastructure;

using GestaoTalentos.Domain;



/// Interface para repositório de areas, definindo operações CRUD e específicas.
public interface IAreaRepository
{
    Task<Area?> GetByNomeAsync(string nome);
    Task<List<Area>> GetAllAsync();
    Task<IEnumerable<Area>> GetAllWithSkilsAsync();
    
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
    Task<IEnumerable<Pais>> GetAllWithPerfisAsync();
    
    Task AddAsync(Pais pais);
    Task DeleteAsync(string nome);
}