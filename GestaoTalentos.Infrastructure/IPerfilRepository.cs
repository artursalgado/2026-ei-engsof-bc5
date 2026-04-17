using GestaoTalentos.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoTalentos.Infrastructure;

public interface IPerfilRepository
{
	Task<IEnumerable<Perfil>> GetAllAsync();
	Task<Perfil?> GetByIdAsync(int id);
	Task AddAsync(Perfil perfil);
	Task UpdateAsync(Perfil perfil);
	Task DeleteAsync(int id);
	Task SaveChangesAsync();
}