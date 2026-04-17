using GestaoTalentos.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoTalentos.API.Services;

public interface IPerfilService
{
    Task<IEnumerable<Perfil>> GetAllPerfisAsync();
    Task<Perfil?> GetPerfilByIdAsync(int id);
    Task CreatePerfilAsync(Perfil perfil);
    Task UpdatePerfilAsync(Perfil perfil);
    Task DeletePerfilAsync(int id);
}