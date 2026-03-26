
using GestaoTalentos.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoTalentos.Infrastructure;

public interface ISkillRepository : IRepository<Skill>
{
    Task<Skill?> GetByNomeAsync(string nome);
    Task<IEnumerable<Skill>> GetByAreaIdAsync(int areaId);
    Task<IEnumerable<Skill>> GetAllWithAreaAsync();
}