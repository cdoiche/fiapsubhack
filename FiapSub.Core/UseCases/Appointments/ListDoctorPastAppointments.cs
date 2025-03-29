using FiapSub.Core.Entities;
using FiapSub.Core.Interfaces;

namespace FiapSub.Core.UseCases.Appointments;

public class ListDoctorPastAppointmentsUseCase
{
    private readonly IAppointmentRepository _appointmentRepository;

    public ListDoctorPastAppointmentsUseCase(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<IEnumerable<Appointment>> ExecuteAsync(int doctorId)
    {
        if (doctorId <= 0)
        {
            throw new ArgumentException("Invalid doctor ID.");
        }

        var appointments = await _appointmentRepository.GetPastAppointmentsByDoctorAsync(doctorId);

        return appointments;
    }
}