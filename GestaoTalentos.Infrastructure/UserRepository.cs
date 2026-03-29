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
    
    
    public async Task<User?> GetByUsernameAsync(string username) /// Obtém um utilizador pelo nome de utilizador.
        => await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    public async Task<User?> GetByIdAsync(int id) /// Obtém um utilizador pelo ID.
        => await _context.Users.FindAsync(id);

    
    public async Task<List<User>> GetAllAsync() /// Obtém todos os utilizadores.
        => await _context.Users.AsNoTracking().ToListAsync();

    
    public async Task AddAsync(User user)     /// Adiciona um novo utilizador na base de dados.
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateAsync(User user)    /// Atualiza um utilizador existente na base de dados.
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteAsync(int id)    /// Exclui um utilizador da base de dados pelo ID.
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }

    
    public async Task<bool> AnyAsync() /// Verifica se existe pelo menos um utilizador na base de dados. 
        => await _context.Users.AnyAsync();
}