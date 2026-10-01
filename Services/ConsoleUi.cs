namespace PathsEnvariament.Services;

public static class ConsoleUi
{
    public static void WriteStatus(string path)
    {
        var available = File.Exists(path) || Directory.Exists(path);
        Console.ForegroundColor = available ? ConsoleColor.Green : ConsoleColor.Red;
        Console.WriteLine(available ? "[beschikbaar]" : "[niet beschikbaar - bewaard]");
        Console.ResetColor();
    }

    public static void WriteSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public static void WriteError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public static string ReadRequired(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var value = Console.ReadLine()?.Trim();
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            WriteError("Dit veld mag niet leeg zijn.");
        }
    }

    public static string ReadOptional(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine()?.Trim() ?? string.Empty;
    }
}
