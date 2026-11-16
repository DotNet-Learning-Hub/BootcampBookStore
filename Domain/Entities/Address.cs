namespace BootcampBookStore.Domain.Entities;

public class Address
{
    public string City { get; }
    public string Street { get; }

    // The caller validates the values before creating an address.
    public Address(string city, string street)
    {
        City = city;
        Street = street;
    }

    public override string ToString() => $"{City}, {Street}";
}
