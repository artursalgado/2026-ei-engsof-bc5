namespace GestaoTalentos.Domain;

/// Interface para repositório de utilizadores, definindo operações CRUD básicas.
public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByIdAsync(int id);
    
    Task<List<User>> GetAllAsync();
    Task<IEnumerable<User>> GetByRoleIdAsync(int roleId);
    
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(int id);
    
    Task<bool> AnyAsync();
}

/// Interface para repositório de perfis, definindo operações CRUD e específicas.
public interface IPerfilRepository
{
    Task<Perfil?> GetByIdAsync(int id);
    
    Task<List<Perfil>> GetAllAsync();
    Task<List<Perfil>> GetVisibleForUserAsync(int userId);
    Task<IEnumerable<Perfil>> GetByPaisIdAsync(int paisId);
    
    Task AddAsync(Perfil perfil);
    Task UpdateAsync(Perfil perfil);
    Task DeleteAsync(int id);
}