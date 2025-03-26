using FiapSub.Core.Entities;

namespace FiapSub.Core.Interfaces;

public interface IPatientRepository
{
    Task<Patient> GetByIdAsync(int id);
    Task<IEnumerable<Patient>> GetByIdAsync();
    Task AddAsync(Patient patient);
    Task UpdateAsync(Patient patient);
    Task DeleteAsync(Patient patient);

    Task<Patient> GetByEmailAsync(string email);
    Task<bool> ExistsWithEmailAsync(string email);
}