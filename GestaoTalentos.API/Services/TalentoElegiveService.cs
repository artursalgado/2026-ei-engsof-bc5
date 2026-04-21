using GestaoTalentos.Domain;
using GestaoTalentos.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoTalentos.API.Services;

public class TalentoElegiveService : ITalentoElegiveService
{
    private readonly ITalentoElegivelRepository _repository;

    public TalentoElegiveService(ITalentoElegivelRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TalentoElegivel>> GetAllTalentosElegiveisAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<TalentoElegivel?> GetTalentoElegiveByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task CreateTalentoElegiveAsync(TalentoElegivel talentoElegivel)
    {
        talentoElegivel.CriadoEm = DateTime.UtcNow;
        await _repository.AddAsync(talentoElegivel);
        await _repository.SaveChangesAsync();
    }

    public async Task UpdateTalentoElegiveAsync(TalentoElegivel talentoElegivel)
    {
        await _repository.UpdateAsync(talentoElegivel);
        await _repository.SaveChangesAsync();
    }

    public async Task DeleteTalentoElegiveAsync(int id)
    {
        await _repository.DeleteAsync(id);
        await _repository.SaveChangesAsync();
    }
}