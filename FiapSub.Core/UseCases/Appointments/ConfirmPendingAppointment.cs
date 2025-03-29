using FiapSub.Core.Enums;
using FiapSub.Core.Interfaces;

namespace FiapSub.Core.UseCases.Appointments;

public class ConfirmPendingAppointmentUseCase
{
    private readonly IAppointmentRepository _appointmentRepository;

    public ConfirmPendingAppointmentUseCase(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task ExecuteAsync(int doctorId, int appointmentId)
    {
        if (doctorId <= 0)
        {
            throw new ArgumentException("Invalid doctor ID.");
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

        if (appointment.DoctorId != doctorId)
        {
            throw new UnauthorizedAccessException("You are not authorized to confirm this appointment.");
        }

        if (appointment.Status != AppointmentStatus.Pending)
        {
            throw new InvalidOperationException("Only pending appointments can be confirmed.");
        }

        appointment.Confirm();
        await _appointmentRepository.UpdateAsync(appointment);
    }
}