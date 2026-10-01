using System.Diagnostics;
using dotpath.Models;
using dotpath.Messages;

namespace dotpath.Services;

public sealed class ShortPathApp
{
    private readonly string storagePath;
    private readonly List<SavedPath> savedPaths;

    private string promptText = string.Empty;
    private bool developerModeEnabled;

    public ShortPathApp()
    {
        storagePath = PathStorageService.GetStoragePath();
        savedPaths = PathStorageService.Load(storagePath);
    }

    public void Run()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("Short Paths - snelle toegang tot bestanden en folders");
        Console.WriteLine("Typ 'help' voor alle commando's. Typ 'exit' om te stoppen.\n");

        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            var promptPrefix = string.IsNullOrWhiteSpace(promptText) ? string.Empty : $"{promptText} ";
            var modePrefix = developerModeEnabled ? "[DEV] " : string.Empty;
            Console.Write($"{promptPrefix}{modePrefix}{Environment.CurrentDirectory}> ");
            Console.ResetColor();

            var input = CommandInputReader.Read();
            if (string.IsNullOrWhiteSpace(input))
            {
                continue;
            }

            try
            {
                ExecuteInput(input);
            }
            catch (UnauthorizedAccessException)
            {
                ConsoleUi.WriteError("Error: Access denied.");
            }
            catch (IOException ex)
            {
                ConsoleUi.WriteError($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                ConsoleUi.WriteError($"Error: {ex.Message}");
            }
        }
    }

    private void ExecuteInput(string input)
    {
        var normalizedInput = input.Replace("\r\n", "\n", StringComparison.Ordinal).Replace('\r', '\n');
        var lines = normalizedInput.Split('\n');
        if (lines.Length == 1)
        {
            var (command, argument) = CommandParser.Parse(lines[0]);
            ExecuteWithDeveloperOutput(command, argument);
            return;
        }

        var (firstCommand, firstArgument) = CommandParser.Parse(lines[0]);
        if (firstCommand == "cmd")
        {
            var commandScript = string.Join(Environment.NewLine, new[] { firstArgument }.Concat(lines.Skip(1)));
            ExecuteWithDeveloperOutput("cmd", commandScript);
            return;
        }

        foreach (var line in lines.Where(line => !string.IsNullOrWhiteSpace(line)))
        {
            var (command, argument) = CommandParser.Parse(line);
            ExecuteWithDeveloperOutput(command, argument);
        }
    }

    private void ExecuteWithDeveloperOutput(string command, string argument)
    {
        if (!ShouldVisualizeCommandOutput(command, argument))
        {
            ExecuteCommand(command, argument);
            return;
        }

        var originalOutput = Console.Out;
        using var capturedOutput = new StringWriter();
        try
        {
            Console.SetOut(capturedOutput);
            ExecuteCommand(command, argument);
        }
        finally
        {
            Console.SetOut(originalOutput);
        }

        DeveloperModeService.StreamCommandOutput(capturedOutput.ToString());
    }

    private bool ShouldVisualizeCommandOutput(string command, string argument)
    {
        if (!developerModeEnabled || command is "clear" or "cls" or "exit" or "stop"
            or "add" or "toevoegen" or "edit" or "bewerken" or "delete" or "verwijder")
        {
            return false;
        }

        if (command != "dev")
        {
            return true;
        }

        var (developerCommand, _) = CommandParser.Parse(argument);
        return developerCommand is not ("" or "on" or "off");
    }

    private void ExecuteCommand(string command, string argument)
    {
        switch (command)
        {
            case "help":
                ShowHelp();
                break;

            case "list":
            case "toon":
                ShowPaths();
                break;

            case "check":
            case "controleer":
                CheckPathsArgument(argument);
                break;

            case "pwd":
                ShowCurrentPath();
                break;

            case "cd":
                ChangeDirectory(argument);
                break;

            case "ls":
                ListDirectory(argument);
                break;

            case "clear":
            case "cls":
                Console.Clear();
                break;

            case "cmd":
                WindowsCommandRunner.Run(argument);
                break;

            case "prompt":
                promptText = SetPrompt(argument, promptText);
                break;

            case "dev":
                HandleDeveloperCommand(argument);
                break;

            case "add":
            case "toevoegen":
                AddPath();
                break;

            case "edit":
            case "bewerken":
                EditPath(argument);
                break;

            case "delete":
            case "verwijder":
                DeletePath(argument);
                break;

            case "go":
            case "open":
                OpenPath(argument);
                break;

            case "exit":
            case "stop":
                Environment.Exit(0);
                break;

            default:
                WindowsCommandRunner.Run(command + (string.IsNullOrWhiteSpace(argument) ? string.Empty : " " + argument));
                break;
        }
    }

    private void ShowHelp()
    {
        Console.WriteLine(HelpMessages.Help);
    }

    private string SetPrompt(string newPrompt, string currentPrompt)
    {
        if (string.IsNullOrEmpty(newPrompt))
        {
            Console.WriteLine($"Huidige prompt: {currentPrompt}");
            return currentPrompt;
        }

        Console.WriteLine($"Prompt aangepast naar: {newPrompt}");
        return newPrompt;
    }

    private void HandleDeveloperCommand(string arguments)
    {
        if (string.IsNullOrWhiteSpace(arguments))
        {
            SetDeveloperMode(!developerModeEnabled);
            return;
        }

        var (developerCommand, commandArguments) = CommandParser.Parse(arguments);
        switch (developerCommand)
        {
            case "on":
                SetDeveloperMode(true);
                break;
            case "off":
                SetDeveloperMode(false);
                break;
            case "help":
                Console.WriteLine(DeveloperMessages.Help);
                break;
            case "tools":
                DeveloperModeService.ShowAvailableTools();
                break;
            case "generate":
                ConsoleUi.WriteError("Voer een commando uit; developer mode animeert de echte opdrachtuitvoer.");
                break;
            case "status":
                WindowsCommandRunner.Run("git status" + (string.IsNullOrWhiteSpace(commandArguments) ? string.Empty : $" {commandArguments}"));
                break;
            case "build":
            case "test":
            case "run":
                WindowsCommandRunner.Run($"dotnet {developerCommand}" + (string.IsNullOrWhiteSpace(commandArguments) ? string.Empty : $" {commandArguments}"));
                break;
            default:
                WindowsCommandRunner.Run(arguments);
                break;
        }
    }

    private void SetDeveloperMode(bool enabled)
    {
        if (developerModeEnabled == enabled)
        {
            Console.WriteLine(enabled ? "Developer mode is already enabled." : "Developer mode is already disabled.");
            return;
        }

        developerModeEnabled = enabled;
        if (enabled)
        {
            Console.WriteLine();
            DeveloperModeService.ShowActivationAnimation();
            return;
        }

        Console.WriteLine("Developer mode disabled.");
    }

    private void ShowPaths()
    {
        if (savedPaths.Count == 0)
        {
            Console.WriteLine("Geen paths opgeslagen.");
            return;
        }

        Console.WriteLine("\nOpgeslagen variabelen en paths:");
        foreach (var path in savedPaths)
        {
            Console.Write($"  {path.Name,-20} {path.Path,-60} ");
            ConsoleUi.WriteStatus(path.Path);
        }

        Console.WriteLine();
    }

    private void ShowCurrentPath()
    {
        Console.WriteLine(Environment.CurrentDirectory);
    }

    private void ChangeDirectory(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            ShowCurrentPath();
            return;
        }

        try
        {
            var resolved = PathResolver.Resolve(path, savedPaths);
            var targetPath = Path.IsPathRooted(resolved)
                ? resolved
                : Path.GetFullPath(resolved, Environment.CurrentDirectory);

            if (!Directory.Exists(targetPath))
            {
                ConsoleUi.WriteError($"Error: Directory not found. '{targetPath}'");
                return;
            }

            Directory.SetCurrentDirectory(targetPath);
        }
        catch (UnauthorizedAccessException)
        {
            ConsoleUi.WriteError($"Error: Access denied for '{path}'.");
        }
        catch (InvalidOperationException ex)
        {
            ConsoleUi.WriteError($"Error: {ex.Message}");
        }
        catch (IOException)
        {
            ConsoleUi.WriteError($"Error: Directory not found. '{path}'");
        }
        catch (ArgumentException ex)
        {
            ConsoleUi.WriteError($"Error: {ex.Message}");
        }
    }

    private void ListDirectory(string arguments)
    {
        try
        {
            var remainingArguments = arguments.TrimStart();
            var showDetails = false;
            string? extensionFilter = null;

            while (!string.IsNullOrWhiteSpace(remainingArguments))
            {
                var option = ReadFirstToken(remainingArguments, out var consumedLength);
                if (!option.StartsWith('-'))
                {
                    break;
                }

                remainingArguments = remainingArguments[consumedLength..].TrimStart();
                switch (option)
                {
                    case "-r":
                        showDetails = true;
                        break;
                    case "-e":
                        if (string.IsNullOrWhiteSpace(remainingArguments))
                        {
                            ConsoleUi.WriteError("Gebruik: ls -e <extensie> [<path>]");
                            return;
                        }

                        extensionFilter = ReadFirstToken(remainingArguments, out consumedLength);
                        if (extensionFilter.StartsWith('-'))
                        {
                            ConsoleUi.WriteError("Gebruik: ls -e <extensie> [<path>]");
                            return;
                        }

                        remainingArguments = remainingArguments[consumedLength..].TrimStart();
                        extensionFilter = extensionFilter.StartsWith('.') ? extensionFilter : $".{extensionFilter}";
                        break;
                    default:
                        ConsoleUi.WriteError($"Onbekende ls-optie '{option}'. Gebruik: ls [-e <extensie>] [-r] [<path>]");
                        return;
                }
            }

            var target = string.IsNullOrWhiteSpace(remainingArguments)
                ? Environment.CurrentDirectory
                : remainingArguments;
            var resolved = PathResolver.Resolve(target, savedPaths);
            var fullPath = Path.IsPathRooted(resolved)
                ? resolved
                : Path.GetFullPath(resolved, Environment.CurrentDirectory);

            if (!Directory.Exists(fullPath))
            {
                ConsoleUi.WriteError($"Error: Directory not found. '{fullPath}'");
                return;
            }

            foreach (var entry in Directory.EnumerateFileSystemEntries(fullPath).OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
            {
                var name = Path.GetFileName(entry);
                var isDirectory = Directory.Exists(entry);
                if (extensionFilter is not null && (isDirectory || !Path.GetExtension(entry).Equals(extensionFilter, StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

                if (!showDetails)
                {
                    Console.WriteLine(isDirectory ? $"[DIR]  {name}" : $"       {name}");
                    continue;
                }

                var lastModified = isDirectory ? Directory.GetLastWriteTime(entry) : File.GetLastWriteTime(entry);
                var size = isDirectory ? "<DIR>" : $"{new FileInfo(entry).Length:N0} B";
                var type = isDirectory ? "[DIR]" : "[FILE]";
                Console.WriteLine($"{lastModified:yyyy-MM-dd HH:mm} {size,14} {type,-6} {name}");
            }
        }
        catch (UnauthorizedAccessException)
        {
            ConsoleUi.WriteError($"Error: Access denied for '{arguments}'.");
        }
        catch (InvalidOperationException ex)
        {
            ConsoleUi.WriteError($"Error: {ex.Message}");
        }
        catch (IOException)
        {
            ConsoleUi.WriteError($"Error: Directory not found. '{arguments}'");
        }
        catch (ArgumentException ex)
        {
            ConsoleUi.WriteError($"Error: {ex.Message}");
        }
    }

    private static string ReadFirstToken(string input, out int consumedLength)
    {
        var trimmed = input.TrimStart();
        var leadingWhitespace = input.Length - trimmed.Length;
        if (trimmed.StartsWith('"'))
        {
            var closingQuote = trimmed.IndexOf('"', 1);
            if (closingQuote < 0)
            {
                throw new ArgumentException("A quoted argument is missing its closing quote.");
            }

            consumedLength = leadingWhitespace + closingQuote + 1;
            return trimmed[1..closingQuote];
        }

        var tokenEnd = trimmed.IndexOfAny([' ', '\t']);
        if (tokenEnd < 0)
        {
            consumedLength = input.Length;
            return trimmed;
        }

        consumedLength = leadingWhitespace + tokenEnd;
        return trimmed[..tokenEnd];
    }

    private void CheckPathsArgument(string argument)
    {
        if (string.IsNullOrWhiteSpace(argument))
        {
            CheckPaths();
            return;
        }

        var resolved = PathResolver.Resolve(argument, savedPaths);
        Console.Write($"{argument}: ");
        ConsoleUi.WriteStatus(resolved);
    }

    private void CheckPaths()
    {
        if (savedPaths.Count == 0)
        {
            Console.WriteLine("Geen paths om te controleren.");
            return;
        }

        foreach (var savedPath in savedPaths)
        {
            Console.Write($"{savedPath.Name}: {savedPath.Path} ");
            ConsoleUi.WriteStatus(savedPath.Path);
        }
    }

    private void AddPath()
    {
        var name = SavedPath.NormalizeName(ConsoleUi.ReadRequired("Naam/variabele: "));
        if (string.IsNullOrWhiteSpace(name))
        {
            ConsoleUi.WriteError("Naam mag niet leeg zijn.");
            return;
        }

        if (savedPaths.Any(path => path.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
        {
            ConsoleUi.WriteError("Deze naam bestaat al.");
            return;
        }

        var rawPath = ConsoleUi.ReadRequired("Path naar bestand of folder: ");
        savedPaths.Add(new SavedPath(name, rawPath));
        PathStorageService.Save(storagePath, savedPaths);
        ConsoleUi.WriteSuccess("Path opgeslagen.");
    }

    private void EditPath(string name)
    {
        var targetName = string.IsNullOrWhiteSpace(name) ? ConsoleUi.ReadRequired("Welke naam wil je bewerken? ") : name;
        var savedPath = FindPath(targetName);
        if (savedPath is null)
        {
            return;
        }

        Console.WriteLine($"Huidige naam: {savedPath.Name}");
        var newName = ConsoleUi.ReadOptional("Nieuwe naam (Enter = behouden): ");
        var newPath = ConsoleUi.ReadOptional($"Nieuw path (Enter = {savedPath.Path}): ");

        if (!string.IsNullOrWhiteSpace(newName) && savedPaths.Any(path => path != savedPath && path.Name.Equals(SavedPath.NormalizeName(newName), StringComparison.OrdinalIgnoreCase)))
        {
            ConsoleUi.WriteError("Deze nieuwe naam bestaat al.");
            return;
        }

        savedPath.Name = string.IsNullOrWhiteSpace(newName) ? savedPath.Name : SavedPath.NormalizeName(newName);
        savedPath.Path = string.IsNullOrWhiteSpace(newPath) ? savedPath.Path : newPath.Trim().Trim('"');
        PathStorageService.Save(storagePath, savedPaths);
        ConsoleUi.WriteSuccess("Path bijgewerkt.");
    }

    private void DeletePath(string name)
    {
        var targetName = string.IsNullOrWhiteSpace(name) ? ConsoleUi.ReadRequired("Welke naam wil je verwijderen? ") : name;
        var savedPath = FindPath(targetName);
        if (savedPath is null)
        {
            return;
        }

        savedPaths.Remove(savedPath);
        PathStorageService.Save(storagePath, savedPaths);
        ConsoleUi.WriteSuccess("Opgeslagen path verwijderd. Het echte bestand of de folder is niet verwijderd.");
    }

    private void OpenPath(string name)
    {
        var targetName = string.IsNullOrWhiteSpace(name) ? ConsoleUi.ReadRequired("Welke naam wil je openen? ") : name;
        var savedPath = FindPath(targetName);
        if (savedPath is null)
        {
            return;
        }

        var target = savedPath.Path.Trim().Trim('"');
        if (!File.Exists(target) && !Directory.Exists(target))
        {
            ConsoleUi.WriteError($"The target could not be opened: {target}");
            return;
        }

        try
        {
            if (Directory.Exists(target))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "explorer.exe",
                    Arguments = $"\"{target}\"",
                    UseShellExecute = true
                });
                return;
            }

            Process.Start(new ProcessStartInfo(target) { UseShellExecute = true });
        }
        catch (UnauthorizedAccessException)
        {
            ConsoleUi.WriteError($"Access denied: {target}");
        }
        catch (IOException)
        {
            ConsoleUi.WriteError($"The target could not be opened: {target}");
        }
        catch (Exception ex)
        {
            ConsoleUi.WriteError($"The target could not be opened: {ex.Message}");
        }
    }

    private SavedPath? FindPath(string name)
    {
        var normalized = PathResolver.NormalizeName(name);
        var savedPath = savedPaths.FirstOrDefault(path => path.Name.Equals(normalized, StringComparison.OrdinalIgnoreCase));
        if (savedPath is null)
        {
            ConsoleUi.WriteError($"Geen opgeslagen path gevonden met naam '{name}'.");
        }

        return savedPath;
    }
}
