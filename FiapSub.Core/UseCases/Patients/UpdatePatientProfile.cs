using FiapSub.Core.Interfaces;

namespace FiapSub.Core.UseCases.Patients;

public class UpdatePatientProfileUseCase
{
    private readonly IPatientRepository _patientRepository;

    public UpdatePatientProfileUseCase(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task ExecuteAsync(int patientId, string patientName, string patientPhone)
    {
        var patient = await _patientRepository.GetByIdAsync(patientId);
        if (patient == null)
        {
            throw new KeyNotFoundException("Patient not found.");
        }

        if (!string.IsNullOrEmpty(patientName)) {
            patient.UpdateProfile(patientName, patientPhone);
        }

        await _patientRepository.UpdateAsync(patient);
    }
}