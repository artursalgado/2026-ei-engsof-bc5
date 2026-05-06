using GestaoTalentos.Domain;

using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;


namespace GestaoTalentos.Infrastructure;

public class TalentoElegivelRepository : Repository<TalentoElegivel>, ITalentoElegivelRepository
{
    public TalentoElegivelRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<TalentoElegivel>> GetByPropostaIdAsync(int propostaId)
    {
        return await _context.TalentosElegiveis
            .Include(te => te.Perfil)
            .Where(te => te.PropostaId == propostaId)
            .ToListAsync();
    }

    public async Task<IEnumerable<TalentoElegivel>> GetByPropostaIdOrderedByValorAsync(int propostaId)
    {
        return await _context.TalentosElegiveis
            .Include(te => te.Perfil)
            .Where(te => te.PropostaId == propostaId)
            .OrderBy(te => te.ValorEstimado)
            .ToListAsync();
    }

    public async Task DeleteByPropostaIdAsync(int propostaId)
    {
        var talentos = await _context.TalentosElegiveis
            .Where(te => te.PropostaId == propostaId)
            .ToListAsync();
        
        _context.TalentosElegiveis.RemoveRange(talentos);
        await _context.SaveChangesAsync();
    }

    //
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}