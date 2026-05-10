namespace GestaoTalentos.Domain;

/// Entidade Role — usada apenas como DTO de domínio.
/// Não tem tabela na BD (roles são geridos via enum UserRole no User).
public class Role
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
}
