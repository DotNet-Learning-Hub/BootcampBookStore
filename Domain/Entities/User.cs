using BootcampBookStore.Domain.Enums;

namespace BootcampBookStore.Domain.Entities;

// Role is a property: we do not need three subclasses with identical data.
public class User
{
    private string _password;

    public int Id { get; }
    public string Username { get; }
    public string FullName { get; private set; }
    public UserRole Role { get; }
    public Address? Address { get; private set; }

    // Input and initial demo passwords are prepared by the application.
    public User(int id, string username, string fullName,
        string password, UserRole role, Address? address)
    {
        Id = id;
        Username = username;
        FullName = fullName;
        _password = password;
        Role = role;
        Address = address;
    }

    public bool VerifyPassword(string password)
    {
        return _password == password;
    }

    public bool ChangePassword(string oldPassword, string newPassword)
    {
        if (!VerifyPassword(oldPassword)) return false;
        _password = newPassword;
        return true;
    }

    public bool ChangeAddress(Address? newAddress)
    {
        if (Role != UserRole.Customer || newAddress is null ||
            string.IsNullOrWhiteSpace(newAddress.City) ||
            string.IsNullOrWhiteSpace(newAddress.Street))
            return false;

        Address = newAddress;
        return true;
    }
}
