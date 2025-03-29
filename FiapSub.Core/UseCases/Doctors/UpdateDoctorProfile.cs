using FiapSub.Core.Enums;
using FiapSub.Core.Interfaces;

namespace FiapSub.Core.UseCases.Doctors;

public class UpdateDoctorProfileUseCase
{
    private readonly IDoctorRepository _doctorRepository;

    public UpdateDoctorProfileUseCase(IDoctorRepository doctorRepository)
    {
        _doctorRepository = doctorRepository;
    }

    public async Task ExecuteAsync(int doctorId, string doctorName, string doctorPhone, Specialty doctorSpecialty)
    {
        var doctor = await _doctorRepository.GetByIdAsync(doctorId);
        if (doctor == null)
        {
            throw new KeyNotFoundException("Doctor not found.");
        }

        if (!string.IsNullOrEmpty(doctorName)) {
            doctor.UpdateProfile(doctorName, doctorPhone, doctorSpecialty);
        }

        await _doctorRepository.UpdateAsync(doctor);
    }
}