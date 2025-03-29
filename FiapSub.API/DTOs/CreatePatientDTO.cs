namespace FiapSub.API.DTOs;

public class CreatePatientDTO
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Phone { get; set; }
    public string CPF { get; set; }
}