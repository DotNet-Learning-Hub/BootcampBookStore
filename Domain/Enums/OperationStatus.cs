namespace BootcampBookStore.Domain.Enums;

public enum OperationStatus
{
    Success,
    AccessDenied,
    InvalidInput,
    WrongPassword,
    WeakPassword,
    StorageFull,
    DuplicateBook,
    BookNotFound,
    OrderNotFound,
    InvalidQuantity,
    DuplicateOrderItem,
    OrderFull,
    AlreadyPlaced,
    EmptyOrder,
    OutOfStock
}
