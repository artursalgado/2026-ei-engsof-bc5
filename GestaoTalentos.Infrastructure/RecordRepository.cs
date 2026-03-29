using GestaoTalentos.Domain;
using Microsoft.EntityFrameworkCore;

namespace GestaoTalentos.Infrastructure;

/// Repositório para operações de base de dados relacionadas a registros (Records).
/// Implementa a interface IRecordRepository.
public class RecordRepository : IRecordRepository
{
    /// Campo privado para armazenar o contexto da base de dados,
    /// permitindo acesso às entidades e operações de persistência.
    private readonly AppDbContext _context;
    private DbSet<Record> Records => _context.Set<Record>();

    /// Construtor que injeta o contexto da base de dados.
    /// <param name="context">Instância do AppDbContext.</param>
    public RecordRepository(AppDbContext context) => _context = context;
    
    
    public async Task<Record?> GetByIdAsync(int id) // Obtém um registro pelo ID.
        => await Records.FindAsync(id);
 
    
    public async Task<List<Record>> GetAllAsync() // Obtém todos os registros.
        => await Records.AsNoTracking().ToListAsync();
    public async Task<List<Record>> GetVisibleForUserAsync(int userId) /// Obtém registros visíveis para um utilizador
        => await Records.AsNoTracking()                               /// específico (próprios ou compartilhados).
            .Where(r => r.OwnerId == userId || r.IsShared)
            .ToListAsync();
    
    
    public async Task AddAsync(Record record) /// Adiciona um novo registro à base de dados.
    {
        await Records.AddAsync(record);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateAsync(Record record) /// Atualiza um registro existente na base de dados.
    {
        Records.Update(record);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteAsync(int id)  /// Remove um registro pelo ID.
    {
        var entity = await Records.FindAsync(id);
        if (entity != null)
        {
            Records.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}