using BootcampBookStore.Domain;
using BootcampBookStore.Domain.Entities;
using BootcampBookStore.Domain.Enums;

namespace BootcampBookStore.Services;

// This classroom RBAC policy does not need subclasses for each role.
public class AccessService
{
    public bool Can(User? user, Permission permission)
    {
        if (user is null) return false;

        if (permission == Permission.ViewCatalog ||
            permission == Permission.ViewProfile ||
            permission == Permission.ChangePassword)
            return true;

        switch (user.Role)
        {
            case UserRole.Admin:
                return permission == Permission.RegisterBook ||
                    permission == Permission.ChangeBookPrice ||
                    permission == Permission.ViewUsers ||
                    permission == Permission.ViewAllOrders ||
                    permission == Permission.CheckoutOrder;
            case UserRole.Operator:
                return permission == Permission.ViewAllOrders ||
                    permission == Permission.CheckoutOrder;
            case UserRole.Customer:
                return permission == Permission.CreateOrder ||
                    permission == Permission.AddBookToOrder ||
                    permission == Permission.ViewOwnOrders ||
                    permission == Permission.ChangeAddress;
            default:
                return false;
        }
    }

    public bool CanViewOrder(User? user, Order? order)
    {
        if (user is null || order is null) return false;
        if (Can(user, Permission.ViewAllOrders)) return true;
        return Can(user, Permission.ViewOwnOrders) &&
            order.Customer.Id == user.Id;
    }
}
