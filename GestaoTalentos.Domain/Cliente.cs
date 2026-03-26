namespace GestaoTalentos.Domain;

/// Classe que representa um cliente no domínio da aplicação.
/// Um cliente possui informações básicas como nome, email, e referências ao criador e conta associada.
public class Cliente
{

    /// Identificador único do cliente (chave primária).
    public int Id { get; set; }

    /// Nome do cliente.
    public string Nome { get; set; } = string.Empty;
    
    /// Endereço de email do cliente.
    public string Email { get; set; } = string.Empty;
    
    /// ID do usuário que criou este cliente.
    public int IdCriador { get; set; }
    
    /// ID da conta associada ao cliente (pode ser nulo).
    public int? IdMinhaConta { get; set; }
}
