using System.Collections.Generic;
using System.Threading.Tasks;

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

public interface IPerfilRepository
{
    Task<Perfil?> GetByIdAsync(int id);
    Task<List<Perfil>> GetAllAsync();
    Task<List<Perfil>> GetByOwnerAsync(int userId);
    Task<List<Perfil>> GetPublicAsync();
    Task<List<Perfil>> GetVisibleForUserAsync(int userId);
    Task AddAsync(Perfil perfil);
    Task UpdateAsync(Perfil perfil);
    Task DeleteAsync(int id);
}