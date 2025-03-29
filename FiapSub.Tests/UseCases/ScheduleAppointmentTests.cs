using FiapSub.Core.Entities;
using FiapSub.Core.Enums;
using FiapSub.Core.Interfaces;
using FiapSub.Core.UseCases.Appointments;
using Moq;

namespace FiapSub.Tests.UseCases;

public class ScheduleAppointmentTests
{
    [Fact]
    public async Task Execute_ValidData_ShouldScheduleAppointment()
    {
        var mockAppointmentRepository = new Mock<IAppointmentRepository>();
        var mockDoctorAvailabilityRepository = new Mock<IDoctorAvailabilityRepository>();
        var mockNotificationService = new Mock<INotificationService>();
        var mockPatientRepository = new Mock<IPatientRepository>();
        var mockDoctorRepository = new Mock<IDoctorRepository>();

        var useCase = new ScheduleAppointmentUseCase(
            mockAppointmentRepository.Object,
            mockDoctorAvailabilityRepository.Object,
            mockNotificationService.Object,
            mockPatientRepository.Object,
            mockDoctorRepository.Object
        );

        int patientId = 1;
        int availabilityId = 1;
        var availability = new DoctorAvailability(2, DateTime.UtcNow.AddHours(5), DateTime.UtcNow.AddHours(6));
        var patient = new Patient("John Doe", "john.doe@example.com", "password", "123456789", "12345678909");
        var doctor = new Doctor("Dr. Smith", "dr.smith@example.com", "password", "987654321", "12345678901", Specialty.Generalist);

        mockDoctorAvailabilityRepository
            .Setup(repo => repo.GetAvailabilityByIdAsync(availabilityId))
            .ReturnsAsync(availability);

        mockPatientRepository
            .Setup(repo => repo.GetByIdAsync(patientId))
            .ReturnsAsync(patient);

        mockDoctorRepository
            .Setup(repo => repo.GetByIdAsync(availability.DoctorId))
            .ReturnsAsync(doctor);

        await useCase.ExecuteAsync(patientId, availabilityId);

        mockDoctorAvailabilityRepository.Verify(repo => repo.UpdateAvailabilityAsync(It.Is<DoctorAvailability>(a => !a.IsAvailable)), Times.Once);
        mockAppointmentRepository.Verify(repo => repo.AddAsync(It.IsAny<Appointment>()), Times.Once);
        mockNotificationService.Verify(service => service.NotifyAppointmentScheduledAsync(It.IsAny<int>(), patient.Email), Times.Once);
        mockNotificationService.Verify(service => service.NotifyAppointmentScheduledAsync(It.IsAny<int>(), doctor.Email), Times.Once);
    }

    [Fact]
    public async Task Execute_InvalidPatientId_ShouldThrowArgumentException()
    {
        var mockDoctorAvailabilityRepository = new Mock<IDoctorAvailabilityRepository>();
        var useCase = new ScheduleAppointmentUseCase(
            Mock.Of<IAppointmentRepository>(),
            mockDoctorAvailabilityRepository.Object,
            Mock.Of<INotificationService>(),
            Mock.Of<IPatientRepository>(),
            Mock.Of<IDoctorRepository>()
        );

        int patientId = 0;
        int availabilityId = 1;

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecuteAsync(patientId, availabilityId));
        mockDoctorAvailabilityRepository.Verify(repo => repo.GetAvailabilityByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Execute_InvalidAvailabilityId_ShouldThrowArgumentException()
    {
        var mockDoctorAvailabilityRepository = new Mock<IDoctorAvailabilityRepository>();
        var useCase = new ScheduleAppointmentUseCase(
            Mock.Of<IAppointmentRepository>(),
            mockDoctorAvailabilityRepository.Object,
            Mock.Of<INotificationService>(),
            Mock.Of<IPatientRepository>(),
            Mock.Of<IDoctorRepository>()
        );

        int patientId = 1;
        int availabilityId = 0;

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecuteAsync(patientId, availabilityId));
        mockDoctorAvailabilityRepository.Verify(repo => repo.GetAvailabilityByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Execute_InvalidAvailability_ShouldThrowInvalidOperationException()
    {
        var mockDoctorAvailabilityRepository = new Mock<IDoctorAvailabilityRepository>();
        var useCase = new ScheduleAppointmentUseCase(
            Mock.Of<IAppointmentRepository>(),
            mockDoctorAvailabilityRepository.Object,
            Mock.Of<INotificationService>(),
            Mock.Of<IPatientRepository>(),
            Mock.Of<IDoctorRepository>()
        );

        int patientId = 1;
        int availabilityId = 1;

        mockDoctorAvailabilityRepository
            .Setup(repo => repo.GetAvailabilityByIdAsync(availabilityId))
            .ReturnsAsync((DoctorAvailability)null);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecuteAsync(patientId, availabilityId));
        mockDoctorAvailabilityRepository.Verify(repo => repo.GetAvailabilityByIdAsync(availabilityId), Times.Once);
    }

    [Fact]
    public async Task Execute_AvailabilityStartTimeLessThan4Hours_ShouldThrowArgumentException()
    {
        var mockDoctorAvailabilityRepository = new Mock<IDoctorAvailabilityRepository>();
        var useCase = new ScheduleAppointmentUseCase(
            Mock.Of<IAppointmentRepository>(),
            mockDoctorAvailabilityRepository.Object,
            Mock.Of<INotificationService>(),
            Mock.Of<IPatientRepository>(),
            Mock.Of<IDoctorRepository>()
        );

        int patientId = 1;
        int availabilityId = 1;
        var availability = new DoctorAvailability(2, DateTime.UtcNow.AddHours(3), DateTime.UtcNow.AddHours(4));

        mockDoctorAvailabilityRepository
            .Setup(repo => repo.GetAvailabilityByIdAsync(availabilityId))
            .ReturnsAsync(availability);

        await Assert.ThrowsAsync<NullReferenceException>(() => useCase.ExecuteAsync(patientId, availabilityId));
        mockDoctorAvailabilityRepository.Verify(repo => repo.GetAvailabilityByIdAsync(availabilityId), Times.Once);
    }
}