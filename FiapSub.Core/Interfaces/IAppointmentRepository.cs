using FiapSub.Core.Entities;

namespace FiapSub.Core.Interfaces;

public interface IAppointmentRepository
{
    Task<Appointment> GetByIdAsync(int id);
    Task<IEnumerable<Appointment>> GetAllAsync();
    Task AddAsync(Appointment appointment);
    Task UpdateAsync(Appointment appointment);
    Task DeleteAsync(int id);

    Task<IEnumerable<Appointment>> GetUpcomingAppointmentsByPatientAsync(int patientId);
    Task<IEnumerable<Appointment>> GetPastAppointmentsByPatientAsync(int patientId);
    Task<IEnumerable<Appointment>> GetPendingAppointmentsByDoctorAsync(int doctorId);
    Task<IEnumerable<Appointment>> GetConfirmedAppointmentsByDoctorAsync(int doctorId);
    Task<IEnumerable<Appointment>> GetCancelledAppointmentsByDoctorAsync(int doctorId);
    Task<IEnumerable<Appointment>> GetPastAppointmentsByDoctorAsync(int doctorId);
}