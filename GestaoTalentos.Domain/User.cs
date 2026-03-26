namespace GestaoTalentos.Domain;

/// Classe que representa um utilizador no domínio da aplicação.
/// Um utilizador possui ID, nome de utilizador, senha e role.
public class User
{

    /// Identificador único do utilizador (chave primária).
    public int Id { get; set; }
    
    /// Nome de usuário único para login.
    public string Username { get; set; } = string.Empty;
    
    /// Senha criptografada do usuário.
    public string Password { get; set; } = string.Empty;
    
    /// Role do usuário, definindo permissões.
    public UserRole Role { get; set; }
}

/// Enumeração para os roles de usuário, definindo níveis de permissão.
public enum UserRole
{

    /// Utilizador comum, com permissões básicas.
    User,
    
    /// Gerenciador de utilizadores, pode gerenciar utilizadores e roles.
    UserManager,
    
    /// Administrador, com permissões totais.
    Admin
}
