using GestaoTalentos.Domain;
using Microsoft.EntityFrameworkCore;

namespace GestaoTalentos.Infrastructure;

/// Repositório para operações de base de dados relacionadas a registros (Records).
/// Implementa a interface IRecordRepository.
public class RecordRepository : IRecordRepository
{
    private readonly AppDbContext _context;
    private DbSet<Record> Records => _context.Set<Record>();

    /// Construtor que injeta o contexto da base de dados.
    /// <param name="context">Instância do AppDbContext.</param>
    public RecordRepository(AppDbContext context) => _context = context;
    
    /// Obtém um registro pelo ID.
    /// <param name="id">ID do registro.</param>
    /// <returns>O registro encontrado ou null se não existir.</returns>
    public async Task<Record?> GetByIdAsync(int id)
        => await Records.FindAsync(id);
    
    /// Obtém todos os registros.
    /// <returns>Lista de todos os registros.</returns>
    public async Task<List<Record>> GetAllAsync()
        => await Records.AsNoTracking().ToListAsync();


    /// Obtém registros visíveis para um utilizador específico (próprios ou compartilhados).
    /// <param name="userId">ID do utilizador.</param>
    /// <returns>Lista de registros visíveis.</returns>
    public async Task<List<Record>> GetVisibleForUserAsync(int userId)
        => await Records.AsNoTracking()
            .Where(r => r.OwnerId == userId || r.IsShared)
            .ToListAsync();
    
    /// Adiciona um novo registro à base de dados.
    /// <param name="record">Instância do registro a ser adicionado.</param>
    public async Task AddAsync(Record record)
    {
        await Records.AddAsync(record);
        await _context.SaveChangesAsync();
    }


    /// Atualiza um registro existente na base de dados.
    /// <param name="record">Instância do registro a ser atualizado.</param>
    public async Task UpdateAsync(Record record)
    {
        Records.Update(record);
        await _context.SaveChangesAsync();
    }
    
    /// Remove um registro pelo ID.
    /// <param name="id">ID do registro a ser removido.</param>
    public async Task DeleteAsync(int id)
    {
        var entity = await Records.FindAsync(id);
        if (entity != null)
        {
            Records.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}