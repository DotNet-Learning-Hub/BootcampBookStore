namespace BootcampBookStore.Domain.Entities;

// All books have common properties, but delivery differs by type.
public abstract class Book
{
    public int Id { get; }
    public string Title { get; }
    public decimal Price { get; private set; }

    protected Book(int id, string title, decimal price)
    {
        Id = id;
        Title = title;
        Price = price;
    }

    public bool ChangePrice(decimal newPrice)
    {
        if (newPrice < 0) return false;
        Price = newPrice;
        return true;
    }

    public abstract decimal CalculateShippingCost(int quantity);

    public virtual string GetDescription()
    {
        return $"#{Id} | {Title} | {Price:N0} toman";
    }
}
