namespace FiapSub.Core.Entities;
public class Patient
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string Phone { get; private set; }
    public string CPF { get; private set; }

    public Patient(string name, string email, string password, string phone, string cpf)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.");
        if (!IsValidEmail(email)) throw new ArgumentException("Invalid email.");
        if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Password is required.");

        Name = name;
        Email = email;
        PasswordHash = HashPassword(password);
        Phone = phone;
        CPF = cpf;
    }

    public void UpdateProfile(string name, string phone) 
    {
        if (!string.IsNullOrWhiteSpace(name)) Name = name;
        if (!string.IsNullOrWhiteSpace(phone)) Phone = phone;
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