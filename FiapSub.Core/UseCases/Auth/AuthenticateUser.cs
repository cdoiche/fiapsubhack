using FiapSub.Core.Interfaces;

namespace FiapSub.Core.UseCases.Auth;

public class AuthenticateUserUseCase
{
    private readonly IPatientRepository _patientRepository;
    private readonly IDoctorRepository _doctorRepository;

    public AuthenticateUserUseCase(
        IPatientRepository patientRepository,
        IDoctorRepository doctorRepository)
    {
        _patientRepository = patientRepository;
        _doctorRepository = doctorRepository;
    }
    
    public async Task<AuthenticationResult> Execute(string email, string password)
    {
        var patient = await _patientRepository.GetByEmailAsync(email);

        if (patient != null)
        {
            if (patient.VerifyPassword(password))
            {
                return new AuthenticationResult
                {
                    UserId = patient.Id,
                    UserType = "Patient"
                };
            }
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        var doctor = await _doctorRepository.GetByEmailAsync(email);
        if (doctor != null)
        {
            if (doctor.VerifyPassword(password))
            {
                return new AuthenticationResult
                {
                    UserId = doctor.Id,
                    UserType = "Doctor"
                };
            }
            throw new UnauthorizedAccessException("Invalid email or password");
        }
        
        throw new UnauthorizedAccessException("Invalid email or password");
    }

    public class AuthenticationResult
    {
        public int UserId { get; set; }
        public string UserType { get; set; }
    }
}