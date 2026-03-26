using GestaoTalentos.Domain;
using Microsoft.EntityFrameworkCore;

namespace GestaoTalentos.Infrastructure;

/// Repositório para operações de base de dados relacionadas a utilizadores.
/// Implementa a interface IUserRepository.
public class UserRepository : IUserRepository
{
    private readonly GestaoTalentos.Infrastructure.AppDbContext _context;
    
    /// Construtor que injeta o contexto da base de dados.
    /// <param name="context">Instância do AppDbContext.</param>
    public UserRepository(GestaoTalentos.Infrastructure.AppDbContext context) => _context = context;
    
    /// Obtém um utilizador pelo nome de utilizador.
    /// <param name="username">Nome de utilizador.</param>
    /// <returns>O utilizador encontrado ou null se não existir.</returns>
    public async Task<User?> GetByUsernameAsync(string username)
        => await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    
    /// Obtém um utilizador pelo ID.
    /// <param name="id">ID do usuário.</param>
    /// <returns>O utilizador encontrado ou null se não existir.</returns>
    public async Task<User?> GetByIdAsync(int id)
        => await _context.Users.FindAsync(id);
    
    /// Obtém todos os utilizadores.
    /// <returns>Lista de todos os utilizadores.</returns>
    public async Task<List<User>> GetAllAsync()
        => await _context.Users.AsNoTracking().ToListAsync();


    /// Adiciona um novo utilizador na base de dados.
    /// <param name="user">Instância do utilizador a ser adicionado.</param>
    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }


    /// Atualiza um utilizador existente na base de dados.
    /// <param name="user">Instância do utilizador a ser atualizado.</param>
    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }


    /// Verifica se existe pelo menos um utilizador na base de dados.
    /// <returns>True se existir pelo menos um utilizador, false caso contrário.</returns>
    public async Task<bool> AnyAsync() => await _context.Users.AnyAsync();
}