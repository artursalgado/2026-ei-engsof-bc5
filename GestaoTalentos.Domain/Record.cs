namespace GestaoTalentos.Domain;

/// Classe que representa um registro no domínio da aplicação.
/// Um registro possui conteúdo, proprietário, status de compartilhamento e data de criação.
public class Record
{
    public int Id { get; set; }
    public int OwnerId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// Indica se o registro é compartilhado com outros utilizadores.
    public bool IsShared { get; set; }
    
}