namespace GestaoTalentos.Domain;

/// Interface para repositório de utilizadores, definindo operações CRUD básicas.
public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByIdAsync(int id);
    
    Task<List<User>> GetAllAsync();
    
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(int id);
    
    Task<bool> AnyAsync();
}

/// Interface para repositório de registros, definindo operações CRUD e específicas.
public interface IRecordRepository
{
    
    Task<Record?> GetByIdAsync(int id);
    
    Task<List<Record>> GetAllAsync();
    Task<List<Record>> GetVisibleForUserAsync(int userId);
    
    Task AddAsync(Record record);
    Task UpdateAsync(Record record);
    Task DeleteAsync(int id);
}