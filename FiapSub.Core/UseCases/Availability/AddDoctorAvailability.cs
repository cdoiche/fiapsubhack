using FiapSub.Core.Entities;
using FiapSub.Core.Interfaces;

namespace FiapSub.Core.UseCases.Availability;

public class AddDoctorAvailabilityUseCase
{
    private readonly IDoctorAvailabilityRepository _doctorAvailabilityRepository;

    public AddDoctorAvailabilityUseCase(IDoctorAvailabilityRepository doctorAvailabilityRepository)
    {
        _doctorAvailabilityRepository = doctorAvailabilityRepository;
    }

    public async Task ExecuteAsync(int doctorId, DateTime start, DateTime end)
    {
        if (doctorId <= 0)
        {
            throw new ArgumentException("Invalid doctor ID.");
        }

        if (start >= end)
        {
            throw new ArgumentException("Start time must be before end time.");
        }

        if (start < DateTime.UtcNow.AddHours(4))
        {
            throw new ArgumentException("Availability must be set at least 4 hours in the future.");
        }

        if (end > start.AddHours(1))
        {
            throw new ArgumentException("Default availability slot is one hour.");
        }

        var exists = await _doctorAvailabilityRepository.ExistsAsync(doctorId, start, end);
        if (exists)
        {
            throw new InvalidOperationException("This availability already exists.");
        }
        
        var availability = new DoctorAvailability(doctorId, start, end);
        await _doctorAvailabilityRepository.AddAvailabilityAsync(availability);
    }
}