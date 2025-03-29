using FiapSub.Core.Enums;

namespace FiapSub.API.DTOs;

public class UpdateDoctorProfileDTO
{
    public string Name { get; set; }
    public string Phone { get; set; }
    public Specialty Specialty { get; set; }
}