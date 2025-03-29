namespace FiapSub.API.DTOs;

public class RejectAppointmentRequest
{
    public int AppointmentId { get; set; }
    public string? RejectionReason { get; set; }
}