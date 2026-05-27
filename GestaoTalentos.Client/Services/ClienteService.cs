using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using GestaoTalentos.Shared.DTOs;

namespace GestaoTalentos.Client.Services;

public interface IClienteService
{
    Task<List<ClienteDto>> GetAllClientesAsync();
    Task<ClienteDto?> AddClienteAsync(ClienteCreateDto dto);
    Task<bool> UpdateClienteAsync(int id, ClienteUpdateDto dto);
    Task<bool> DeleteClienteAsync(int id);
}

public class ClienteService : IClienteService
{
    private readonly HttpClient _httpClient;

    public ClienteService(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<List<ClienteDto>> GetAllClientesAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<ClienteDto>>("clientes") ?? new();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao obter clientes: {ex.Message}");
            return new();
        }
    }

    public async Task<ClienteDto?> AddClienteAsync(ClienteCreateDto dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("clientes", dto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ClienteDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao criar cliente: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> UpdateClienteAsync(int id, ClienteUpdateDto dto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"clientes/{id}", dto);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar cliente: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteClienteAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"clientes/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao apagar cliente: {ex.Message}");
            return false;
        }
    }
}