using BootcampBookStore.Data;
using BootcampBookStore.Domain;
using BootcampBookStore.Domain.Entities;
using BootcampBookStore.Domain.Enums;

namespace BootcampBookStore.Services;

public class BookService
{
    private readonly AccessService _access;

    public BookService(AccessService access)
    {
        _access = access;
    }

    public Book[] GetCatalog(User? actor)
    {
        if (!_access.Can(actor, Permission.ViewCatalog)) return new Book[0];
        return ShopStorage.GetBooks();
    }

    public OperationStatus Register(User? actor, Book? book)
    {
        if (!_access.Can(actor, Permission.RegisterBook))
            return OperationStatus.AccessDenied;
        if (book is null || book.Id <= 0 ||
            string.IsNullOrWhiteSpace(book.Title) || book.Price < 0)
            return OperationStatus.InvalidInput;
        if (book is PrintedBook printed &&
            (printed.Stock < 0 || printed.ShippingCostPerCopy < 0))
            return OperationStatus.InvalidInput;
        if (book is EBook ebook && string.IsNullOrWhiteSpace(ebook.FileFormat))
            return OperationStatus.InvalidInput;
        if (ShopStorage.FindBook(book.Id) is not null)
            return OperationStatus.DuplicateBook;
        return ShopStorage.AddBook(book)
            ? OperationStatus.Success : OperationStatus.StorageFull;
    }

    public OperationStatus ChangePrice(User? actor, int bookId, decimal price)
    {
        if (!_access.Can(actor, Permission.ChangeBookPrice))
            return OperationStatus.AccessDenied;
        if (price < 0) return OperationStatus.InvalidInput;
        Book? book = ShopStorage.FindBook(bookId);
        if (book is null) return OperationStatus.BookNotFound;
        return book.ChangePrice(price)
            ? OperationStatus.Success : OperationStatus.InvalidInput;
    }
}
