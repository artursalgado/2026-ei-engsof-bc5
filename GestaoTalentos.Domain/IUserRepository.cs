namespace GestaoTalentos.Domain;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task AddAsync(User user);
    Task<bool> AnyAsync(); 
}
