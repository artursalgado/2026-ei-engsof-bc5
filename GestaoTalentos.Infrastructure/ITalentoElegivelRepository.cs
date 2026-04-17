using GestaoTalentos.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoTalentos.Infrastructure;

public interface ITalentoElegiveRepository
{
    Task<IEnumerable<TalentoElegivel>> GetAllAsync();
    Task<TalentoElegivel?> GetByIdAsync(int id);
    Task AddAsync(TalentoElegivel talentoElegivel);
    Task UpdateAsync(TalentoElegivel talentoElegivel);
    Task DeleteAsync(int id);
    Task SaveChangesAsync();
}