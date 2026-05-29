namespace GestaoTalentos.Domain;

/// Estado de aprovação de uma conta de utilizador.
/// Pendente: aguarda aprovação do Admin.
/// Ativo: conta aprovada, pode autenticar.
/// Rejeitado: conta rejeitada pelo Admin.
public enum EstadoConta
{
    Pendente = 0,
    Ativo = 1,
    Rejeitado = 2
}

/// Classe que representa um utilizador no domínio da aplicação.
/// Um utilizador possui ID, nome de utilizador, pass, role e estado de conta.
public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    /// Estado de aprovação da conta. Contas criadas pelo Admin ficam Ativo.
    /// Contas auto-registadas ficam Pendente até aprovação.
    public EstadoConta EstadoConta { get; set; } = EstadoConta.Ativo;
}
