using System.Globalization;
using System.Text.RegularExpressions;
using PathsEnvariament.Configuration;

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

    public static void StreamCommandOutput(string output)
    {
        var tokens = TokenizeOutput(output);
        var visibleTokenCount = tokens.Count(token => !string.IsNullOrWhiteSpace(token));
        var visibleTokenIndex = 0;

        foreach (var token in tokens)
        {
            Console.Write(token);
            if (string.IsNullOrWhiteSpace(token))
            {
                continue;
            }

            visibleTokenIndex++;

            if (!Console.IsOutputRedirected && visibleTokenIndex < visibleTokenCount)
            {
                Thread.Sleep(GetTokenDelayMs(tokens.Count));
            }
        }
    }

    private static List<string> TokenizeOutput(string output)
    {
        var tokens = new List<string>();
        var pendingWhitespace = new System.Text.StringBuilder();
        foreach (Match match in Regex.Matches(output, @"\s+|[\p{L}\p{N}_]+|[^\s\p{L}\p{N}_]", RegexOptions.CultureInvariant))
        {
            if (string.IsNullOrWhiteSpace(match.Value))
            {
                pendingWhitespace.Append(match.Value);
                continue;
            }

            var isWord = Regex.IsMatch(match.Value, @"^[\p{L}\p{N}_]+$", RegexOptions.CultureInvariant);
            if (!isWord)
            {
                tokens.Add(pendingWhitespace + match.Value);
                pendingWhitespace.Clear();
                continue;
            }

            var pieces = SplitIntoSubwords(match.Value);
            foreach (var piece in pieces)
            {
                tokens.Add(pendingWhitespace + piece);
                pendingWhitespace.Clear();
            }
        }

        if (pendingWhitespace.Length > 0)
        {
            tokens.Add(pendingWhitespace.ToString());
        }

        return tokens;
    }

    private static int GetTokenDelayMs(int tokenCount)
    {
        var safeTokenCount = Math.Max(tokenCount, 1);
        var lengthScale = Math.Clamp(
            Math.Log2(safeTokenCount) / Math.Log2(AppConfig.DeveloperTokenDelayReferenceTokenCount),
            0d,
            1d);
        var targetDelay = AppConfig.DeveloperTokenMaxDelayMs
            - (AppConfig.DeveloperTokenMaxDelayMs - AppConfig.DeveloperTokenMinDelayMs) * lengthScale;
        var jitter = targetDelay * 0.15;
        var minimumDelay = Math.Max(AppConfig.DeveloperTokenMinDelayMs, (int)Math.Floor(targetDelay - jitter));
        var maximumDelay = Math.Min(AppConfig.DeveloperTokenMaxDelayMs, (int)Math.Ceiling(targetDelay + jitter));

        return Random.Shared.Next(minimumDelay, maximumDelay + 1);
    }

    private static List<string> SplitIntoSubwords(string word)
    {
        var elements = new List<string>();
        var enumerator = StringInfo.GetTextElementEnumerator(word);
        while (enumerator.MoveNext())
        {
            elements.Add((string)enumerator.Current!);
        }

        var pieces = new List<string>();
        for (var index = 0; index < elements.Count; index += 3)
        {
            pieces.Add(string.Concat(elements.Skip(index).Take(3)));
        }

        return pieces;
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