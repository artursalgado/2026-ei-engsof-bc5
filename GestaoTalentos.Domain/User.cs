namespace GestaoTalentos.Domain;

/// Classe que representa um utilizador no domínio da aplicação.
/// Um utilizador possui ID, nome de utilizador, pass e role.
public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    
    public List<Cliente> Clientes { get; set; } = new();
}

/// Enumeração para os roles de usuário, definindo níveis de permissão.
public enum UserRole
{
    User, /// Utilizador comum, com permissões básicas.
    UserManager, /// Gestor de utilizadores, pode gerir utilizadores e roles.
    Admin/// Administrador, com permissões totais.
}
