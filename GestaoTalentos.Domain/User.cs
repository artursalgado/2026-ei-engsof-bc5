namespace GestaoTalentos.Domain;

/// Classe que representa um utilizador no domínio da aplicação.
/// Um utilizador possui ID, nome de utilizador, pass e role.
public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public int RoleId { get; set; }
    
    // Relacionamentos
    public Role? Role { get; set; }
    
    //public UserRole Role { get; set; }
}

/*public enum TipoUtilizador
{
    Talento,
    Cliente
}
