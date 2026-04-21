using GestaoTalentos.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoTalentos.Infrastructure;

public interface IPerfilRepository
{
    Task<Perfil?> GetByIdAsync(int id);
    Task<IEnumerable<Perfil>> GetAllAsync();
    Task<List<Perfil>> GetVisibleForUserAsync(int userId);
    Task AddAsync(Perfil perfil);
    Task UpdateAsync(Perfil perfil);
    Task DeleteAsync(int id);
    Task SaveChangesAsync();
}