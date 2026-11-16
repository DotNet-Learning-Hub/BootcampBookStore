namespace BootcampBookStore.Domain.Entities;

public class EBook : Book
{
    public string FileFormat { get; }

    public EBook(int id, string title, decimal price, string fileFormat)
        : base(id, title, price)
    {
        FileFormat = fileFormat;
    }

    public override decimal CalculateShippingCost(int quantity)
    {
        return 0m;
    }

    public override string GetDescription()
    {
        return base.GetDescription() + $" | Digital ({FileFormat})";
    }
}
