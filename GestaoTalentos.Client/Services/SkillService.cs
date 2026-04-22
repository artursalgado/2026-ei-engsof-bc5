using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using GestaoTalentos.Shared.DTOs;

namespace GestaoTalentos.Client.Services;

public interface ISkillService
{
    Task<List<SkillDto>> GetAllSkillsAsync();
    Task<SkillDto?> GetSkillByIdAsync(int id);

    Task<List<SkillDto>> GetSkillsByAreaAsync(int areaId);

    Task<SkillDto?> CreateSkillAsync(CreateSkillDto skillDto);

  
    Task<bool> UpdateSkillAsync(int id, UpdateSkillDto skillDto);

    Task<bool> DeleteSkillAsync(int id);
}

public class SkillService : ISkillService
{
    private readonly HttpClient _httpClient;

    public SkillService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<SkillDto>> GetAllSkillsAsync()
    {
        try
        {
            // API: GET /skills
            return await _httpClient.GetFromJsonAsync<List<SkillDto>>("skills") ?? new List<SkillDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao obter skills: {ex.Message}");
            return new List<SkillDto>();
        }
    }

    public async Task<SkillDto?> GetSkillByIdAsync(int id)
    {
        try
        {
            var all = await GetAllSkillsAsync();
            return all.FirstOrDefault(s => s.Id == id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao obter skill: {ex.Message}");
            return null;
        }
    }

    public async Task<List<SkillDto>> GetSkillsByAreaAsync(int areaId)
    {
        try
        {
            
            var all = await GetAllSkillsAsync();
            return all.Where(s => s.AreaId == areaId).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao obter skills por área: {ex.Message}");
            return new List<SkillDto>();
        }
    }

    public async Task<SkillDto?> CreateSkillAsync(CreateSkillDto skillDto)
    {
        try
        {
            // API: POST /skills (RequireAuthorization UserManagerPolicy)
            var response = await _httpClient.PostAsJsonAsync("skills", skillDto);

            
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Erro ao criar skill. Status={(int)response.StatusCode} {response.StatusCode}. Body={body}");
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<SkillDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao criar skill: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> UpdateSkillAsync(int id, UpdateSkillDto skillDto)
    {
        try
        {
            // API: PUT /skills/{id}
            var response = await _httpClient.PutAsJsonAsync($"skills/{id}", skillDto);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Erro ao atualizar skill. Status={(int)response.StatusCode} {response.StatusCode}. Body={body}");
            }

            return response.IsSuccessStatusCode; // deve ser 204 NoContent quando OK
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar skill: {ex.Message}");
            return false;
        }
    }


    public async Task<bool> DeleteSkillAsync(int id)
    {
        try
        {
            // API: DELETE /skills/{id}
            var response = await _httpClient.DeleteAsync($"skills/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao deletar skill: {ex.Message}");
            return false;
        }
    }
}