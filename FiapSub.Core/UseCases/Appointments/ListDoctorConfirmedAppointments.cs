using FiapSub.Core.Entities;
using FiapSub.Core.Interfaces;

namespace FiapSub.Core.UseCases.Appointments;

public class ListDoctorConfirmedAppointmentsUseCase
{
    private readonly IAppointmentRepository _appointmentRepository;

    public ListDoctorConfirmedAppointmentsUseCase(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<IEnumerable<Appointment>> ExecuteAsync(int doctorId)
    {
        if (doctorId <= 0)
        {
            throw new ArgumentException("Invalid doctor ID.");
        }

        var appointments = await _appointmentRepository.GetConfirmedAppointmentsByDoctorAsync(doctorId);

        return appointments;
    }
}