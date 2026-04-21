using GestaoTalentos.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoTalentos.API.Services;

public interface ITalentoElegiveService
{
    Task<IEnumerable<TalentoElegivel>> GetAllTalentosElegiveisAsync();
    Task<TalentoElegivel?> GetTalentoElegiveByIdAsync(int id);
    Task CreateTalentoElegiveAsync(TalentoElegivel talentoElegivel);
    Task UpdateTalentoElegiveAsync(TalentoElegivel talentoElegivel);
    Task DeleteTalentoElegiveAsync(int id);
}