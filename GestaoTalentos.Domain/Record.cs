namespace GestaoTalentos.Domain;

/// Classe que representa um registro no domínio da aplicação.
/// Um registro possui conteúdo, proprietário, status de compartilhamento e data de criação.
public class Record
{

    /// Identificador único do registro (chave primária).
    public int Id { get; set; }
    
    /// ID do utilizador proprietário do registro.
    public int OwnerId { get; set; }
    
    /// Conteúdo textual do registro.
    public string Content { get; set; } = string.Empty;
    
    /// Indica se o registro é compartilhado com outros utilizadores.
    public bool IsShared { get; set; }
    
    /// Data e hora de criação do registro (em UTC).
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}