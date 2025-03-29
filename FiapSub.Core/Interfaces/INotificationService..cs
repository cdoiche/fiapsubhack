namespace FiapSub.Core.Interfaces;

public interface INotificationService
{
    Task NotifyAppointmentScheduledAsync(int appointmentId, string recipientEmail);
    Task NotifyAppointmentConfirmedAsync(int appointmentId, string recipientEmail);
    Task NotifyAppointmentCancelledAsync(int appointmentId, string recipientEmail, string? message);
}