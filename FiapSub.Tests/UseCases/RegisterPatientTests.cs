using FiapSub.Core.Entities;
using FiapSub.Core.Interfaces;
using FiapSub.Core.UseCases.Patients;
using Moq;

namespace FiapSub.Tests.UseCases;

public class RegisterPatientTests
{
    [Fact]
    public async Task Execute_ValidData_ShouldAddPatient()
    {
        var mockPatientRepository = new Mock<IPatientRepository>();
        var useCase = new RegisterPatientUseCase(mockPatientRepository.Object);

        string name = "Paciente 1";
        string email = "paciente1@gmail.com";
        string password = "senhapaciente1";
        string phone = "11999999999";
        string cpf = "12345678909";

        var patient = new Patient(name, email, password, phone, cpf);

        await useCase.ExecuteAsync(patient);

        mockPatientRepository.Verify(repo => repo.AddAsync(It.Is<Patient>(p => p.Name == name && p.Email == email && p.Phone == phone && p.CPF == cpf)), Times.Once);
    }

    [Fact]
    public async Task Execute_EmailAlreadyExists_ShouldThrowInvalidOperationException()
    {
        var mockPatientRepository = new Mock<IPatientRepository>();
        var useCase = new RegisterPatientUseCase(mockPatientRepository.Object);

        string name = "Paciente 1";
        string email = "paciente1@gmail.com";
        string password = "senhapaciente1";
        string phone = "11999999999";
        string cpf = "12345678909";

        var patient = new Patient(name, email, password, phone, cpf);

        mockPatientRepository
            .Setup(repo => repo.ExistsWithEmailAsync(email))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecuteAsync(patient));
        mockPatientRepository.Verify(repo => repo.AddAsync(It.IsAny<Patient>()), Times.Never);
    }
}