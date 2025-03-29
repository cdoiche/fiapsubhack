using FiapSub.Core.Enums;
using FiapSub.Core.Interfaces;

namespace FiapSub.Core.UseCases.Appointments;

public class CancelConfirmedAppointmentUseCase
{
    private readonly IAppointmentRepository _appointmentRepository;

    public CancelConfirmedAppointmentUseCase(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task ExecuteAsync(int userId, int appointmentId, string userType, string? cancellationReason = null)
    {
        if (userId <= 0)
        {
            throw new ArgumentException("Invalid user ID.");
        }

        if (appointmentId <= 0)
        {
            throw new ArgumentException("Invalid appointment ID.");
        }

        var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);

        if (appointment == null)
        {
            throw new KeyNotFoundException("Appointment not found.");
        }

        if (appointment.Status != AppointmentStatus.Confirmed)
        {
            throw new InvalidOperationException("Only confirmed appointments can be cancelled.");
        }

        if ((userType == "Patient" && appointment.PatientId != userId) ||
            (userType == "Doctor" && appointment.DoctorId != userId))
        {
            throw new UnauthorizedAccessException("You are not authorized to cancel this appointment.");
        }

        appointment.Cancel(cancellationReason);
        await _appointmentRepository.UpdateAsync(appointment);
    }
}