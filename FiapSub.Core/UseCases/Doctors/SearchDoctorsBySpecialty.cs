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
        if (string.IsNullOrWhiteSpace(specialty.ToString()))
        {
            throw new ArgumentException("Specialty is required.");
        }

        var doctors = await _doctorRepository.SearchBySpecialtyAsync(specialty);

        return doctors;
    }
}