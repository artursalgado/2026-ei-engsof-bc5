namespace GestaoTalentos.Domain;

/// Interface para repositório de clientes, definindo operações CRUD básicas.
public interface IClienteRepository
{

    /// Obtém um cliente pelo nome.
    /// <param name="nome">Nome do cliente.</param>
    /// <returns>O cliente encontrado ou null se não existir.</returns>
    Task<Cliente?> GetByNomeAsync(string nome);
    
    /// Obtém um cliente pelo ID.
    /// <param name="id">ID do cliente.</param>
    /// <returns>O cliente encontrado ou null se não existir.</returns>
    Task<Cliente?> GetByIdAsync(int id);
    
    /// Obtém todos os clientes.
    /// <returns>Lista de todos os clientes.</returns>
    Task<List<Cliente>> GetAllAsync();
    
    /// Adiciona um novo cliente.
    /// <param name="cliente">Instância do cliente a ser adicionado.</param>
    Task AddAsync(Cliente cliente);
    
    /// Atualiza um cliente existente.
    /// <param name="cliente">Instância do cliente a ser atualizado.</param>
    Task UpdateAsync(Cliente cliente);
    
    /// Verifica se existe pelo menos um cliente.
    /// <returns>True se existir pelo menos um cliente, false caso contrário.</returns>
    Task<bool> AnyAsync();
}
