using FiapSub.Core.Entities;
using FiapSub.Core.Enums;
using FiapSub.Core.Interfaces;

namespace FiapSub.Core.UseCases.Doctors;

public class SearchDoctorsBySpecialtyUseCase
{
    private readonly IDoctorRepository _doctorRepository;
    public SearchDoctorsBySpecialtyUseCase(IDoctorRepository doctorRepository)
    {
        _doctorRepository = doctorRepository;
    }
    public async Task<IEnumerable<Doctor>> ExecuteAsync(Specialty specialty)
    {
        if (!Enum.IsDefined(typeof(Specialty), specialty))
        {
            throw new ArgumentException("Specialty is required and must be valid.");
        }

        var doctors = await _doctorRepository.SearchBySpecialtyAsync(specialty);

        return doctors;
    }
}