using BootcampBookStore.Domain.Entities;
using BootcampBookStore.Domain.Enums;

namespace BootcampBookStore.Domain;

public class Order
{
    public const int MaxLines = 10;
    private readonly OrderItem?[] _items = new OrderItem?[MaxLines];

    public int Id { get; }
    public User Customer { get; }
    public Address DeliveryAddress { get; }
    public OrderStatus Status { get; private set; }
    public int ItemCount { get; private set; }

    // Only OrderService calls this after validating the signed-in customer.
    public Order(int id, User customer, Address deliveryAddress)
    {
        Id = id;
        Customer = customer;
        DeliveryAddress = deliveryAddress;
        Status = OrderStatus.Draft;
    }

    public OperationStatus TryAddItem(Book? book, int quantity)
    {
        if (Status != OrderStatus.Draft) return OperationStatus.AlreadyPlaced;
        if (book is null) return OperationStatus.BookNotFound;
        if (quantity <= 0) return OperationStatus.InvalidQuantity;
        if (ItemCount >= MaxLines) return OperationStatus.OrderFull;

        for (int i = 0; i < ItemCount; i++)
            if (_items[i]!.Book.Id == book.Id)
                return OperationStatus.DuplicateOrderItem;

        _items[ItemCount] = new OrderItem(book, quantity);
        ItemCount++;
        return OperationStatus.Success;
    }

    public OrderItem? GetItemAt(int index)
    {
        if (index < 0 || index >= ItemCount) return null;
        return _items[index];
    }

    public decimal GetTotal()
    {
        decimal total = 0m;
        for (int i = 0; i < ItemCount; i++)
            total += _items[i]!.GetLineTotal();
        return total;
    }

    internal void MarkPlaced()
    {
        Status = OrderStatus.Placed;
    }
}
