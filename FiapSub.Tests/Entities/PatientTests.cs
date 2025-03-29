using FiapSub.Core.Entities;

namespace FiapSub.Tests.Entities;

public class PatientTests
{
    [Fact]
    public void Constructor_ShouldInitializeCorrectly()
    {
        string name = "John Doe";
        string email = "john.doe@example.com";
        string password = "password123";
        string phone = "11999999999";
        string cpf = "12345678909";

        var patient = new Patient(name, email, password, phone, cpf);

        Assert.Equal(name, patient.Name);
        Assert.Equal(email, patient.Email);
        Assert.Equal("hashed_" + password, patient.PasswordHash);
        Assert.Equal(phone, patient.Phone);
        Assert.Equal(cpf, patient.CPF);
    }

    [Fact]
    public void UpdateProfile_ShouldUpdateNameAndPhone()
    {
        var patient = new Patient("John Doe", "john.doe@example.com", "password123", "11999999999", "12345678909");
        string updatedName = "Jane Doe";
        string updatedPhone = "11888888888";

        patient.UpdateProfile(updatedName, updatedPhone);

        Assert.Equal(updatedName, patient.Name);
        Assert.Equal(updatedPhone, patient.Phone);
    }

    [Fact]
    public void UpdateProfile_ShouldNotUpdateNameIfNullOrEmpty()
    {
        var patient = new Patient("John Doe", "john.doe@example.com", "password123", "11999999999", "12345678909");
        string updatedName = "";
        string updatedPhone = "11888888888";

        patient.UpdateProfile(updatedName, updatedPhone);

        Assert.Equal("John Doe", patient.Name);
        Assert.Equal(updatedPhone, patient.Phone);
    }

    [Fact]
    public void UpdateProfile_ShouldNotUpdatePhoneIfNullOrEmpty()
    {
        var patient = new Patient("John Doe", "john.doe@example.com", "password123", "11999999999", "12345678909");
        string updatedName = "Jane Doe";
        string updatedPhone = "";

        patient.UpdateProfile(updatedName, updatedPhone);

        Assert.Equal(updatedName, patient.Name);
        Assert.Equal("11999999999", patient.Phone);
    }

    [Fact]
    public void UpdateProfile_ShouldNotUpdateAnythingIfBothNameAndPhoneAreEmpty()
    {
        var patient = new Patient("John Doe", "john.doe@example.com", "password123", "11999999999", "12345678909");
        string updatedName = "";
        string updatedPhone = "";

        patient.UpdateProfile(updatedName, updatedPhone);

        Assert.Equal("John Doe", patient.Name);
        Assert.Equal("11999999999", patient.Phone);
    }
}