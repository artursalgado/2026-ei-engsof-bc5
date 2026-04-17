using GestaoTalentos.Domain;

namespace GestaoTalentos.Infrastructure;

public interface IPropostaRepository : IRepository<Proposta>
{
    Task<Proposta?> GetByIdWithSkillsAsync(int id);
    Task<IEnumerable<Proposta>> GetAllWithSkillsAsync();
    Task<Proposta?> GetByNomeAsync(string nome);
    Task<IEnumerable<Proposta>> GetByAreaIdAsync(int areaId);
}