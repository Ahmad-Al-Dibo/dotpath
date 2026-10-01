using System.Text;

namespace dotpath.Services;

public static class CommandInputReader
{
    private const int PasteLineBreakWindowMs = 35;

    public static string? Read()
    {
        if (Console.IsInputRedirected)
        {
            return Console.ReadLine();
        }

        var input = new StringBuilder();
        while (true)
        {
            var key = Console.ReadKey(intercept: true);
            if (key.Key == ConsoleKey.Enter)
            {
                if ((key.Modifiers & ConsoleModifiers.Shift) != 0 || HasMoreInputSoon())
                {
                    input.Append(Environment.NewLine);
                    Console.WriteLine();
                    Console.Write(">>> ");
                    continue;
                }

                Console.WriteLine();
                return input.ToString();
            }

            if (key.Key == ConsoleKey.Backspace)
            {
                if (input.Length > 0 && input[^1] is not '\r' and not '\n')
                {
                    input.Length--;
                    Console.Write("\b \b");
                }

                continue;
            }

            if (key.KeyChar != '\0' && !char.IsControl(key.KeyChar))
            {
                input.Append(key.KeyChar);
                Console.Write(key.KeyChar);
            }
        }
    }

    private static bool HasMoreInputSoon()
    {
        var deadline = Environment.TickCount64 + PasteLineBreakWindowMs;
        while (Environment.TickCount64 < deadline)
        {
            if (Console.KeyAvailable)
            {
                return true;
            }

            Thread.Sleep(1);
        }

        return false;
    }
}