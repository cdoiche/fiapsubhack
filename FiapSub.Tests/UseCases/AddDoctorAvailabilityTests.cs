using FiapSub.Core.Entities;
using FiapSub.Core.Interfaces;
using FiapSub.Core.UseCases.Availability;
using Moq;

namespace FiapSub.Tests.UseCases;

public class AddDoctorAvailabilityTests
{
    [Fact]
    public async Task Execute_ValidData_ShouldAddAvailability()
    {
        var mockRepository = new Mock<IDoctorAvailabilityRepository>();
        var useCase = new AddDoctorAvailabilityUseCase(mockRepository.Object);

        int doctorId = 1;
        DateTime start = DateTime.UtcNow.AddHours(5);
        DateTime end = start.AddHours(1);

        mockRepository
            .Setup(repo => repo.ExistsAsync(doctorId, start, end))
            .ReturnsAsync(false);

        await useCase.ExecuteAsync(doctorId, start, end);

        mockRepository.Verify(repo => repo.AddAvailabilityAsync(It.Is<DoctorAvailability>(
            a => a.DoctorId == doctorId && a.StartTime == start && a.EndTime == end)), Times.Once);
    }

    [Fact]
    public async Task Execute_InvalidDoctorId_ShouldThrowArgumentException()
    {
        var mockRepository = new Mock<IDoctorAvailabilityRepository>();
        var useCase = new AddDoctorAvailabilityUseCase(mockRepository.Object);

        int doctorId = 0;
        DateTime start = DateTime.UtcNow.AddHours(5);
        DateTime end = start.AddHours(1);

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecuteAsync(doctorId, start, end));
        mockRepository.Verify(repo => repo.AddAvailabilityAsync(It.IsAny<DoctorAvailability>()), Times.Never);
    }

    [Fact]
    public async Task Execute_StartGreaterThanEnd_ShouldThrowArgumentException()
    {
        var mockRepository = new Mock<IDoctorAvailabilityRepository>();
        var useCase = new AddDoctorAvailabilityUseCase(mockRepository.Object);

        int doctorId = 1;
        DateTime start = DateTime.UtcNow.AddHours(5);
        DateTime end = start.AddHours(-1);

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecuteAsync(doctorId, start, end));
        mockRepository.Verify(repo => repo.AddAvailabilityAsync(It.IsAny<DoctorAvailability>()), Times.Never);
    }

    [Fact]
    public async Task Execute_StartLessThan4HoursFromNow_ShouldThrowArgumentException()
    {
        var mockRepository = new Mock<IDoctorAvailabilityRepository>();
        var useCase = new AddDoctorAvailabilityUseCase(mockRepository.Object);

        int doctorId = 1;
        DateTime start = DateTime.UtcNow.AddHours(3);
        DateTime end = start.AddHours(1);

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecuteAsync(doctorId, start, end));
        mockRepository.Verify(repo => repo.AddAvailabilityAsync(It.IsAny<DoctorAvailability>()), Times.Never);
    }

    [Fact]
    public async Task Execute_EndGreaterThan1HourFromStart_ShouldThrowArgumentException()
    {
        var mockRepository = new Mock<IDoctorAvailabilityRepository>();
        var useCase = new AddDoctorAvailabilityUseCase(mockRepository.Object);

        int doctorId = 1;
        DateTime start = DateTime.UtcNow.AddHours(5);
        DateTime end = start.AddHours(2);

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecuteAsync(doctorId, start, end));
        mockRepository.Verify(repo => repo.AddAvailabilityAsync(It.IsAny<DoctorAvailability>()), Times.Never);
    }

    [Fact]
    public async Task Execute_AvailabilityAlreadyExists_ShouldThrowInvalidOperationException()
    {
        var mockRepository = new Mock<IDoctorAvailabilityRepository>();
        var useCase = new AddDoctorAvailabilityUseCase(mockRepository.Object);

        int doctorId = 1;
        DateTime start = DateTime.UtcNow.AddHours(5);
        DateTime end = start.AddHours(1);

        mockRepository
            .Setup(repo => repo.ExistsAsync(doctorId, start, end))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecuteAsync(doctorId, start, end));
        mockRepository.Verify(repo => repo.AddAvailabilityAsync(It.IsAny<DoctorAvailability>()), Times.Never);
    }
}