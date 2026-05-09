using GestaoTalentos.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoTalentos.Infrastructure;

public interface IPerfilRepository
{
    Task<Perfil?> GetByIdAsync(int id);
    Task<IEnumerable<Perfil>> GetAllAsync();
    Task<List<Perfil>> GetByOwnerAsync(int userId); 
    Task<List<Perfil>> GetPublicAsync();               
    Task<List<Perfil>> GetVisibleForUserAsync(int userId);
    Task AddAsync(Perfil perfil);
    Task UpdateAsync(Perfil perfil);
    Task DeleteAsync(int id);
    Task SaveChangesAsync();

    // Pesquisa perfis que tenham uma combinação de skills (AND ou OR), ordenados por nome
    Task<List<Perfil>> SearchBySkillsAsync(IEnumerable<int> skillIds, bool todasAsSkills);

    // Devolve os IDs dos perfis que foram apresentados a um determinado cliente
    Task<HashSet<int>> GetPerfilIdsApresentadosAoClienteAsync(int clienteId);
}