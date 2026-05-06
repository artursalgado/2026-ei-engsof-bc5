using GestaoTalentos.Domain;
using GestaoTalentos.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoTalentos.API.Services;

public class PropostaService : IPropostaService
{
    private readonly IPropostaRepository _propostaRepository;

    public PropostaService(IPropostaRepository propostaRepository)
    {
        _propostaRepository = propostaRepository;
    }

    public async Task<IEnumerable<Proposta>> GetAllPropostasAsync()
    {
        return await _propostaRepository.GetAllWithSkillsAsync();
    }

    public async Task<Proposta?> GetPropostaByIdAsync(int id)
    {
        return await _propostaRepository.GetByIdWithSkillsAsync(id);
    }

    public async Task CreatePropostaAsync(Proposta proposta)
    {
        await _propostaRepository.AddAsync(proposta);
    }

    public async Task UpdatePropostaAsync(Proposta proposta)
    {
        await _propostaRepository.UpdateAsync(proposta);
    }

    public async Task DeletePropostaAsync(int id)
    {
        await _propostaRepository.DeleteAsync(id);
    }
}