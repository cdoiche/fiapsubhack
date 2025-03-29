using FiapSub.Core.Entities;
using FiapSub.Core.Enums;
using FiapSub.Core.Interfaces;
using FiapSub.Core.UseCases.Doctors;
using Moq;

namespace FiapSub.Tests.UseCases;

public class SearchDoctorsBySpecialtyTests
{
    [Fact]
    public async Task Execute_ValidSpecialty_ShouldReturnDoctors()
    {
        var mockDoctorRepository = new Mock<IDoctorRepository>();
        var useCase = new SearchDoctorsBySpecialtyUseCase(mockDoctorRepository.Object);

        Specialty specialty = Specialty.Cardiologist;
        var doctors = new List<Doctor>
        {
            new Doctor("Dr. John Doe", "john.doe@example.com", "password", "123456789", "122345", specialty),
            new Doctor("Dr. Jane Smith", "jane.smith@example.com", "password", "987654321", "122342", specialty)
        };

        mockDoctorRepository
            .Setup(repo => repo.SearchBySpecialtyAsync(specialty))
            .ReturnsAsync(doctors);

        var result = await useCase.ExecuteAsync(specialty);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        mockDoctorRepository.Verify(repo => repo.SearchBySpecialtyAsync(specialty), Times.Once);
    }

    [Fact]
    public async Task Execute_EmptySpecialty_ShouldThrowArgumentException()
    {
        var mockDoctorRepository = new Mock<IDoctorRepository>();
        var useCase = new SearchDoctorsBySpecialtyUseCase(mockDoctorRepository.Object);
        
        Specialty invalidSpecialty = (Specialty)(-1);

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecuteAsync(invalidSpecialty));
        mockDoctorRepository.Verify(repo => repo.SearchBySpecialtyAsync(It.IsAny<Specialty>()), Times.Never);
    }
}