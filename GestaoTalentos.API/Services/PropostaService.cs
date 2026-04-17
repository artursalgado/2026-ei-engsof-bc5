using GestaoTalentos.Domain;
using GestaoTalentos.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoTalentos.API.Services;

public class PropostaService : IPropostaService
{
    private readonly IPropostaRepository _repository;

    public PropostaService(IPropostaRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Proposta>> GetAllPropostasAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Proposta?> GetPropostaByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task CreatePropostaAsync(Proposta proposta)
    {
        proposta.CriadoEm = DateTime.UtcNow;
        proposta.AtualizadoEm = DateTime.UtcNow;
        await _repository.AddAsync(proposta);
        await _repository.SaveChangesAsync();
    }

    public async Task UpdatePropostaAsync(Proposta proposta)
    {
        proposta.AtualizadoEm = DateTime.UtcNow;
        await _repository.UpdateAsync(proposta);
        await _repository.SaveChangesAsync();
    }

    public async Task DeletePropostaAsync(int id)
    {
        await _repository.DeleteAsync(id);
        await _repository.SaveChangesAsync();
    }
}