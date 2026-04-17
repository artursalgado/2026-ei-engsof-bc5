using GestaoTalentos.Domain;

namespace GestaoTalentos.Infrastructure;

public interface ITalentoElegivelRepository : IRepository<TalentoElegivel>
{
    Task<IEnumerable<TalentoElegivel>> GetByPropostaIdAsync(int propostaId);
    Task<IEnumerable<TalentoElegivel>> GetByPropostaIdOrderedByValorAsync(int propostaId);
    Task DeleteByPropostaIdAsync(int propostaId);
}