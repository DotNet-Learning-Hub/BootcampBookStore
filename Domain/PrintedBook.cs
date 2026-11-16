using BootcampBookStore.Domain.Entities;

namespace BootcampBookStore.Domain;

public class PrintedBook : Book
{
    public int Stock { get; private set; }
    public decimal ShippingCostPerCopy { get; }

    public PrintedBook(int id, string title, decimal price,
        int stock, decimal shippingCostPerCopy)
        : base(id, title, price)
    {
        Stock = stock;
        ShippingCostPerCopy = shippingCostPerCopy;
    }

    public override decimal CalculateShippingCost(int quantity)
    {
        return quantity * ShippingCostPerCopy;
    }

    public bool HasStock(int quantity)
    {
        return quantity > 0 && Stock >= quantity;
    }

    public bool ReduceStock(int quantity)
    {
        if (!HasStock(quantity)) return false;
        Stock -= quantity;
        return true;
    }

    public override string GetDescription()
    {
        return base.GetDescription() + $" | Printed | Stock: {Stock}";
    }
}
