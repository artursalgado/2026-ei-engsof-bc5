using GestaoTalentos.Domain;
using Microsoft.EntityFrameworkCore;

namespace GestaoTalentos.Infrastructure;

public class UserRepository : IUserRepository
{
    private readonly GestaoTalentos.Infrastructure.AppDbContext _context;
    public UserRepository(GestaoTalentos.Infrastructure.AppDbContext context) => _context = context;

    public async Task<User?> GetByUsernameAsync(string username)
        => await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

    public async Task<User?> GetByIdAsync(int id)
        => await _context.Users.FindAsync(id);

    public async Task<List<User>> GetAllAsync()
        => await _context.Users.AsNoTracking().ToListAsync();

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> AnyAsync() => await _context.Users.AnyAsync();
}