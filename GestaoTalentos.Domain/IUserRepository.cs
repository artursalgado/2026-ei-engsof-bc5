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
