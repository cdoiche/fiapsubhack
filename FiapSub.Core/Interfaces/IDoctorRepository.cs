using FiapSub.Core.Entities;
using FiapSub.Core.Enums;

namespace FiapSub.Core.Interfaces;

public interface IDoctorRepository : IUserRepository<Doctor>
{
    Task<IEnumerable<Doctor>> SearchBySpecialtyAsync(Specialty specialty);
}