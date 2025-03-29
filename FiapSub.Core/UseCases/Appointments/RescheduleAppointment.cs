using FiapSub.Core.Entities;
using FiapSub.Core.Enums;
using FiapSub.Core.Interfaces;

namespace FiapSub.Core.UseCases.Appointments;

public class RescheduleAppointmentUseCase
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IDoctorAvailabilityRepository _doctorAvailabilityRepository;

    public RescheduleAppointmentUseCase(
        IAppointmentRepository appointmentRepository,
        IDoctorAvailabilityRepository doctorAvailabilityRepository)
    {
        _appointmentRepository = appointmentRepository;
        _doctorAvailabilityRepository = doctorAvailabilityRepository;
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

        var newAppointment = new Appointment(appointment.PatientId, availability.DoctorId, availability.StartTime);

        availability.Block();
        await _doctorAvailabilityRepository.UpdateAvailabilityAsync(availability);

        await _appointmentRepository.AddAsync(newAppointment);
    }
}