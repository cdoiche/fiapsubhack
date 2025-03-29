namespace FiapSub.API.DTOs;

public class CancelAppointmentRequest
{
    public int AppointmentId { get; set; }
    public string? CancellationReason { get; set; }
}