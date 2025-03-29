using FiapSub.Core.Enums;
using FiapSub.Core.Interfaces;

namespace FiapSub.Core.UseCases.Appointments;

public class RejectPendingAppointmentUseCase
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly INotificationService _notificationService;
    private readonly IPatientRepository _patientRepository;

    public RejectPendingAppointmentUseCase(
        IAppointmentRepository appointmentRepository,
        INotificationService notificationService,
        IPatientRepository patientRepository)
    {
        _appointmentRepository = appointmentRepository;
        _notificationService = notificationService;
        _patientRepository = patientRepository;
    }

    public async Task ExecuteAsync(int doctorId, int appointmentId, string? rejectionReason = null)
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
            throw new UnauthorizedAccessException("You are not authorized to reject this appointment.");
        }

        if (appointment.Status != AppointmentStatus.Pending)
        {
            throw new InvalidOperationException("Only pending appointments can be rejected.");
        }

        appointment.Reject(rejectionReason);
        await _appointmentRepository.UpdateAsync(appointment);

        var patient = await _patientRepository.GetByIdAsync(appointment.PatientId);
        await _notificationService.NotifyAppointmentCancelledAsync(appointment.Id, patient.Email, rejectionReason);
    }
}