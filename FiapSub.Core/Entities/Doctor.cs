namespace FiapSub.Core.Entities;

public class Doctor
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string Phone { get; private set; }
    public string CRM { get; private set; }
    public Specialty Specialty { get; private set; } = Specialty.None;

    public Doctor(string name, string email, string password, string phone, string crm, Specialty specialty)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.");
        if (!IsValidEmail(email)) throw new ArgumentException("Invalid email.");
        if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Password is required.");

        Name = name;
        Email = email;
        PasswordHash = HashPassword(password);
        Phone = phone;
        CRM = crm;
        Specialty = specialty;
    }

    public void UpdateProfile(string name, string phone, Specialty specialty) 
    {
        if (!string.IsNullOrWhiteSpace(name)) Name = name;
        if (!string.IsNullOrWhiteSpace(phone)) Phone = phone;
        if (specialty != Specialty.None) Specialty = specialty;
    }

    public bool IsValidEmail(string email) 
    {
        return !string.IsNullOrWhiteSpace(email) && email.Contains("@");
    }

    public string HashPassword(string password) 
    {
        return "hashed_" + password;
    }
}