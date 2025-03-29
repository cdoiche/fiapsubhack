using FiapSub.Core.Entities;

namespace FiapSub.Tests.Entities;

public class DoctorAvailabilityTests
{
    [Fact]
    public void Constructor_ShouldInitializeCorrectly()
    {
        int doctorId = 1;
        DateTime startTime = DateTime.UtcNow.AddHours(5);
        DateTime endTime = startTime.AddHours(1);

        var availability = new DoctorAvailability(doctorId, startTime, endTime);

        Assert.Equal(doctorId, availability.DoctorId);
        Assert.Equal(startTime, availability.StartTime);
        Assert.Equal(endTime, availability.EndTime);
        Assert.True(availability.IsAvailable);
    }

    [Fact]
    public void Block_ShouldSetIsAvailableToFalse()
    {
        var availability = new DoctorAvailability(1, DateTime.UtcNow.AddHours(5), DateTime.UtcNow.AddHours(6));

        availability.Block();

        Assert.False(availability.IsAvailable);
    }

    [Fact]
    public void Block_ShouldThrowInvalidOperationException_WhenAlreadyBlocked()
    {
        var availability = new DoctorAvailability(1, DateTime.UtcNow.AddHours(5), DateTime.UtcNow.AddHours(6));
        availability.Block();

        Assert.False(availability.IsAvailable);
    }

    [Fact]
    public void Unblock_ShouldSetIsAvailableToTrue()
    {
        var availability = new DoctorAvailability(1, DateTime.UtcNow.AddHours(5), DateTime.UtcNow.AddHours(6));
        availability.Block();

        availability.Unblock();

        Assert.True(availability.IsAvailable);
    }

    [Fact]
    public void Unblock_ShouldThrowInvalidOperationException_WhenAlreadyAvailable()
    {
        var availability = new DoctorAvailability(1, DateTime.UtcNow.AddHours(5), DateTime.UtcNow.AddHours(6));

        Assert.True(availability.IsAvailable);
    }
}