using FiapSub.Core.Entities;
using FiapSub.Core.Interfaces;
using FiapSub.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace FiapSub.Infra.Repositories;

public class DoctorAvailabilityRepository : IDoctorAvailabilityRepository
{
    private readonly ApplicationDbContext _context;

    public DoctorAvailabilityRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAvailabilityAsync(DoctorAvailability availability)
    {
        await _context.DoctorAvailabilities.AddAsync(availability);
        await _context.SaveChangesAsync();
    }

    public async Task AddAvailabilityBatchAsync(IEnumerable<DoctorAvailability> availabilities)
    {
        await _context.DoctorAvailabilities.AddRangeAsync(availabilities);
        await _context.SaveChangesAsync();
    }

    public async Task AddAvailabilityRangeAsync(int doctorId, DateTime start, DateTime end)
    {
        var availabilities = new List<DoctorAvailability>();
        for (int hour = start.Hour; hour < end.Hour; hour++)
        {
            availabilities.Add(
                new DoctorAvailability(
                    doctorId, 
                    new DateTime(start.Year, start.Month, start.Day, hour, 0, 0), 
                    new DateTime(start.Year, start.Month, start.Day, hour + 1, 0, 0)
                )
            );
        }

        await _context.DoctorAvailabilities.AddRangeAsync(availabilities);
        await _context.SaveChangesAsync();
    }

    public async Task BlockAvailabilityAsync(int availabilityId)
    {
        var availability = await _context.DoctorAvailabilities.FindAsync(availabilityId) 
            ?? throw new KeyNotFoundException("Availability not found.");
        availability.Block();
        await _context.SaveChangesAsync();
    }

    public async Task BlockAvailabilityBatchAsync(IEnumerable<int> availabilityIds)
    {
        var availabilities = await _context.DoctorAvailabilities
            .Where(a => availabilityIds.Contains(a.Id))
            .ToListAsync();
            
        availabilities.ForEach(a => a.Block());
        await _context.SaveChangesAsync();
    }

    public async Task BlockAvailabilityDayAsync(int doctorId, DateTime date)
    {
        var availabilities = await _context.DoctorAvailabilities
            .Where(a => a.DoctorId == doctorId && a.StartTime.Date == date.Date)
            .ToListAsync();
        availabilities.ForEach(a => a.Block());
        await _context.SaveChangesAsync();
    }

    public async Task BlockAvailabilityRangeAsync(int doctorId, DateTime start, DateTime end)
    {
        var availabilities = await _context.DoctorAvailabilities
            .Where(a => a.DoctorId == doctorId && a.StartTime >= start && a.EndTime <= end)
            .ToListAsync();
        availabilities.ForEach(a => a.Block());
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAvailabilityAsync(int availabilityId)
    {
        var availability = await _context.DoctorAvailabilities.FindAsync(availabilityId);

        if(availability != null)
        {
            _context.DoctorAvailabilities.Remove(availability);
            await _context.SaveChangesAsync();
        }        
    }

    public async Task<bool> ExistsAsync(int doctorId, DateTime start, DateTime end)
    {
        return await _context.DoctorAvailabilities
                             .AnyAsync(a => a.DoctorId == doctorId && 
                                            a.StartTime == start && 
                                            a.EndTime == end);
    }

    public async Task<DoctorAvailability> GetAvailabilityByIdAsync(int availabilityId)
    {
        var availability = await _context.DoctorAvailabilities.FindAsync(availabilityId)
            ?? throw new KeyNotFoundException($"Availability with id {availabilityId} not found.");

        return availability;
    }

    public async Task<IEnumerable<DoctorAvailability>> GetAvailableSlotsAsync(int doctorId)
    {
        var availabilities = await _context.DoctorAvailabilities
            .Where(a => a.DoctorId == doctorId && a.IsAvailable)
            .ToListAsync();
        return availabilities;
    }

    public async Task UpdateAvailabilityAsync(DoctorAvailability availability)
    {
        _context.DoctorAvailabilities.Update(availability);
        await _context.SaveChangesAsync();
    }
}