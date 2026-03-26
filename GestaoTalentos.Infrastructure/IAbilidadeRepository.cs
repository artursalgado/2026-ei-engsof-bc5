using System;
namespace GestaoTalentos.Infrastructure;

using GestaoTalentos.Domain;

public interface IAbilidadeRepository : IRepository<Abilidade>
{
    Task<IEnumerable<Abilidade>> GetBySkillIdAsync(int skillId);
    Task<Abilidade?> GetByNomeAndSkillIdAsync(string nome, int skillId);
}