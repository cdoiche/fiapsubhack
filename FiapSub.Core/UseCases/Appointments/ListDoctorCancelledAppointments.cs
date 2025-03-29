using FiapSub.Core.Entities;
using FiapSub.Core.Interfaces;

namespace FiapSub.Core.UseCases.Appointments;

public class ListDoctorCancelledAppointmentsUseCase
{
    private readonly IAppointmentRepository _appointmentRepository;

    public ListDoctorCancelledAppointmentsUseCase(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<IEnumerable<Appointment>> ExecuteAsync(int doctorId)
    {
        if (doctorId <= 0)
        {
            throw new ArgumentException("Invalid doctor ID.");
        }

        var appointments = await _appointmentRepository.GetCancelledAppointmentsByDoctorAsync(doctorId);

        return appointments;
    }
}