using System;

namespace GestaoTalentos.Infrastructure;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
    Task<bool> AnyAsync();
    Task<bool> ExistsAsync(int id);
}