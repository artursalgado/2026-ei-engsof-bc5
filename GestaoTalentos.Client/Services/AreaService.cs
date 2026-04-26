using System.Net.Http.Json;
using GestaoTalentos.Shared.DTOs;

namespace GestaoTalentos.Client.Services;

public interface IAreaService
{
    Task<List<AreaDto>> GetAllAreasAsync();
    Task<AreaDto?> CreateAreaAsync(AreaCreateDto dto);
    Task<bool> UpdateAreaAsync(int id, AreaCreateDto dto);
    Task<(bool success, string? error)> DeleteAreaAsync(int id);
}

public class AreaService : IAreaService
{
    private readonly HttpClient _httpClient;

    public AreaService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<AreaDto>> GetAllAreasAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<AreaDto>>("areas") ?? new List<AreaDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao obter áreas: {ex.Message}");
            return new List<AreaDto>();
        }
    }

    public async Task<AreaDto?> CreateAreaAsync(AreaCreateDto dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("areas", dto);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new Exception(body.Trim('"'));
            }
            return await response.Content.ReadFromJsonAsync<AreaDto>();
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> UpdateAreaAsync(int id, AreaCreateDto dto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"areas/{id}", dto);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new Exception(body.Trim('"'));
            }
            return true;
        }
        catch
        {
            throw;
        }
    }

    public async Task<(bool success, string? error)> DeleteAreaAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"areas/{id}");
            if (response.IsSuccessStatusCode) return (true, null);
            var body = await response.Content.ReadAsStringAsync();
            return (false, body.Trim('"'));
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }
}
