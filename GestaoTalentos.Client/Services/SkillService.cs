using System;
using System.Collections.Generic;
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
            return await _httpClient.GetFromJsonAsync<List<SkillDto>>("api/skills") ?? new List<SkillDto>();
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
            return await _httpClient.GetFromJsonAsync<SkillDto>($"api/skills/{id}");
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
            return await _httpClient.GetFromJsonAsync<List<SkillDto>>($"api/skills/area/{areaId}") ?? new List<SkillDto>();
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
            var response = await _httpClient.PostAsJsonAsync("api/skills", skillDto);
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
            var response = await _httpClient.PutAsJsonAsync($"api/skills/{id}", skillDto);
            return response.IsSuccessStatusCode;
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
            var response = await _httpClient.DeleteAsync($"api/skills/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao deletar skill: {ex.Message}");
            return false;
        }
    }
}