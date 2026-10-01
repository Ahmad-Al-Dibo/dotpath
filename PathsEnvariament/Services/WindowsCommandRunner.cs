using System.ComponentModel;
using System.Diagnostics;
using System.Text;

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

        string? temporaryScriptPath = null;
        try
        {
            var hasMultipleLines = commandText.Contains('\n') || commandText.Contains('\r');
            var command = commandText;
            if (hasMultipleLines)
            {
                temporaryScriptPath = Path.Combine(Path.GetTempPath(), $"ShortPaths-{Guid.NewGuid():N}.cmd");
                File.WriteAllText(temporaryScriptPath, commandText, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
                command = $"\"{temporaryScriptPath}\"";
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/d /s /c \"{command}\"",
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
        finally
        {
            if (temporaryScriptPath is not null)
            {
                try
                {
                    File.Delete(temporaryScriptPath);
                }
                catch (IOException)
                {
                    ConsoleUi.WriteError("The temporary CMD script could not be removed.");
                }
                catch (UnauthorizedAccessException)
                {
                    ConsoleUi.WriteError("Access denied while removing the temporary CMD script.");
                }
            }
        }
    }
}
