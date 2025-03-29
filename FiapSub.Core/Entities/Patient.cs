namespace FiapSub.Core.Entities;
public class Patient : User
{
    public string Name { get; private set; }
    public string Phone { get; private set; }
    public string CPF { get; private set; }

    public Patient(string name, string email, string password, string phone, string cpf)
        : base(email, password)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.");
        if (!IsValidEmail(email)) throw new ArgumentException("Invalid email.");
        if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Password is required.");

        Name = name;
        Phone = phone;
        CPF = cpf;
    }

    protected Patient() : base() { }

    public void UpdateProfile(string name, string phone) 
    {
        if (!string.IsNullOrWhiteSpace(name)) Name = name;
        if (!string.IsNullOrWhiteSpace(phone)) Phone = phone;
    }

    public bool IsValidEmail(string email) 
    {
        return !string.IsNullOrWhiteSpace(email) && email.Contains("@");
    }
}