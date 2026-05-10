namespace GestaoTalentos.Domain;

/// Entidade Pais — não tem tabela na BD.
/// Nos perfis, o país é guardado como string directa.
public class Pais
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
}
