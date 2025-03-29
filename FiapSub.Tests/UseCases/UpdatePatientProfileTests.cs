using FiapSub.Core.Entities;
using FiapSub.Core.Interfaces;
using FiapSub.Core.UseCases.Patients;
using Moq;

namespace FiapSub.Tests.UseCases;

public class UpdatePatientProfileTests
{
    [Fact]
    public async Task Execute_ValidData_ShouldUpdatePatientProfile()
    {
        var mockPatientRepository = new Mock<IPatientRepository>();
        var useCase = new UpdatePatientProfileUseCase(mockPatientRepository.Object);

        int patientId = 1;
        string updatedName = "Updated Name";
        string updatedPhone = "11999999999";

        var patient = new Patient("Original Name", "email@example.com", "password", "11888888888", "12345678909");

        mockPatientRepository
            .Setup(repo => repo.GetByIdAsync(patientId))
            .ReturnsAsync(patient);

        await useCase.ExecuteAsync(patientId, updatedName, updatedPhone);

        mockPatientRepository.Verify(repo => repo.UpdateAsync(It.Is<Patient>(p => p.Name == updatedName && p.Phone == updatedPhone)), Times.Once);
    }

    [Fact]
    public async Task Execute_PatientNotFound_ShouldThrowKeyNotFoundException()
    {
        var mockPatientRepository = new Mock<IPatientRepository>();
        var useCase = new UpdatePatientProfileUseCase(mockPatientRepository.Object);

        int patientId = 1;
        string updatedName = "Updated Name";
        string updatedPhone = "11999999999";

        mockPatientRepository
            .Setup(repo => repo.GetByIdAsync(patientId))
            .ReturnsAsync((Patient)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => useCase.ExecuteAsync(patientId, updatedName, updatedPhone));
        mockPatientRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Patient>()), Times.Never);
    }

    [Fact]
    public async Task Execute_EmptyPatientName_ShouldNotUpdateNameNorUpdatePhone()
    {
        var mockPatientRepository = new Mock<IPatientRepository>();
        var useCase = new UpdatePatientProfileUseCase(mockPatientRepository.Object);

        int patientId = 1;
        string updatedName = "";
        string updatedPhone = "11999999999";

        var patient = new Patient("Original Name", "email@example.com", "password", "11888888888", "12345678909");

        mockPatientRepository
            .Setup(repo => repo.GetByIdAsync(patientId))
            .ReturnsAsync(patient);

        await useCase.ExecuteAsync(patientId, updatedName, updatedPhone);
        mockPatientRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Patient>()), Times.Never);
    }
}