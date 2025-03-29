namespace FiapSub.API.DTOs;

public class RescheduleAppointmentRequest
{
    public int AppointmentId { get; set; }
    public int NewAvailabilityId { get; set; }
    public string? CancellationReason { get; set; }
}