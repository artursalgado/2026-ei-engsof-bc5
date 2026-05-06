using GestaoTalentos.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoTalentos.Infrastructure;

public interface IPropostaRepository
{
    Task<IEnumerable<Proposta>> GetAllAsync();
    Task<IEnumerable<Proposta>> GetAllWithSkillsAsync();
    Task<Proposta?> GetByIdAsync(int id);
    Task<Proposta?> GetByIdWithSkillsAsync(int id);
    Task<Proposta?> GetByNomeAsync(string nome);
    Task AddAsync(Proposta proposta);
    Task UpdateAsync(Proposta proposta);
    Task DeleteAsync(int id);
    Task SaveChangesAsync();
}