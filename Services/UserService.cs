using BootcampBookStore.Data;
using BootcampBookStore.Domain.Entities;
using BootcampBookStore.Domain.Enums;

namespace BootcampBookStore.Services;

public class UserService
{
    private readonly AccessService _access;

    public UserService(AccessService access)
    {
        _access = access;
    }

    public User? Login(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(password)) return null;

        User? user = ShopStorage.FindUserByUsername(username.Trim());
        if (user is null || !user.VerifyPassword(password)) return null;
        return user;
    }

    public User[] GetAllUsers(User? actor)
    {
        if (!_access.Can(actor, Permission.ViewUsers)) return new User[0];
        return ShopStorage.GetUsers();
    }

    public OperationStatus ChangeMyPassword(User? actor,
        string oldPassword, string newPassword)
    {
        if (!_access.Can(actor, Permission.ChangePassword))
            return OperationStatus.AccessDenied;
        if (actor is null) return OperationStatus.AccessDenied;
        if (!actor.VerifyPassword(oldPassword))
            return OperationStatus.WrongPassword;
        if (!IsStrongPassword(newPassword))
            return OperationStatus.WeakPassword;
        if (!actor.ChangePassword(oldPassword, newPassword))
            return OperationStatus.WrongPassword;
        return OperationStatus.Success;
    }

    public OperationStatus ChangeMyAddress(User? actor,
        string city, string street)
    {
        if (!_access.Can(actor, Permission.ChangeAddress))
            return OperationStatus.AccessDenied;
        if (actor is null || string.IsNullOrWhiteSpace(city) ||
            string.IsNullOrWhiteSpace(street))
            return OperationStatus.InvalidInput;
        return actor.ChangeAddress(new Address(city.Trim(), street.Trim()))
            ? OperationStatus.Success : OperationStatus.InvalidInput;
    }

    // Practice with loops and char helpers: 8+ chars, upper, lower, digit, symbol.
    public bool IsStrongPassword(string password)
    {
        if (string.IsNullOrEmpty(password) || password.Length < 8)
            return false;

        bool upper = false, lower = false, digit = false, symbol = false;
        foreach (char c in password)
        {
            if (char.IsUpper(c)) upper = true;
            else if (char.IsLower(c)) lower = true;
            else if (char.IsDigit(c)) digit = true;
            else if (!char.IsWhiteSpace(c)) symbol = true;
        }
        return upper && lower && digit && symbol;
    }
}
