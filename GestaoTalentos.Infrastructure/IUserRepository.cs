namespace GestaoTalentos.Domain;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByIdAsync(int id);
    Task<List<User>> GetAllAsync();
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task<bool> AnyAsync();
}

public interface IRecordRepository
{
    Task<Record?> GetByIdAsync(int id);
    Task<List<Record>> GetAllAsync();
    Task<List<Record>> GetVisibleForUserAsync(int userId);
    Task AddAsync(Record record);
    Task UpdateAsync(Record record);
    Task DeleteAsync(int id);
}