using FiapSub.Core.Entities;
using FiapSub.Core.Enums;

namespace FiapSub.Core.Interfaces;

public interface IDoctorRepository
{
    Task<Doctor> GetByIdAsync(int id);
    Task<IEnumerable<Doctor>> GetAllAsync();
    Task AddAsync(Doctor doctor);
    Task UpdateAsync(Doctor doctor);
    Task DeleteAsync( int id );

    Task<Doctor> GetByEmailAsync(string email);
    Task<bool> ExistsWithEmailAsync(string email);
    Task<IEnumerable<Doctor>> SearchBySpecialtyAsync(Specialty specialty);
}