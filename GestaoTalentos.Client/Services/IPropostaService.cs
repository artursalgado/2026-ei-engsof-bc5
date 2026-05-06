using GestaoTalentos.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace GestaoTalentos.Client.Services;

public interface IPropostaService
{
    Task<List<PropostaDto>> GetAllPropostasAsync();
    Task<PropostaDto?> GetPropostaByIdAsync(int id);
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

    public async Task<List<PropostaDto>> GetAllPropostasAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<PropostaDto>>("propostas") ?? new List<PropostaDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao obter propostas: {ex.Message}");
            return new List<PropostaDto>();
        }
    }

    public async Task<PropostaDto?> GetPropostaByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<PropostaDto>($"propostas/{id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao obter proposta: {ex.Message}");
            return null;
        }
    }

    public async Task<HttpResponseMessage> CreateAsync(CreatePropostaDto dto)
    {
        try
        {
            return await _httpClient.PostAsJsonAsync("propostas", dto);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao criar proposta: {ex.Message}");
            throw;
        }
    }

    public async Task<HttpResponseMessage> UpdateAsync(int id, UpdatePropostaDto dto)
    {
        try
        {
            return await _httpClient.PutAsJsonAsync($"propostas/{id}", dto);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar proposta: {ex.Message}");
            throw;
        }
    }

    public async Task<HttpResponseMessage> DeleteAsync(int id)
    {
        try
        {
            return await _httpClient.DeleteAsync($"propostas/{id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao deletar proposta: {ex.Message}");
            throw;
        }
    }
}