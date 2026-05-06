using GestaoTalentos.Domain;
using Microsoft.EntityFrameworkCore;

namespace GestaoTalentos.Infrastructure;

public class RoleRepository : IRoleRepository
{
    private readonly GestaoTalentos.Infrastructure.AppDbContext _context;
    
    /// Construtor que injeta o contexto da base de dados.
    /// <param name="context">Instância do AppDbContext.</param>
    public RoleRepository(GestaoTalentos.Infrastructure.AppDbContext context) => _context = context;

    public async Task<Role?> GetByNomeAsync(string nome) /// Obtém um Role pelo nome de utilizador.
    {
        var nomeNormalizado = nome.Trim().ToLower();
        return await _context.Roles
            .Include(r => r.Users)
            .FirstOrDefaultAsync(r => r.Nome.ToLower() == nomeNormalizado);
    }
    public async Task<List<Role>> GetAllAsync() /// Obtém todos os utilizadores.
        => await _context.Roles.AsNoTracking().ToListAsync();
    public async Task<IEnumerable<Role>> GetAllWithUsersAsync()
    {
        return await _context.Roles
            .Include(r => r.Users)
            .OrderBy(r => r.Nome)
            .ToListAsync();
    }
    
    public async Task AddAsync(Role role)     /// Adiciona um novo utilizador na base de dados.
    {
        await _context.Roles.AddAsync(role);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteAsync(string nome)    /// Exclui um utilizador da base de dados pelo ID.
    {
        var role = await _context.Roles
            .FirstOrDefaultAsync(r => r.Nome.ToLower() == nome.ToLower());
        if (role != null)
        {
            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
        }
    }
}