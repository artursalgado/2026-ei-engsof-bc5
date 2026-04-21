using GestaoTalentos.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoTalentos.API.Services;

public interface ITalentoElegivelService
{
    Task<IEnumerable<TalentoElegivel>> GetAllTalentosElegiveisAsync();
    Task<TalentoElegivel?> GetTalentoElegivelByIdAsync(int id);
    Task CreateTalentoElegivelAsync(TalentoElegivel talentoElegivel);
    Task UpdateTalentoElegivelAsync(TalentoElegivel talentoElegivel);
    Task DeleteTalentoElegivelAsync(int id);
}