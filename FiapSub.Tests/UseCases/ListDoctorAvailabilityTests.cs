using FiapSub.Core.Entities;
using FiapSub.Core.Interfaces;
using FiapSub.Core.UseCases.Availability;
using Moq;

namespace FiapSub.Tests.UseCases;

public class ListDoctorAvailabilityTests
{
    [Fact]
    public async Task Execute_ValidDoctorId_ShouldReturnAvailabilities()
    {
        var mockRepository = new Mock<IDoctorAvailabilityRepository>();
        var useCase = new ListDoctorAvailabilityUseCase(mockRepository.Object);

        int doctorId = 1;
        var availabilities = new List<DoctorAvailability>
        {
            new DoctorAvailability(doctorId, DateTime.UtcNow.AddHours(5), DateTime.UtcNow.AddHours(6)),
            new DoctorAvailability(doctorId, DateTime.UtcNow.AddHours(7), DateTime.UtcNow.AddHours(8))
        };

        mockRepository
            .Setup(repo => repo.GetAvailableSlotsAsync(doctorId))
            .ReturnsAsync(availabilities);

        var result = await useCase.ExecuteAsync(doctorId);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        mockRepository.Verify(repo => repo.GetAvailableSlotsAsync(doctorId), Times.Once);
    }

    [Fact]
    public async Task Execute_InvalidDoctorId_ShouldThrowArgumentException()
    {
        var mockRepository = new Mock<IDoctorAvailabilityRepository>();
        var useCase = new ListDoctorAvailabilityUseCase(mockRepository.Object);

        int doctorId = 0;

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecuteAsync(doctorId));
        mockRepository.Verify(repo => repo.GetAvailableSlotsAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Execute_NoAvailabilities_ShouldThrowKeyNotFoundException()
    {
        var mockRepository = new Mock<IDoctorAvailabilityRepository>();
        var useCase = new ListDoctorAvailabilityUseCase(mockRepository.Object);

        int doctorId = 1;

        mockRepository
            .Setup(repo => repo.GetAvailableSlotsAsync(doctorId))
            .ReturnsAsync(new List<DoctorAvailability>());

        await Assert.ThrowsAsync<KeyNotFoundException>(() => useCase.ExecuteAsync(doctorId));
        mockRepository.Verify(repo => repo.GetAvailableSlotsAsync(doctorId), Times.Once);
    }
}