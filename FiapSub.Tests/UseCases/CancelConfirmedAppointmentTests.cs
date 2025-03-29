using FiapSub.Core.Entities;
using FiapSub.Core.Enums;
using FiapSub.Core.Interfaces;
using FiapSub.Core.UseCases.Appointments;
using Moq;

namespace FiapSub.Tests.UseCases;

public class CancelConfirmedAppointmentTests
{
    [Fact]
    public async Task Execute_ValidData_ShouldCancelAppointment()
    {
        var mockAppointmentRepository = new Mock<IAppointmentRepository>();
        var mockNotificationService = new Mock<INotificationService>();
        var mockPatientRepository = new Mock<IPatientRepository>();
        var mockDoctorRepository = new Mock<IDoctorRepository>();

        var useCase = new CancelConfirmedAppointmentUseCase(
            mockAppointmentRepository.Object,
            mockNotificationService.Object,
            mockPatientRepository.Object,
            mockDoctorRepository.Object
        );

        int userId = 1;
        int appointmentId = 1;
        string userType = "Doctor";
        string cancellationReason = "Doctor unavailable";

        var appointment = new Appointment(2, userId, DateTime.UtcNow);
        appointment.Confirm();
        var patient = new Patient("John Doe", "john.doe@example.com", "password", "123456789", "12345678909");
        var doctor = new Doctor("Dr. Smith", "dr.smith@example.com", "password", "987654321", "12345678901", Specialty.Generalist);

        mockAppointmentRepository
            .Setup(repo => repo.GetByIdAsync(appointmentId))
            .ReturnsAsync(appointment);

        mockPatientRepository
            .Setup(repo => repo.GetByIdAsync(appointment.PatientId))
            .ReturnsAsync(patient);

        mockDoctorRepository
            .Setup(repo => repo.GetByIdAsync(appointment.DoctorId))
            .ReturnsAsync(doctor);

        await useCase.ExecuteAsync(userId, appointmentId, userType, cancellationReason);

        mockAppointmentRepository.Verify(repo => repo.UpdateAsync(It.Is<Appointment>(a => a.Status == AppointmentStatus.Cancelled)), Times.Once);
        mockNotificationService.Verify(service => service.NotifyAppointmentCancelledAsync(appointment.Id, patient.Email, cancellationReason), Times.Once);
        mockNotificationService.Verify(service => service.NotifyAppointmentCancelledAsync(appointment.Id, doctor.Email, cancellationReason), Times.Once);
    }

    [Fact]
    public async Task Execute_InvalidUserId_ShouldThrowArgumentException()
    {
        var useCase = new CancelConfirmedAppointmentUseCase(
            Mock.Of<IAppointmentRepository>(),
            Mock.Of<INotificationService>(),
            Mock.Of<IPatientRepository>(),
            Mock.Of<IDoctorRepository>()
        );

        int userId = 0;
        int appointmentId = 1;
        string userType = "Doctor";

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecuteAsync(userId, appointmentId, userType));
    }

    [Fact]
    public async Task Execute_InvalidAppointmentId_ShouldThrowArgumentException()
    {
        var useCase = new CancelConfirmedAppointmentUseCase(
            Mock.Of<IAppointmentRepository>(),
            Mock.Of<INotificationService>(),
            Mock.Of<IPatientRepository>(),
            Mock.Of<IDoctorRepository>()
        );

        int userId = 1;
        int appointmentId = 0;
        string userType = "Doctor";

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecuteAsync(userId, appointmentId, userType));
    }

    [Fact]
    public async Task Execute_AppointmentNotFound_ShouldThrowKeyNotFoundException()
    {
        var mockAppointmentRepository = new Mock<IAppointmentRepository>();
        var useCase = new CancelConfirmedAppointmentUseCase(
            mockAppointmentRepository.Object,
            Mock.Of<INotificationService>(),
            Mock.Of<IPatientRepository>(),
            Mock.Of<IDoctorRepository>()
        );

        int userId = 1;
        int appointmentId = 1;
        string userType = "Doctor";

        mockAppointmentRepository
            .Setup(repo => repo.GetByIdAsync(appointmentId))
            .ReturnsAsync((Appointment)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => useCase.ExecuteAsync(userId, appointmentId, userType));
    }

    [Fact]
    public async Task Execute_AppointmentNotConfirmed_ShouldThrowInvalidOperationException()
    {
        var mockAppointmentRepository = new Mock<IAppointmentRepository>();
        var useCase = new CancelConfirmedAppointmentUseCase(
            mockAppointmentRepository.Object,
            Mock.Of<INotificationService>(),
            Mock.Of<IPatientRepository>(),
            Mock.Of<IDoctorRepository>()
        );

        int userId = 1;
        int appointmentId = 1;
        string userType = "Doctor";
        var appointment = new Appointment(2, userId, DateTime.UtcNow);

        mockAppointmentRepository
            .Setup(repo => repo.GetByIdAsync(appointmentId))
            .ReturnsAsync(appointment);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecuteAsync(userId, appointmentId, userType));
    }

    [Fact]
    public async Task Execute_UnauthorizedUser_ShouldThrowUnauthorizedAccessException()
    {
        var mockAppointmentRepository = new Mock<IAppointmentRepository>();
        var useCase = new CancelConfirmedAppointmentUseCase(
            mockAppointmentRepository.Object,
            Mock.Of<INotificationService>(),
            Mock.Of<IPatientRepository>(),
            Mock.Of<IDoctorRepository>()
        );

        int userId = 1;
        int appointmentId = 1;
        string userType = "Doctor";
        var appointment = new Appointment(2, 2, DateTime.UtcNow);
        appointment.Confirm();

        mockAppointmentRepository
            .Setup(repo => repo.GetByIdAsync(appointmentId))
            .ReturnsAsync(appointment);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => useCase.ExecuteAsync(userId, appointmentId, userType));
    }
}