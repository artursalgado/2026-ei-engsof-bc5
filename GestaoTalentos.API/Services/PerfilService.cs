using GestaoTalentos.Domain;
using GestaoTalentos.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoTalentos.API.Services;

public class PerfilService : IPerfilService
{
    private readonly IPerfilRepository _repository;

    public PerfilService(IPerfilRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Perfil>> GetAllPerfisAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Perfil?> GetPerfilByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task CreatePerfilAsync(Perfil perfil)
    {
        perfil.CreatedAt = DateTime.UtcNow;
        await _repository.AddAsync(perfil);
        await _repository.SaveChangesAsync();
    }

    public async Task UpdatePerfilAsync(Perfil perfil)
    {
        await _repository.UpdateAsync(perfil);
        await _repository.SaveChangesAsync();
    }

    public async Task DeletePerfilAsync(int id)
    {
        await _repository.DeleteAsync(id);
        await _repository.SaveChangesAsync();
    }
}