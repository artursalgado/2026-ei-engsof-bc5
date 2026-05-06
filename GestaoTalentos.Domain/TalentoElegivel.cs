using System;

namespace GestaoTalentos.Domain;

public class TalentoElegivel
{
    public int Id { get; set; }
    
    public int PropostaId { get; set; }
    public int PerfilId { get; set; } // Referência ao talento/perfil
    
    public decimal ValorEstimado { get; set; } // Preço/Hora x NumeroTotalHoras
    
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public Proposta? Proposta { get; set; }
    public Perfil? Perfil { get; set; }
}