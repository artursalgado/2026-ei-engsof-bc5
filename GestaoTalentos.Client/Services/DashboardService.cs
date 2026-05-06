using System.Net.Http.Json;

namespace GestaoTalentos.Client.Services;

public class DashboardDto
{
    public int TotalPerfis { get; set; }
    public int TotalPropostas { get; set; }
    public decimal ValorTotalEstimadoPropostas { get; set; }
    public int PerfisUltimasQuatroSemanas { get; set; }
    public int TotalTalentosElegiveis { get; set; }
    public double TaxaAprovacao { get; set; }
    public List<PropostaDashboardDto> PropostasDetalhadas { get; set; } = new();
}

public class PropostaDashboardDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public int NumeroTotalHoras { get; set; }
    public decimal PrecoHoraMedio { get; set; }
    public decimal ValorEstimado { get; set; }
    public double MesesEquivalentes { get; set; }
    public double ValorMensalEquivalente { get; set; }
}

public interface IDashboardService
{
    Task<DashboardDto?> GetDashboardAsync();
}

public class DashboardService : IDashboardService
{
    private readonly HttpClient _httpClient;

    public DashboardService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<DashboardDto?> GetDashboardAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<DashboardDto>("dashboard");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao obter dashboard: {ex.Message}");
            return null;
        }
    }
}
