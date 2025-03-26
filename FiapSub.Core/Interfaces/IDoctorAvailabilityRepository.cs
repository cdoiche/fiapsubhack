using FiapSub.Core.Entities;

namespace FiapSub.Core.Interfaces;

public interface IDoctorAvailabilityRepository
{
    Task AddAvailabilityAsync(DoctorAvailability availability);
    Task AddAvailabilityRangeAsync(int doctorId, DateTime start, DateTime end);
    Task AddAvailabilityBatchAsync(IEnumerable<DoctorAvailability> availabilities);

    Task BlockAvailabilityAsync(int availabilityId);
    Task BlockAvailabilityRangeAsync(int doctorId, DateTime start, DateTime end);
    Task BlockAvailabilityBatchAsync(IEnumerable<int> availabilityIds);
    Task BlockAvailabilityDayAsync(int doctorId, DateTime date);

    Task<IEnumerable<DoctorAvailability>> GetAvailableSlotsAsync(int doctorId);
    Task<DoctorAvailability> GetAvailabilityByIdAsync(int availabilityId);

    Task UpdateAvailabilityAsync(DoctorAvailability availability);

    Task DeleteAvailabilityAsync(int availabilityId);
}