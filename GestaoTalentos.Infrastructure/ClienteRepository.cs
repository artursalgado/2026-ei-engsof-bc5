using GestaoTalentos.Domain;
using Microsoft.EntityFrameworkCore;

namespace GestaoTalentos.Infrastructure;

/// Repositório para operações de base de dados relacionadas a clientes.
/// Implementa a interface IClienteRepository.
public class ClienteRepository : IClienteRepository
{
    private readonly GestaoTalentos.Infrastructure.AppDbContext _context;
 
    /// Construtor que injeta o contexto da base de dados.
    /// <param name="context">Instância do AppDbContext.</param>
    public ClienteRepository(GestaoTalentos.Infrastructure.AppDbContext context) => _context = context;

    /// Obtém um cliente pelo nome.
    /// <param name="nome">Nome do cliente.</param>
    /// <returns>O cliente encontrado ou null se não existir.</returns>
    public async Task<Cliente?> GetByNomeAsync(string nome)
        => await _context.Clientes.FirstOrDefaultAsync(c => c.Nome == nome);

    /// Obtém um cliente pelo ID.
    /// <param name="id">ID do cliente.</param>
    /// <returns>O cliente encontrado ou null se não existir.</returns>
    public async Task<Cliente?> GetByIdAsync(int id)
        => await _context.Clientes.FindAsync(id);
    
    /// Obtém todos os clientes sem rastreamento.
    /// <returns>Lista de todos os clientes.</returns>
    public async Task<List<Cliente>> GetAllAsync()
        => await _context.Clientes.AsNoTracking().ToListAsync();
    
    /// Adiciona um novo cliente ao banco de dados.
    /// <param name="cliente">Instância do cliente a ser adicionado.</param>
    public async Task AddAsync(Cliente cliente)
    {
        await _context.Clientes.AddAsync(cliente);
        await _context.SaveChangesAsync();
    }
    
    /// Atualiza um cliente existente no banco de dados.
    /// <param name="cliente">Instância do cliente a ser atualizado.</param>
    public async Task UpdateAsync(Cliente cliente)
    {
        _context.Clientes.Update(cliente);
        await _context.SaveChangesAsync();
    }
    
    /// Verifica se existe pelo menos um cliente no banco de dados.
    /// <returns>True se existir pelo menos um cliente, false caso contrário.</returns>
    public async Task<bool> AnyAsync() => await _context.Clientes.AnyAsync();
    
    public async Task<List<Cliente>> GetByIdCriadorAsync(int id)
        => await _context.Clientes.Where(c => c.IdCriador == id).ToListAsync();
    
    public async Task<Cliente?> GetByIdMinhaContaAsync(int id)
        => await _context.Clientes.FirstOrDefaultAsync(c => c.IdMinhaConta == id);
    
    //public async Task<Cliente?> GetWithPropostaAsync(int id)
    // => await _context.Clientes
    //  .Include(c => c.Propostas)
    // .FirstOrDefaultAsync(c => c.Id == id);
}