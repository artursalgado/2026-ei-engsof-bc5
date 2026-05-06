using GestaoTalentos.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoTalentos.API.Services;

public interface IPropostaService
{
    Task<IEnumerable<Proposta>> GetAllPropostasAsync();
    Task<Proposta?> GetPropostaByIdAsync(int id);
    Task CreatePropostaAsync(Proposta proposta);
    Task UpdatePropostaAsync(Proposta proposta);
    Task DeletePropostaAsync(int id);
}