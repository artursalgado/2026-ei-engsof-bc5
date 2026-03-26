namespace GestaoTalentos.Domain;

/// Interface para repositório de utilizadores, definindo operações CRUD básicas.
public interface IUserRepository
{

    /// Obtém um usuário pelo nome de utilizador.
    /// <param name="username">Nome de utilizador.</param>
    /// <returns>O usuário encontrado ou null se não existir.</returns>
    Task<User?> GetByUsernameAsync(string username);
    
    /// Obtém um usuário pelo ID.
    /// <param name="id">ID do utilizador.</param>
    /// <returns>O utilizador encontrado ou null se não existir.</returns>
    Task<User?> GetByIdAsync(int id);
    
    /// Obtém todos os utilizadores.
    /// <returns>Lista de todos os utilizadores.</returns>
    Task<List<User>> GetAllAsync();
    
    /// Adiciona um novo usuário.
    /// <param name="user">Instância do utilizador a ser adicionado.</param>
    Task AddAsync(User user);
    
    /// Atualiza um usuário existente.
    /// <param name="user">Instância do utilizador a ser atualizado.</param>
    Task UpdateAsync(User user);
    
    /// Verifica se existe pelo menos um utilizador.
    /// <returns>True se existir pelo menos um utilizador, false caso contrário.</returns>
    Task<bool> AnyAsync();
}

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

/// Interface para repositório de registros, definindo operações CRUD e específicas.
public interface IRecordRepository
{

    /// Obtém um registro pelo ID.
    /// <param name="id">ID do registro.</param>
    /// <returns>O registro encontrado ou null se não existir.</returns>
    Task<Record?> GetByIdAsync(int id);
    
    /// Obtém todos os registros.
    /// <returns>Lista de todos os registros.</returns>
    Task<List<Record>> GetAllAsync();
    
    /// Obtém registros visíveis para um utilizador específico (próprios ou compartilhados).
    /// <param name="userId">ID do usuário.</param>
    /// <returns>Lista de registros visíveis.</returns>
    Task<List<Record>> GetVisibleForUserAsync(int userId);
    
    /// Adiciona um novo registro.
    /// <param name="record">Instância do registro a ser adicionado.</param>
    Task AddAsync(Record record);
    
    /// Atualiza um registro existente.
    /// <param name="record">Instância do registro a ser atualizado.</param>
    Task UpdateAsync(Record record);
    
    /// Remove um registro pelo ID.
    /// <param name="id">ID do registro a ser removido.</param>
    Task DeleteAsync(int id);
}