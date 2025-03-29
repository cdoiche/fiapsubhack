using FiapSub.Core.Enums;
using FiapSub.Core.Interfaces;

namespace FiapSub.Core.UseCases.Appointments;

public class CancelConfirmedAppointmentUseCase
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly INotificationService _notificationService;
    private readonly IPatientRepository _patientRepository;
    private readonly IDoctorRepository _doctorRepository;

    public CancelConfirmedAppointmentUseCase(
        IAppointmentRepository appointmentRepository,
        INotificationService notificationService,
        IPatientRepository patientRepository,
        IDoctorRepository doctorRepository)
    {
        _appointmentRepository = appointmentRepository;
        _notificationService = notificationService;
        _patientRepository = patientRepository;
        _doctorRepository = doctorRepository;
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

        var patient = await _patientRepository.GetByIdAsync(appointment.PatientId);
        var doctor = await _doctorRepository.GetByIdAsync(appointment.DoctorId);

        await _notificationService.NotifyAppointmentCancelledAsync(appointment.Id, patient.Email, cancellationReason);
        await _notificationService.NotifyAppointmentCancelledAsync(appointment.Id, doctor.Email, cancellationReason);
    }
}