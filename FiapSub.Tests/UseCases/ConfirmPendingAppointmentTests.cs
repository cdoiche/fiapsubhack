using FiapSub.Core.Entities;
using FiapSub.Core.Enums;
using FiapSub.Core.Interfaces;
using FiapSub.Core.UseCases.Appointments;
using Moq;

namespace FiapSub.Tests.UseCases;

public class ConfirmPendingAppointmentTests
{
    [Fact]
    public async Task Execute_ValidData_ShouldConfirmAppointment()
    {
        var mockAppointmentRepository = new Mock<IAppointmentRepository>();
        var mockNotificationService = new Mock<INotificationService>();
        var mockPatientRepository = new Mock<IPatientRepository>();

        var useCase = new ConfirmPendingAppointmentUseCase(
            mockAppointmentRepository.Object,
            mockNotificationService.Object,
            mockPatientRepository.Object
        );

        int doctorId = 1;
        int appointmentId = 1;
        var appointment = new Appointment(2, doctorId, DateTime.UtcNow);
        var patient = new Patient("John Doe", "john.doe@example.com", "password", "123456789", "12345678909");

        mockAppointmentRepository
            .Setup(repo => repo.GetByIdAsync(appointmentId))
            .ReturnsAsync(appointment);

        mockPatientRepository
            .Setup(repo => repo.GetByIdAsync(appointment.PatientId))
            .ReturnsAsync(patient);

        await useCase.ExecuteAsync(doctorId, appointmentId);

        mockAppointmentRepository.Verify(repo => repo.UpdateAsync(It.Is<Appointment>(a => a.Status == AppointmentStatus.Confirmed)), Times.Once);
        mockNotificationService.Verify(service => service.NotifyAppointmentConfirmedAsync(appointment.Id, patient.Email), Times.Once);
    }

    [Fact]
    public async Task Execute_InvalidDoctorId_ShouldThrowArgumentException()
    {
        var useCase = new ConfirmPendingAppointmentUseCase(
            Mock.Of<IAppointmentRepository>(),
            Mock.Of<INotificationService>(),
            Mock.Of<IPatientRepository>()
        );

        int doctorId = 0;
        int appointmentId = 1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecuteAsync(doctorId, appointmentId));
    }

    [Fact]
    public async Task Execute_InvalidAppointmentId_ShouldThrowArgumentException()
    {
        var useCase = new ConfirmPendingAppointmentUseCase(
            Mock.Of<IAppointmentRepository>(),
            Mock.Of<INotificationService>(),
            Mock.Of<IPatientRepository>()
        );

        int doctorId = 1;
        int appointmentId = 0;

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecuteAsync(doctorId, appointmentId));
    }

    [Fact]
    public async Task Execute_AppointmentNotFound_ShouldThrowKeyNotFoundException()
    {
        var mockAppointmentRepository = new Mock<IAppointmentRepository>();
        var useCase = new ConfirmPendingAppointmentUseCase(
            mockAppointmentRepository.Object,
            Mock.Of<INotificationService>(),
            Mock.Of<IPatientRepository>()
        );

        int doctorId = 1;
        int appointmentId = 1;

        mockAppointmentRepository
            .Setup(repo => repo.GetByIdAsync(appointmentId))
            .ReturnsAsync((Appointment)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => useCase.ExecuteAsync(doctorId, appointmentId));
    }

    [Fact]
    public async Task Execute_DoctorIdMismatch_ShouldThrowUnauthorizedAccessException()
    {
        var mockAppointmentRepository = new Mock<IAppointmentRepository>();
        var useCase = new ConfirmPendingAppointmentUseCase(
            mockAppointmentRepository.Object,
            Mock.Of<INotificationService>(),
            Mock.Of<IPatientRepository>()
        );

        int doctorId = 1;
        int appointmentId = 1;
        var appointment = new Appointment(2, 2, DateTime.UtcNow);

        mockAppointmentRepository
            .Setup(repo => repo.GetByIdAsync(appointmentId))
            .ReturnsAsync(appointment);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => useCase.ExecuteAsync(doctorId, appointmentId));
    }

    [Fact]
    public async Task Execute_AppointmentNotPending_ShouldThrowInvalidOperationException()
    {
        var mockAppointmentRepository = new Mock<IAppointmentRepository>();
        var useCase = new ConfirmPendingAppointmentUseCase(
            mockAppointmentRepository.Object,
            Mock.Of<INotificationService>(),
            Mock.Of<IPatientRepository>()
        );

        int doctorId = 1;
        int appointmentId = 1;
        var appointment = new Appointment(2, doctorId, DateTime.UtcNow);
        appointment.Confirm();

        mockAppointmentRepository
            .Setup(repo => repo.GetByIdAsync(appointmentId))
            .ReturnsAsync(appointment);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecuteAsync(doctorId, appointmentId));
    }
}