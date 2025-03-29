using FiapSub.Core.Entities;
using FiapSub.Core.Enums;
using FiapSub.Core.Interfaces;

namespace FiapSub.Core.UseCases.Appointments;

public class RescheduleAppointmentUseCase
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IDoctorAvailabilityRepository _doctorAvailabilityRepository;
    private readonly INotificationService _notificationService;
    private readonly IPatientRepository _patientRepository;
    private readonly IDoctorRepository _doctorRepository;

    public RescheduleAppointmentUseCase(
        IAppointmentRepository appointmentRepository,
        IDoctorAvailabilityRepository doctorAvailabilityRepository,
        INotificationService notificationService,
        IPatientRepository patientRepository,
        IDoctorRepository doctorRepository)
    {
        _appointmentRepository = appointmentRepository;
        _doctorAvailabilityRepository = doctorAvailabilityRepository;
        _notificationService = notificationService;
        _patientRepository = patientRepository;
        _doctorRepository = doctorRepository;
    }

    public async Task ExecuteAsync(int appointmentId, int newAvailabilityId, string? cancellationReason = null)
    {
        if (appointmentId <= 0)
        {
            throw new ArgumentException("Invalid appointment ID.");
        }

        if (newAvailabilityId <= 0)
        {
            throw new ArgumentException("Invalid availability ID.");
        }

        var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);

        if (appointment == null || appointment.Status == AppointmentStatus.Cancelled)
        {
            throw new InvalidOperationException("The selected appointment is not valid or already canceled.");
        }

        var availability = await _doctorAvailabilityRepository.GetAvailabilityByIdAsync(newAvailabilityId);

        if (availability == null || !availability.IsAvailable)
        {
            throw new InvalidOperationException("The selected availability is not valid or is no longer available.");
        }

        appointment.Cancel(cancellationReason);
        await _appointmentRepository.UpdateAsync(appointment);

        var patient = await _patientRepository.GetByIdAsync(appointment.PatientId);
        var doctor = await _doctorRepository.GetByIdAsync(availability.DoctorId);

        await _notificationService.NotifyAppointmentCancelledAsync(appointment.Id, patient.Email, cancellationReason);
        await _notificationService.NotifyAppointmentCancelledAsync(appointment.Id, doctor.Email, cancellationReason);

        var newAppointment = new Appointment(appointment.PatientId, availability.DoctorId, availability.StartTime);

        availability.Block();
        await _doctorAvailabilityRepository.UpdateAvailabilityAsync(availability);

        await _appointmentRepository.AddAsync(newAppointment);

        await _notificationService.NotifyAppointmentScheduledAsync(appointment.Id, patient.Email);
        await _notificationService.NotifyAppointmentScheduledAsync(appointment.Id, doctor.Email);
    }
}