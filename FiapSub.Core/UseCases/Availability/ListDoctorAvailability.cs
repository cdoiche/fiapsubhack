using FiapSub.Core.Entities;
using FiapSub.Core.Interfaces;

namespace FiapSub.Core.UseCases.Availability;

public class ListDoctorAvailabilityUseCase
{
    private readonly IDoctorAvailabilityRepository _doctorAvailabilityRepository;

    public ListDoctorAvailabilityUseCase(IDoctorAvailabilityRepository doctorAvailabilityRepository)
    {
        _doctorAvailabilityRepository = doctorAvailabilityRepository;
    }

    public async Task<IEnumerable<DoctorAvailability>> ExecuteAsync(int doctorId)
    {
        if (doctorId <= 0)
        {
            throw new ArgumentException("Invalid doctor ID.");
        }

        var availabilities = await _doctorAvailabilityRepository.GetAvailableSlotsAsync(doctorId);

        if (!availabilities.Any())
        {
            throw new KeyNotFoundException($"No available slots found for doctor with ID {doctorId}.");
        }

        return availabilities;
    }
}