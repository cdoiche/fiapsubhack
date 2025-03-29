using FiapSub.Core.Entities;
using FiapSub.Core.Interfaces;

namespace FiapSub.Core.UseCases.Appointments;

public class ScheduleAppointmentUseCase
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IDoctorAvailabilityRepository _doctorAvailabilityRepository;
    private readonly INotificationService _notificationService;
    private readonly IPatientRepository _patientRepository;
    private readonly IDoctorRepository _doctorRepository;

    public ScheduleAppointmentUseCase(
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

    public async Task ExecuteAsync(int patientId, int availabilityId)
    {
        if (patientId <= 0)
        {
            throw new ArgumentException("Invalid patient ID.");
        }

        if (availabilityId <= 0)
        {
            throw new ArgumentException("Invalid availability ID.");
        }

        var availability = await _doctorAvailabilityRepository.GetAvailabilityByIdAsync(availabilityId);

        if (availability == null || !availability.IsAvailable)
        {
            throw new InvalidOperationException("The selected availability is not valid or is no longer available.");
        }

        if (availability.StartTime < DateTime.Now.AddHours(4))
            throw new ArgumentException("Appointment must be scheduled at least 4 hours in advance.");

        var appointment = new Appointment(patientId, availability.DoctorId, availability.StartTime);

        availability.Block();
        await _doctorAvailabilityRepository.UpdateAvailabilityAsync(availability);

        await _appointmentRepository.AddAsync(appointment);

        var patient = await _patientRepository.GetByIdAsync(patientId);
        var doctor = await _doctorRepository.GetByIdAsync(availability.DoctorId);

        await _notificationService.NotifyAppointmentScheduledAsync(appointment.Id, patient.Email);
        await _notificationService.NotifyAppointmentScheduledAsync(appointment.Id, doctor.Email);
    }
}