using FiapSub.Core.Entities;
using FiapSub.Core.Interfaces;

namespace FiapSub.Core.UseCases.Appointments;

public class ListPatientPastAppointmentsUseCase
{
    private readonly IAppointmentRepository _appointmentRepository;

    public ListPatientPastAppointmentsUseCase(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<IEnumerable<Appointment>> ExecuteAsync(int patientId)
    {
        if (patientId <= 0)
        {
            throw new ArgumentException("Invalid patient ID.");
        }

        var appointments = await _appointmentRepository.GetPastAppointmentsByPatientAsync(patientId);

        return appointments;
    }
}