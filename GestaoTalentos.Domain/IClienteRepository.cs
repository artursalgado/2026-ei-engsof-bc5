namespace GestaoTalentos.Domain;

/// Interface para repositório de clientes, definindo operações CRUD básicas.
public interface IClienteRepository
{
    Task<Cliente?> GetByNomeAsync(string nome);
    Task<Cliente?> GetByIdAsync(int id);
    Task<Cliente?> GetByEmailAsync(string email);

    
    Task<List<Cliente>> GetAllAsync();
    Task<List<Cliente>> GetByCriadorIdAsync(int criadorId);
    
    Task AddAsync(Cliente cliente);
    Task UpdateAsync(Cliente cliente);
    Task DeleteAsync(int id);  
    
    Task<bool> AnyAsync();
}
