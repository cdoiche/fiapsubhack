using FiapSub.Core.Entities;
using FiapSub.Core.Interfaces;
using FiapSub.Core.UseCases.Appointments;
using Moq;

namespace FiapSub.Tests.UseCases;

public class ListDoctorPendingAppointmentsTests
{
    [Fact]
    public async Task Execute_ValidDoctorId_ShouldReturnPendingAppointments()
    {
        var mockAppointmentRepository = new Mock<IAppointmentRepository>();
        var useCase = new ListDoctorPendingAppointmentsUseCase(mockAppointmentRepository.Object);

        int doctorId = 1;
        var pendingAppointments = new List<Appointment>
        {
            new Appointment(1, doctorId, DateTime.UtcNow.AddHours(1)),
            new Appointment(2, doctorId, DateTime.UtcNow.AddHours(2))
        };

        mockAppointmentRepository
            .Setup(repo => repo.GetPendingAppointmentsByDoctorAsync(doctorId))
            .ReturnsAsync(pendingAppointments);

        var result = await useCase.ExecuteAsync(doctorId);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        mockAppointmentRepository.Verify(repo => repo.GetPendingAppointmentsByDoctorAsync(doctorId), Times.Once);
    }

    [Fact]
    public async Task Execute_InvalidDoctorId_ShouldThrowArgumentException()
    {
        var mockAppointmentRepository = new Mock<IAppointmentRepository>();
        var useCase = new ListDoctorPendingAppointmentsUseCase(mockAppointmentRepository.Object);

        int doctorId = 0;

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecuteAsync(doctorId));
        mockAppointmentRepository.Verify(repo => repo.GetPendingAppointmentsByDoctorAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Execute_NoPendingAppointments_ShouldReturnEmptyList()
    {
        var mockAppointmentRepository = new Mock<IAppointmentRepository>();
        var useCase = new ListDoctorPendingAppointmentsUseCase(mockAppointmentRepository.Object);

        int doctorId = 1;

        mockAppointmentRepository
            .Setup(repo => repo.GetPendingAppointmentsByDoctorAsync(doctorId))
            .ReturnsAsync(new List<Appointment>());

        var result = await useCase.ExecuteAsync(doctorId);

        Assert.NotNull(result);
        Assert.Empty(result);
        mockAppointmentRepository.Verify(repo => repo.GetPendingAppointmentsByDoctorAsync(doctorId), Times.Once);
    }
}