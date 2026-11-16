using BootcampBookStore.Domain;
using BootcampBookStore.Domain.Entities;
using BootcampBookStore.Domain.Enums;

namespace BootcampBookStore.Data;

// One shared in-memory storage for this single-process classroom exercise.
public static class ShopStorage
{
    private static readonly Book?[] _books = new Book?[20];
    private static readonly User?[] _users = new User?[10];
    private static readonly Order?[] _orders = new Order?[30];
    private static int _bookCount;
    private static int _userCount;
    private static int _orderCount;
    private static int _nextOrderId = 1;

    static ShopStorage()
    {
        Console.WriteLine("[Storage] Initialized once for this program run.");
        AddBook(new PrintedBook(101, "C# Fundamentals", 200000m, 5, 25000m));
        AddBook(new PrintedBook(102, "Clean Code", 350000m, 2, 30000m));
        AddBook(new EBook(103, "Algorithms PDF", 120000m, "PDF"));

        AddUser(new User(1, "admin", "Store Administrator", "Admin@123",
            UserRole.Admin, null));
        AddUser(new User(2, "operator", "Store Operator", "Operator@123",
            UserRole.Operator, null));
        AddUser(new User(3, "sara", "Sara", "Sara@123",
            UserRole.Customer, new Address("Tehran", "Valiasr")));
        AddUser(new User(4, "ali", "Ali", "Ali@123",
            UserRole.Customer, new Address("Shiraz", "Zand")));
        AddUser(new User(5, "mina", "Mina", "Mina@123",
            UserRole.Customer, new Address("Isfahan", "Chaharbagh")));
    }

    public static bool AddBook(Book? book)
    {
        if (book is null || _bookCount >= _books.Length ||
            FindBook(book.Id) is not null) return false;
        _books[_bookCount] = book;
        _bookCount++;
        return true;
    }

    public static Book? FindBook(int id)
    {
        for (int i = 0; i < _bookCount; i++)
            if (_books[i]!.Id == id) return _books[i];
        return null;
    }

    public static Book[] GetBooks()
    {
        Book[] result = new Book[_bookCount];
        for (int i = 0; i < _bookCount; i++) result[i] = _books[i]!;
        return result;
    }

    private static bool AddUser(User? user)
    {
        if (user is null || user.Id <= 0 ||
            string.IsNullOrWhiteSpace(user.Username) ||
            _userCount >= _users.Length ||
            FindUserById(user.Id) is not null ||
            FindUserByUsername(user.Username) is not null) return false;
        _users[_userCount] = user;
        _userCount++;
        return true;
    }

    public static User? FindUserById(int id)
    {
        for (int i = 0; i < _userCount; i++)
            if (_users[i]!.Id == id) return _users[i];
        return null;
    }

    public static User? FindUserByUsername(string username)
    {
        for (int i = 0; i < _userCount; i++)
            if (string.Equals(_users[i]!.Username, username,
                StringComparison.OrdinalIgnoreCase)) return _users[i];
        return null;
    }

    public static User[] GetUsers()
    {
        User[] result = new User[_userCount];
        for (int i = 0; i < _userCount; i++) result[i] = _users[i]!;
        return result;
    }

    public static bool AddOrder(Order? order)
    {
        if (order is null || order.Id <= 0 ||
            _orderCount >= _orders.Length || FindOrder(order.Id) is not null)
            return false;
        _orders[_orderCount] = order;
        _orderCount++;
        return true;
    }

    public static Order? FindOrder(int id)
    {
        for (int i = 0; i < _orderCount; i++)
            if (_orders[i]!.Id == id) return _orders[i];
        return null;
    }

    public static Order[] GetOrders()
    {
        Order[] result = new Order[_orderCount];
        for (int i = 0; i < _orderCount; i++) result[i] = _orders[i]!;
        return result;
    }

    public static int GetNextOrderId()
    {
        int id = _nextOrderId;
        _nextOrderId++;
        return id;
    }

    public static int BookCount => _bookCount;
    public static int UserCount => _userCount;
    public static int OrderCount => _orderCount;
}
