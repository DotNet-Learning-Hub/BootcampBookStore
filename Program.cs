using BootcampBookStore.Data;
using BootcampBookStore.Domain;
using BootcampBookStore.Domain.Entities;
using BootcampBookStore.Domain.Enums;
using BootcampBookStore.Helpers;
using BootcampBookStore.Services;

Console.OutputEncoding = System.Text.Encoding.UTF8;

AccessService access = new();
UserService userService = new(access);
BookService bookService = new(access);
OrderService orderService = new(access);
User? currentUser = null;

while (true)
{
    if (currentUser is null)
    {
        ConsoleKit.Title("LOGIN");
        Console.WriteLine("1. Login");
        Console.WriteLine("0. Exit");
        int choice = ReadInt("Select: ");

        if (choice == 0) break;
        if (choice != 1) { ConsoleKit.Error("Invalid option."); ConsoleKit.Pause(); continue; }

        Console.Write("Username: ");
        string username = Console.ReadLine() ?? "";
        Console.Write("Password: ");
        string password = Console.ReadLine() ?? "";
        currentUser = userService.Login(username, password);

        if (currentUser is null) ConsoleKit.Error("Invalid username or password.");
        else ConsoleKit.Success($"Welcome, {currentUser.FullName}!");
        ConsoleKit.Pause();
        continue;
    }

    User user = currentUser;
    ShowMenu(user);
    int option = ReadInt("Select: ");

    switch (option)
    {
        case 1: ShowCatalog(user); break;
        case 2: ShowProfile(user); break;
        case 3: ChangePassword(user); break;
        case 4: RegisterBook(user); break;
        case 5: ChangePrice(user); break;
        case 6: ShowUsers(user); break;
        case 7: CreateOrder(user); break;
        case 8: AddBook(user); break;
        case 9: ShowOrders(user); break;
        case 10: ShowOrder(user); break;
        case 11: Checkout(user); break;
        case 12: ChangeAddress(user); break;
        case 13: StaticExperiment(user); break;
        case 0: currentUser = null; continue;
        case -1: return;
        default: ConsoleKit.Error("Invalid option."); break;
    }

    ConsoleKit.Pause();
}

void ShowMenu(User user)
{
    ConsoleKit.Title($"{user.Username} ({user.Role})");
    Console.WriteLine("1. Catalog");
    Console.WriteLine("2. My profile");
    Console.WriteLine("3. Change my password");

    if (access.Can(user, Permission.RegisterBook))
    {
        ConsoleKit.Info("\nAdmin features");
        Console.WriteLine("4. Register printed book");
        Console.WriteLine("5. Change book price");
        Console.WriteLine("6. List users");
    }

    if (access.Can(user, Permission.CreateOrder))
    {
        ConsoleKit.Info("\nCustomer features");
        Console.WriteLine("7. Create my order");
        Console.WriteLine("8. Add book to my order");
        Console.WriteLine("12. Change my address");
    }

    ConsoleKit.Info("\nOrder features");
    Console.WriteLine("9. List visible orders");
    Console.WriteLine("10. View order");
    if (access.Can(user, Permission.CheckoutOrder)) Console.WriteLine("11. Checkout an order");
    Console.WriteLine("13. Static experiment");
    Console.WriteLine("0. Logout");
    Console.WriteLine("-1. Exit program\n");
}

void ShowCatalog(User user)
{
    ConsoleKit.Title("CATALOG");
    foreach (Book book in bookService.GetCatalog(user))
        Console.WriteLine(book.GetDescription());
}

void ShowProfile(User user)
{
    ConsoleKit.Title("MY PROFILE");
    Console.WriteLine($"ID: {user.Id}");
    Console.WriteLine($"Name: {user.FullName}");
    Console.WriteLine($"Username: {user.Username}");
    Console.WriteLine($"Role: {user.Role}");
    Console.WriteLine($"Address: {user.Address?.ToString() ?? "Not set"}");
}

void ChangePassword(User user)
{
    ConsoleKit.Title("CHANGE PASSWORD");
    Console.Write("Current password: ");
    string oldPassword = Console.ReadLine() ?? "";
    Console.Write("New password: ");
    string newPassword = Console.ReadLine() ?? "";
    ShowStatus(userService.ChangeMyPassword(user, oldPassword, newPassword));
}

void RegisterBook(User user)
{
    ConsoleKit.Title("REGISTER BOOK");
    int id = ReadInt("Book ID: ");
    Console.Write("Title: ");
    string title = Console.ReadLine() ?? "";
    decimal price = ReadDecimal("Price: ");
    int stock = ReadInt("Stock: ");
    decimal shipping = ReadDecimal("Shipping: ");
    ShowStatus(bookService.Register(user, new PrintedBook(id, title, price, stock, shipping)));
}

void ChangePrice(User user)
{
    ConsoleKit.Title("CHANGE PRICE");
    ShowStatus(bookService.ChangePrice(user, ReadInt("Book ID: "), ReadDecimal("New price: ")));
}

void ShowUsers(User user)
{
    ConsoleKit.Title("USERS");
    foreach (User item in userService.GetAllUsers(user))
        Console.WriteLine($"#{item.Id} | {item.Username} | {item.FullName} | {item.Role}");
}

void CreateOrder(User user)
{
    ConsoleKit.Title("CREATE ORDER");
    Order? order = orderService.CreateOrder(user);
    if (order is null) ConsoleKit.Error("Could not create order.");
    else ConsoleKit.Success($"Order #{order.Id} created.");
}

void AddBook(User user)
{
    ConsoleKit.Title("ADD BOOK TO ORDER");
    int orderId = ReadInt("Order ID: ");
    foreach (Book book in bookService.GetCatalog(user)) Console.WriteLine(book.GetDescription());
    int bookId = ReadInt("Book ID: ");
    int quantity = ReadInt("Quantity: ");
    ShowStatus(orderService.AddBook(user, orderId, bookId, quantity));
}

void ShowOrders(User user)
{
    ConsoleKit.Title("ORDERS");
    Order[] orders = orderService.GetVisibleOrders(user);
    if (orders.Length == 0) ConsoleKit.Info("No visible orders.");
    foreach (Order order in orders)
        Console.WriteLine($"#{order.Id} | {order.Customer.Username} | {order.Status} | {order.GetTotal():N0} toman");
}

void ShowOrder(User user)
{
    ConsoleKit.Title("ORDER DETAILS");
    Order? order = orderService.GetVisibleOrder(user, ReadInt("Order ID: "));
    if (order is null) { ConsoleKit.Error("Order not found or access denied."); return; }

    Console.WriteLine($"Customer: {order.Customer.FullName}");
    Console.WriteLine($"Address: {order.DeliveryAddress}");
    Console.WriteLine($"Status: {order.Status}\n");
    for (int i = 0; i < order.ItemCount; i++)
    {
        OrderItem item = order.GetItemAt(i)!;
        Console.WriteLine($"{item.Book.Title} x {item.Quantity} = {item.GetLineTotal():N0} toman");
    }
    Console.WriteLine($"\nTotal: {order.GetTotal():N0} toman");
}

void Checkout(User user)
{
    ConsoleKit.Title("CHECKOUT");
    ShowStatus(orderService.Checkout(user, ReadInt("Order ID: ")));
}

void ChangeAddress(User user)
{
    ConsoleKit.Title("CHANGE ADDRESS");
    Console.Write("City: ");
    string city = Console.ReadLine() ?? "";
    Console.Write("Street: ");
    string street = Console.ReadLine() ?? "";
    ShowStatus(userService.ChangeMyAddress(user, city, street));
}

void StaticExperiment(User user)
{
    ConsoleKit.Title("STATIC EXPERIMENT");
    BookService secondService = new(access);
    Console.WriteLine($"First service: {bookService.GetCatalog(user).Length} books");
    Console.WriteLine($"Second service: {secondService.GetCatalog(user).Length} books");
    Console.WriteLine($"Storage: {ShopStorage.BookCount} books, {ShopStorage.UserCount} users, {ShopStorage.OrderCount} orders");
}

void ShowStatus(OperationStatus status)
{
    if (status == OperationStatus.Success) ConsoleKit.Success("Success");
    else ConsoleKit.Error(status.ToString());
}

int ReadInt(string message)
{
    Console.Write(message);
    int.TryParse(Console.ReadLine(), out int value);
    return value;
}

decimal ReadDecimal(string message)
{
    Console.Write(message);
    decimal.TryParse(Console.ReadLine(), out decimal value);
    return value;
}
