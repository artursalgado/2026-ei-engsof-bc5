using GestaoTalentos.Domain;

namespace GestaoTalentos.Infrastructure;

public interface ISkillRepository : IRepository<Skill>
{
    Task<Skill?> GetByNomeAsync(string nome);
    Task<IEnumerable<Skill>> GetByAreaIdAsync(int areaId);
    Task<bool> IsSkillAssociatedToPropostaAsync(int skillId);
    Task<IEnumerable<Skill>> GetAllWithAreaAsync();
}