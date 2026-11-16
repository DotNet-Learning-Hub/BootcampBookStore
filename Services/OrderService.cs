using BootcampBookStore.Data;
using BootcampBookStore.Domain;
using BootcampBookStore.Domain.Entities;
using BootcampBookStore.Domain.Enums;

namespace BootcampBookStore.Services;

public class OrderService
{
    private readonly AccessService _access;

    public OrderService(AccessService access)
    {
        _access = access;
    }

    public Order? CreateOrder(User? actor)
    {
        if (!_access.Can(actor, Permission.CreateOrder) ||
            actor is null || actor.Address is null) return null;
        if (string.IsNullOrWhiteSpace(actor.Address.City) ||
            string.IsNullOrWhiteSpace(actor.Address.Street)) return null;

        // Capture the shipping address when the order is created.
        Address shipping = new Address(actor.Address.City, actor.Address.Street);
        Order order = new Order(ShopStorage.GetNextOrderId(), actor, shipping);
        return ShopStorage.AddOrder(order) ? order : null;
    }

    public OperationStatus AddBook(User? actor, int orderId,
        int bookId, int quantity)
    {
        if (!_access.Can(actor, Permission.AddBookToOrder))
            return OperationStatus.AccessDenied;
        Order? order = ShopStorage.FindOrder(orderId);
        if (order is null) return OperationStatus.OrderNotFound;
        if (actor is null || order.Customer.Id != actor.Id)
            return OperationStatus.AccessDenied;
        Book? book = ShopStorage.FindBook(bookId);
        if (book is null) return OperationStatus.BookNotFound;
        return order.TryAddItem(book, quantity);
    }

    public Order? GetVisibleOrder(User? actor, int orderId)
    {
        Order? order = ShopStorage.FindOrder(orderId);
        return _access.CanViewOrder(actor, order) ? order : null;
    }

    public Order[] GetVisibleOrders(User? actor)
    {
        if (actor is null) return new Order[0];
        Order[] all = ShopStorage.GetOrders();
        if (_access.Can(actor, Permission.ViewAllOrders)) return all;
        if (!_access.Can(actor, Permission.ViewOwnOrders)) return new Order[0];

        int count = 0;
        foreach (Order order in all)
            if (order.Customer.Id == actor.Id) count++;

        Order[] own = new Order[count];
        int index = 0;
        foreach (Order order in all)
            if (order.Customer.Id == actor.Id) own[index++] = order;
        return own;
    }

    public OperationStatus Checkout(User? actor, int orderId)
    {
        if (!_access.Can(actor, Permission.CheckoutOrder))
            return OperationStatus.AccessDenied;
        Order? order = ShopStorage.FindOrder(orderId);
        if (order is null) return OperationStatus.OrderNotFound;
        if (order.Status == OrderStatus.Placed)
            return OperationStatus.AlreadyPlaced;
        if (order.ItemCount == 0) return OperationStatus.EmptyOrder;

        // Check ALL lines before any stock is changed.
        for (int i = 0; i < order.ItemCount; i++)
        {
            OrderItem item = order.GetItemAt(i)!;
            if (item.Book is PrintedBook printed &&
                !printed.HasStock(item.Quantity))
                return OperationStatus.OutOfStock;
        }

        // Valid for this single-threaded classroom program only.
        for (int i = 0; i < order.ItemCount; i++)
        {
            OrderItem item = order.GetItemAt(i)!;
            if (item.Book is PrintedBook printed)
                printed.ReduceStock(item.Quantity);
        }
        order.MarkPlaced();
        return OperationStatus.Success;
    }
}
