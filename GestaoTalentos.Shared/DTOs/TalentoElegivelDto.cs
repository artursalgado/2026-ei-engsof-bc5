namespace GestaoTalentos.Shared.DTOs;

public class TalentoElegivelDto
{
    public int Id { get; set; }
    public int PerfilId { get; set; }
    public decimal ValorEstimado { get; set; }
    public string NomePerfil { get; set; } = string.Empty; // Info do talento
}