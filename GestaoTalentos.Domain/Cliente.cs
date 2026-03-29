namespace GestaoTalentos.Domain;

/// Classe que representa um cliente no domínio da aplicação.
/// Um cliente possui informações básicas como nome, email, e referências ao criador e conta associada.
public class Cliente
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty; 
    public int IdCriador { get; set; } 
    public int? IdMinhaConta { get; set; } 
    
    public User User { get; set; } = null!;
    
    /// /// Lista de propostas associadas ao cliente.
    // public List<Proposta> PropostasCriadas { get; set; } = new();
    //public List<Proposta> PropostasRecebidas { get; set; } = new();

    // Talentos apresentados
    //public List<Apresentacao> Apresentacoes { get; set; } = new();
    
}
