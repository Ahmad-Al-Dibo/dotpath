using System.ComponentModel;
using System.Diagnostics;

namespace PathsEnvariament.Services;

public static class WindowsCommandRunner
{
    public static void Run(string commandText)
    {
        if (string.IsNullOrWhiteSpace(commandText))
        {
            ConsoleUi.WriteError("Gebruik: cmd <commando>, bijvoorbeeld 'cmd dir'.");
            return;
        }

        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/d /s /c \"{commandText}\"",
                WorkingDirectory = Environment.CurrentDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            if (process is null)
            {
                ConsoleUi.WriteError("CMD kon niet worden gestart.");
                return;
            }

            var standardOutput = process.StandardOutput.ReadToEnd();
            var standardError = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (!string.IsNullOrEmpty(standardOutput))
            {
                Console.Write(standardOutput);
            }

            if (!string.IsNullOrEmpty(standardError))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(standardError);
                Console.ResetColor();
            }
        }
        catch (Win32Exception ex)
        {
            ConsoleUi.WriteError($"Process start failed: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            ConsoleUi.WriteError($"Process start failed: {ex.Message}");
        }
        catch (UnauthorizedAccessException)
        {
            ConsoleUi.WriteError("Error: Access denied while starting the CMD process.");
        }
        catch (Exception ex)
        {
            ConsoleUi.WriteError($"Error: {ex.Message}");
        }
    }
}
