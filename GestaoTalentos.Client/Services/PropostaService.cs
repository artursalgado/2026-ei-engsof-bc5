using GestaoTalentos.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace GestaoTalentos.Client.Services;

public interface IPropostaService
{
    Task<List<object>> GetAllAsync();
    Task<HttpResponseMessage> CreateAsync(CreatePropostaDto dto);
    Task<HttpResponseMessage> UpdateAsync(int id, UpdatePropostaDto dto);
    Task<HttpResponseMessage> DeleteAsync(int id);
}

public class PropostaService : IPropostaService
{
    private readonly HttpClient _httpClient;

    public PropostaService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<object>> GetAllAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<object>>("propostas") ?? new List<object>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao obter propostas: {ex.Message}");
            return new List<object>();
        }
    }

    public async Task<HttpResponseMessage> CreateAsync(CreatePropostaDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("propostas", dto);
        return response;
    }

    public async Task<HttpResponseMessage> UpdateAsync(int id, UpdatePropostaDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"propostas/{id}", dto);
        return response;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"propostas/{id}");
        return response.IsSuccessStatusCode;
    }
}