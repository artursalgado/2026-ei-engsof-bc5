namespace GestaoTalentos.Domain;

/// Token único para partilhar uma proposta com um cliente externo.
/// O link público não requer autenticação e expira após a data definida.
public class PartilhaToken
{
    public int Id { get; set; }

    /// Token aleatório que compõe o URL público (/partilha/{token}).
    public string Token { get; set; } = string.Empty;

    public int PropostaId { get; set; }

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    /// Data de expiração. Nulo = sem expiração.
    public DateTime? ExpiraEm { get; set; }

    public Proposta? Proposta { get; set; }
}
