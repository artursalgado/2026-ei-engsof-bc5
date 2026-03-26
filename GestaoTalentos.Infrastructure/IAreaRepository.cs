using System;
//using Microsoft.EntityFrameworkCore;
//using GestaoTalentos.Domain;

//namespace GestaoTalentos.Infrastructure;

//public class AreaRepository : IAreaRepository
//{
//    private readonly AppDbContext _context;

//    public AreaRepository(AppDbContext context)
//    {
//        _context = context;
//    }

//    public async Task<Area?> GetByIdAsync(int id)
//    {
//        return await _context.Areas
//            .Include(a => a.Skills)
//            .FirstOrDefaultAsync(a => a.Id == id);
//    }

//    public async Task<IEnumerable<Area>> GetAllAsync()
//    {
//        return await _context.Areas
//            .OrderBy(a => a.Nome)
//            .ToListAsync();
//    }

//    public async Task<IEnumerable<Area>> GetAllWithSkillsAsync()
//    {
//        return await _context.Areas
//            .Include(a => a.Skills)
//            .OrderBy(a => a.Nome)
//            .ToListAsync();
//    }

//    public async Task<Area?> GetByNomeAsync(string nome)
//    {
//        return await _context.Areas
//            .FirstOrDefaultAsync(a => a.Nome.ToLower() == nome.ToLower());
//    }

//    public async Task AddAsync(Area entity)
//    {
//        await _context.Areas.AddAsync(entity);
//        await _context.SaveChangesAsync();
//    }

//    public async Task UpdateAsync(Area entity)
//    {
//        _context.Areas.Update(entity);
//        await _context.SaveChangesAsync();
//    }

//    public async Task DeleteAsync(int id)
//    {
//        var area = await GetByIdAsync(id);
//        if (area != null)
//        {
//            _context.Areas.Remove(area);
//            await _context.SaveChangesAsync();
//        }
//    }

//    public async Task<bool> AnyAsync()
//    {
//        return await _context.Areas.AnyAsync();
//    }

//    public async Task<bool> ExistsAsync(int id)
//    {
//        return await _context.Areas.AnyAsync(a => a.Id == id);
//    }
//}

namespace GestaoTalentos.Infrastructure;

using GestaoTalentos.Domain;

public interface IAreaRepository : IRepository<Area>
{
    Task<Area?> GetByNomeAsync(string nome);
    Task<IEnumerable<Area>> GetAllWithSkillsAsync();
}