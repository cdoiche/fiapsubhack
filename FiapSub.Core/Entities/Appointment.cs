using FiapSub.Core.Enums;

namespace FiapSub.Core.Entities;

public class Appointment
{
    public int Id { get; private set; }
    public int PatientId { get; private set; }
    public int DoctorId { get; private set; }
    public DateTime DateTime { get; private set; }
    public AppointmentStatus Status { get; private set; }
    public string Description { get; private set; } = "";

    public Appointment(int patientId, int doctorId, DateTime dateTime)
    {
        PatientId = patientId;
        DoctorId = doctorId;
        DateTime = dateTime;
        Status = AppointmentStatus.Pending;
    }

    public void Confirm()
    {
        if (Status != AppointmentStatus.Pending)
            throw new InvalidOperationException("Only pending appointments can be confirmed.");

        Status = AppointmentStatus.Confirmed;
    }

    public void Reject(string? reason)
    {
        if (Status != AppointmentStatus.Pending)
            throw new InvalidOperationException("Only pending appointments can be rejected.");

        if (!string.IsNullOrEmpty(reason))
        {
            Description = reason;
        }
        Status = AppointmentStatus.Cancelled;
    }

    public void Cancel(string? reason)
    {
        if (Status == AppointmentStatus.Cancelled)
            throw new InvalidOperationException("This appointment is already cancelled.");

        Status = AppointmentStatus.Cancelled;

        if (!string.IsNullOrEmpty(reason))
        {
            Description = reason;
        }
    }
}