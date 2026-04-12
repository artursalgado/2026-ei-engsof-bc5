using GestaoTalentos.Domain;
using Microsoft.EntityFrameworkCore;

namespace GestaoTalentos.Infrastructure;

public class PaisRepository : IPaisRepository
{
    private readonly GestaoTalentos.Infrastructure.AppDbContext _context;
    
    /// Construtor que injeta o contexto da base de dados.
    /// <param name="context">Instância do AppDbContext.</param>
    public PaisRepository(GestaoTalentos.Infrastructure.AppDbContext context) => _context = context;

    public async Task<Pais?> GetByNomeAsync(string nome) /// Obtém um utilizador pelo nome de utilizador.
    {
        var nomeNormalizado = nome.Trim().ToLower();
        return await _context.Paises
            .Include(p => p.Perfis)
            .FirstOrDefaultAsync(p => p.Nome.ToLower() == nomeNormalizado);
    }
    public async Task<List<Pais>> GetAllAsync() /// Obtém todos os utilizadores.
        => await _context.Paises.AsNoTracking().ToListAsync();
    public async Task<IEnumerable<Pais>> GetAllWithSkilsAsync()
    {
        return await _context.Paises
            .Include(p => p.Perfis)
            .OrderBy(p => p.Nome)
            .ToListAsync();
    }
    
    public async Task AddAsync(Pais pais)     /// Adiciona um novo utilizador na base de dados.
    {
        await _context.Paises.AddAsync(pais);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteAsync(string nome)    /// Exclui um utilizador da base de dados pelo ID.
    {
        var pais = await _context.Paises
            .FirstOrDefaultAsync(p => p.Nome.ToLower() == nome.ToLower());
        if (pais != null)
        {
            _context.Paises.Remove(pais);
            await _context.SaveChangesAsync();
        }
    }
}