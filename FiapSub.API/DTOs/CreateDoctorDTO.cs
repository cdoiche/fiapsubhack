using FiapSub.Core.Enums;

namespace FiapSub.API.DTOs;

public class CreateDoctorDTO
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Phone { get; set; }
    public Specialty Specialty { get; set; }
    public string CRM { get; set; }
}