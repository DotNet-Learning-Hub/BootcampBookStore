namespace BootcampBookStore.Helpers;

// This class only improves the appearance of the console.
public static class ConsoleKit
{
    public static void Title(string text)
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("========================================");
        Console.WriteLine($"  BOOK STORE - {text}");
        Console.WriteLine("========================================");
        Console.ResetColor();
    }

    public static void Success(string text) => Write(text, ConsoleColor.Green);
    public static void Error(string text) => Write(text, ConsoleColor.Red);
    public static void Info(string text) => Write(text, ConsoleColor.Yellow);

    public static void Pause()
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("\nPress any key to continue...");
        Console.ResetColor();
        Console.ReadKey(true);
    }

    private static void Write(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ResetColor();
    }
}
