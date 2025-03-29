using FiapSub.Core.Entities;
using FiapSub.Core.Enums;

namespace FiapSub.Tests.Entities;

public class AppointmentTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithPendingStatus()
    {
        int patientId = 1;
        int doctorId = 2;
        DateTime dateTime = DateTime.UtcNow;

        var appointment = new Appointment(patientId, doctorId, dateTime);

        Assert.Equal(patientId, appointment.PatientId);
        Assert.Equal(doctorId, appointment.DoctorId);
        Assert.Equal(dateTime, appointment.DateTime);
        Assert.Equal(AppointmentStatus.Pending, appointment.Status);
        Assert.Equal(string.Empty, appointment.Description);
    }

    [Fact]
    public void Confirm_ShouldSetStatusToConfirmed_WhenStatusIsPending()
    {
        var appointment = new Appointment(1, 2, DateTime.UtcNow);

        appointment.Confirm();

        Assert.Equal(AppointmentStatus.Confirmed, appointment.Status);
    }

    [Fact]
    public void Confirm_ShouldThrowInvalidOperationException_WhenStatusIsNotPending()
    {
        var appointment = new Appointment(1, 2, DateTime.UtcNow);
        appointment.Confirm();

        Assert.Throws<InvalidOperationException>(() => appointment.Confirm());
    }

    [Fact]
    public void Reject_ShouldSetStatusToCancelledAndSetReason_WhenStatusIsPending()
    {
        var appointment = new Appointment(1, 2, DateTime.UtcNow);
        string reason = "Patient unavailable";

        appointment.Reject(reason);

        Assert.Equal(AppointmentStatus.Cancelled, appointment.Status);
        Assert.Equal(reason, appointment.Description);
    }

    [Fact]
    public void Reject_ShouldSetStatusToCancelledWithoutReason_WhenStatusIsPending()
    {
        var appointment = new Appointment(1, 2, DateTime.UtcNow);

        appointment.Reject(null);

        Assert.Equal(AppointmentStatus.Cancelled, appointment.Status);
        Assert.Equal(string.Empty, appointment.Description);
    }

    [Fact]
    public void Reject_ShouldThrowInvalidOperationException_WhenStatusIsNotPending()
    {
        var appointment = new Appointment(1, 2, DateTime.UtcNow);
        appointment.Confirm();

        Assert.Throws<InvalidOperationException>(() => appointment.Reject("Reason"));
    }

    [Fact]
    public void Cancel_ShouldSetStatusToCancelledAndSetReason_WhenStatusIsNotCancelled()
    {
        var appointment = new Appointment(1, 2, DateTime.UtcNow);
        string reason = "Doctor unavailable";

        appointment.Cancel(reason);

        Assert.Equal(AppointmentStatus.Cancelled, appointment.Status);
        Assert.Equal(reason, appointment.Description);
    }

    [Fact]
    public void Cancel_ShouldSetStatusToCancelledWithoutReason_WhenStatusIsNotCancelled()
    {
        var appointment = new Appointment(1, 2, DateTime.UtcNow);

        appointment.Cancel(null);

        Assert.Equal(AppointmentStatus.Cancelled, appointment.Status);
        Assert.Equal(string.Empty, appointment.Description);
    }

    [Fact]
    public void Cancel_ShouldThrowInvalidOperationException_WhenStatusIsAlreadyCancelled()
    {
        var appointment = new Appointment(1, 2, DateTime.UtcNow);
        appointment.Cancel("Reason");

        Assert.Throws<InvalidOperationException>(() => appointment.Cancel("Another reason"));
    }
}