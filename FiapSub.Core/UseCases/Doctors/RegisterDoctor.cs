using FiapSub.Core.Entities;
using FiapSub.Core.Interfaces;

namespace FiapSub.Core.UseCases.Doctors;

public class RegisterDoctorUseCase
{
    private readonly IDoctorRepository _doctorRepository;

    public RegisterDoctorUseCase(IDoctorRepository doctorRepository)
    {
        _doctorRepository = doctorRepository;
    }

    public async Task ExecuteAsync(Doctor doctor)
    {
        if (await _doctorRepository.ExistsWithEmailAsync(doctor.Email))
        {
            throw new InvalidOperationException("Doctor with this email already exists.");
        }
        await _doctorRepository.AddAsync(doctor);
    }
}