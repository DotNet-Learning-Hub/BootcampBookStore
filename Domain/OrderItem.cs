using BootcampBookStore.Domain.Entities;

namespace BootcampBookStore.Domain;

public class OrderItem
{
    public Book Book { get; }
    public int Quantity { get; }
    public decimal UnitPriceAtPurchase { get; }
    public decimal ShippingAtPurchase { get; }

    public OrderItem(Book book, int quantity)
    {
        Book = book;
        Quantity = quantity;
        UnitPriceAtPurchase = book.Price;
        ShippingAtPurchase = book.CalculateShippingCost(quantity);
    }

    public decimal GetLineTotal()
    {
        return UnitPriceAtPurchase * Quantity + ShippingAtPurchase;
    }
}
