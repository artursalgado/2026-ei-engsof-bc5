using GestaoTalentos.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoTalentos.Infrastructure;

public interface ITalentoElegivelRepository
{
    Task<IEnumerable<TalentoElegivel>> GetAllAsync();
    Task<TalentoElegivel?> GetByIdAsync(int id);
    Task AddAsync(TalentoElegivel talentoElegivel);
    Task UpdateAsync(TalentoElegivel talentoElegivel);
    Task DeleteAsync(int id);
    Task SaveChangesAsync();
    Task<IEnumerable<TalentoElegivel>> GetByPropostaIdAsync(int propostaId);
    Task<IEnumerable<TalentoElegivel>> GetByPropostaIdOrderedByValorAsync(int propostaId);
    Task DeleteByPropostaIdAsync(int propostaId);

    public async Task<IEnumerable<TalentoElegivel>> GetByPropostaIdOrderedByValorAsync(int propostaId)
    {
        return await _context.TalentosElegiveis
            .Include(te => te.Perfil)
            .Where(te => te.PropostaId == propostaId)
            .OrderBy(te => te.ValorEstimado)
            .ToListAsync();
    }
}