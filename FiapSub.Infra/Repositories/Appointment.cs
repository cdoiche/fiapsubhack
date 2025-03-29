
using FiapSub.Core.Enums;
using FiapSub.Core.Entities;
using FiapSub.Core.Interfaces;
using FiapSub.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace FiapSub.Infra.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly ApplicationDbContext _context;
    public AppointmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Appointment appointment)
    {
        await _context.Appointments.AddAsync(appointment);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment != null)
        {
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Appointment>> GetAllAsync()
    {
        return await _context.Appointments.ToListAsync();
    }

    public async Task<Appointment> GetByIdAsync(int id)
    {
        var appointment = await _context.Appointments.FindAsync(id)
            ?? throw new KeyNotFoundException($"Appointment with id {id} not found");

        return appointment;
    }

    public async Task<IEnumerable<Appointment>> GetCancelledAppointmentsByDoctorAsync(int doctorId)
    {
        var appointments = _context.Appointments
                                    .Where(a => a.DoctorId == doctorId && 
                                           a.Status == AppointmentStatus.Cancelled)
                                    .OrderBy(a => a.DateTime);
        return await appointments.ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetConfirmedAppointmentsByDoctorAsync(int doctorId)
    {
        var appointments = _context.Appointments
                                    .Where(a => a.DoctorId == doctorId && 
                                           a.Status == AppointmentStatus.Confirmed &&
                                           a.DateTime >= DateTime.Now)
                                    .OrderBy(a => a.DateTime);
        return await appointments.ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetPendingAppointmentsByDoctorAsync(int doctorId)
    {
        var appointments = _context.Appointments
                                    .Where(a => a.DoctorId == doctorId && 
                                           a.Status == AppointmentStatus.Pending &&
                                           a.DateTime >= DateTime.Now)
                                    .OrderBy(a => a.DateTime);
        return await appointments.ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetPastAppointmentsByDoctorAsync(int doctorId)
    {
        var appointments = _context.Appointments
                                    .Where(a => a.DoctorId == doctorId &&
                                           a.DateTime < DateTime.Now)
                                    .OrderBy(a => a.DateTime);
        return await appointments.ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetPastAppointmentsByPatientAsync(int patientId)
    {
        var appointments = _context.Appointments
                                    .Where(a => a.PatientId == patientId &&
                                           a.DateTime < DateTime.Now)
                                    .OrderBy(a => a.DateTime);
        return await appointments.ToListAsync();
    }   

    public async Task<IEnumerable<Appointment>> GetUpcomingAppointmentsByPatientAsync(int patientId)
    {
        var appointments = _context.Appointments
                                    .Where(a => a.PatientId == patientId && 
                                           a.DateTime >= DateTime.Now)
                                    .OrderBy(a => a.DateTime);
        return await appointments.ToListAsync();
    }

    public async Task UpdateAsync(Appointment appointment)
    {
        _context.Update(appointment);
        await _context.SaveChangesAsync();
    }  
}