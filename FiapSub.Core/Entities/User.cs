namespace FiapSub.Core.Entities;

public abstract class User
{
    public int Id { get; protected set; }
    public string Email { get; protected set; }
    public string PasswordHash { get; protected set; }

    protected User(string email, string password)
    {
        Email = email;
        PasswordHash = HashPassword(password);
    }

    protected User()
    {
        Email = "default@default.com";
        PasswordHash = "hashed_default_password";
    }

    private string HashPassword(string password)
    {
        return "hashed_" + password;
    }

    public bool VerifyPassword(string password)
    {
        return PasswordHash == "hashed_" + password;
    }
}