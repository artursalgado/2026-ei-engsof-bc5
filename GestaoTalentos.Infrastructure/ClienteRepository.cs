using GestaoTalentos.Domain;
using Microsoft.EntityFrameworkCore;

namespace GestaoTalentos.Infrastructure;

/// Repositório para operações de base de dados relacionadas a clientes.
/// Implementa a interface IClienteRepository.
public class ClienteRepository : IClienteRepository
{
    // Campo privado para armazenar o contexto da base de dados,
    // permitindo acesso às entidades e operações de persistência.
    private readonly GestaoTalentos.Infrastructure.AppDbContext _context;

    /// Construtor que injeta o contexto da base de dados.
    // <param name="context">Instância do AppDbContext.</param>
    public ClienteRepository(GestaoTalentos.Infrastructure.AppDbContext context) => _context = context;

    public async Task<Cliente?> GetByNomeAsync(string nome) /// Obtém um cliente pelo nome.
        => await _context.Clientes.FirstOrDefaultAsync(c => c.Nome == nome);

    public async Task<Cliente?> GetByIdAsync(int id) /// Obtém um cliente pelo ID.
        => await _context.Clientes.FindAsync(id);

    public async Task<Cliente?> GetByEmailAsync(string email) /// Obtém um cliente pelo email.
        => await _context.Clientes.FirstOrDefaultAsync(c => c.Email == email);

    public async Task<List<Cliente>> GetAllAsync() /// Obtém todos os clientes
        => await _context.Clientes.AsNoTracking().ToListAsync();

    public async Task<List<Cliente>> GetByCriadorIdAsync(int criadorId) /// Obtém clientes pelo ID do criador.
        => await _context.Clientes.Where(c => c.IdCriador == criadorId).AsNoTracking().ToListAsync();

    public async Task AddAsync(Cliente cliente) /// Adiciona um novo cliente ao banco de dados.
    {
        if (cliente.IdMinhaConta == 0)
        {
            cliente.IdMinhaConta = null;
        }
        await _context.Clientes.AddAsync(cliente);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Cliente cliente) /// Atualiza um cliente existente no banco de dados.
    {
        if (cliente.IdMinhaConta == 0)
        {
            cliente.IdMinhaConta = null;
        }
        _context.Clientes.Update(cliente);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id) /// Exclui um cliente do banco de dados pelo ID.
    {
        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente != null)
        {
            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> AnyAsync() /// Verifica se existe pelo menos um cliente no banco de dados.
        => await _context.Clientes.AnyAsync();

    public async Task SaveChangesAsync() // NOVO MÉTODO
        => await _context.SaveChangesAsync();

    public async Task<Cliente?> GetByMinhaContaAsync(int userId)
        => await _context.Clientes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.IdMinhaConta == userId);

    //public async Task<Cliente?> GetWithPropostaAsync(int id)
    // => await _context.Clientes
    //  .Include(c => c.Propostas)
    // .FirstOrDefaultAsync(c => c.Id == id);
}