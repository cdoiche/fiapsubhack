using FiapSub.Core.Entities;
using FiapSub.Core.Interfaces;

namespace FiapSub.Core.UseCases.Patients;

public class RegisterPatientUseCase
{
    private readonly IPatientRepository _patientRepository;

    public RegisterPatientUseCase(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task ExecuteAsync(Patient patient)
    {
        if (await _patientRepository.ExistsWithEmailAsync(patient.Email))
        {
            throw new InvalidOperationException("Patient with this email already exists.");
        }
        await _patientRepository.AddAsync(patient);
    }
}