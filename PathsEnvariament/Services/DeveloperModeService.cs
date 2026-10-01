namespace PathsEnvariament.Services;

public static class DeveloperModeService
{
    private static readonly string[] DeveloperTools = ["git", "dotnet", "node", "npm", "python", "code"];
    private static readonly string[] ExecutableExtensions =
        (Environment.GetEnvironmentVariable("PATHEXT") ?? ".COM;.EXE;.BAT;.CMD;.PS1")
        .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    public static void ShowActivationAnimation()
    {
        if (Console.IsOutputRedirected)
        {
            Console.WriteLine("Developer mode enabled.");
            return;
        }

        var frames = new[] { "[>        ]", "[==>      ]", "[====>    ]", "[======>  ]", "[========>]" };
        foreach (var frame in frames)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"\rStarting developer mode {frame}");
            Thread.Sleep(55);
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\rDeveloper mode ready.                 ");
        Console.ResetColor();
    }

    public static void ShowAvailableTools()
    {
        Console.WriteLine("Developer tools found on PATH:");
        foreach (var tool in DeveloperTools)
        {
            var available = IsAvailableOnPath(tool);
            Console.ForegroundColor = available ? ConsoleColor.Green : ConsoleColor.DarkGray;
            Console.WriteLine($"  {(available ? "[OK]" : "[--]")} {tool}");
        }

        Console.ResetColor();
    }

    private static bool IsAvailableOnPath(string command)
    {
        var pathValue = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
        foreach (var directory in pathValue.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var executablePath = Path.Combine(directory.Trim('"'), command);
            if (File.Exists(executablePath) || ExecutableExtensions.Any(extension => File.Exists(executablePath + extension)))
            {
                return true;
            }
        }

        return false;
    }
}