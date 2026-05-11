namespace GestaoTalentos.Shared.DTOs;

public class PropostaDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int AreaId { get; set; }
    public string NomeArea { get; set; } = string.Empty;
    public string DescricaoTrabalho { get; set; } = string.Empty;
    public int NumeroTotalHoras { get; set; }
    public decimal PrecoHoraMedio { get; set; }
    public decimal ValorEstimadoTotal => NumeroTotalHoras * PrecoHoraMedio;
    public DateTime CriadoEm { get; set; }
    public DateTime AtualizadoEm { get; set; }
}