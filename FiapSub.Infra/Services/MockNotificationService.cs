using FiapSub.Core.Interfaces;

namespace FiapSub.Infra.Services;

public class MockNotificationService : INotificationService
{
    public Task NotifyAppointmentCancelledAsync(int appointmentId, string recipientEmail, string? reason)
    {
        Console.WriteLine($"[Mock] Appointment {appointmentId} cancelled. Notification sent to {recipientEmail}. Reason: {reason}.");
        return Task.CompletedTask;
    }

    public Task NotifyAppointmentConfirmedAsync(int appointmentId, string recipientEmail)
    {
        Console.WriteLine($"[Mock] Appointment {appointmentId} confirmed. Notification sent to {recipientEmail}.");
        return Task.CompletedTask;
    }

    public Task NotifyAppointmentScheduledAsync(int appointmentId, string recipientEmail)
    {
        Console.WriteLine($"[Mock] Appointment {appointmentId} scheduled. Notification sent to {recipientEmail}.");
        return Task.CompletedTask;
    }
}