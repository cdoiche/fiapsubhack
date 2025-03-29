namespace FiapSub.Core.Interfaces;

public interface IUserRepository<T>
{
    Task<T> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T user);
    Task UpdateAsync(T user);
    Task DeleteAsync(int id);
    Task<T> GetByEmailAsync(string email);
    Task<bool> ExistsWithEmailAsync(string email);
}